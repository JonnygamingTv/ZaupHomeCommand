using System;
using System.Collections;
using System.Collections.Generic;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;
using Rocket.API.Serialisation;

namespace ZaupHomeCommand
{
    public class HomePlayer : UnturnedPlayerComponent
    {
        private bool _goingHome;
        private DateTime _lastCalledHomeCommand;
        private bool _waitRestricted;
        private byte _waitTime;
        public bool movementRestricted;
        public bool canGoHome;
        private Vector3 _bedPos;
        private byte _bedRot;
        private UnturnedPlayer _player;

        public static readonly Dictionary<UnturnedPlayer, HomePlayer> CurrentHomePlayers = new Dictionary<UnturnedPlayer, HomePlayer>();

        public void Awake()
        {
            Rocket.Core.Logging.Logger.Log("Homeplayer is awake");
        }
        protected override void Load()
        {
            _goingHome = false;
            canGoHome = false;
        }
        public void GoHome(Vector3 bedPos, byte bedRot, UnturnedPlayer player)
        {
            Rocket.Core.Logging.Logger.Log("starting gohome");
            _waitRestricted = ZaupHomeCommand.Instance.Configuration.Instance.TeleportWait;
            movementRestricted = ZaupHomeCommand.Instance.Configuration.Instance.MovementRestriction;
            _player = player;
            _bedPos = Vector3.up + bedPos + new Vector3(0f, 0.5f, 0f);
            _bedRot = bedRot;

            if (_waitRestricted)
            {
                // We have to wait to teleport now find out how long
                _lastCalledHomeCommand = DateTime.Now;
                if (ZaupHomeCommand.Instance.waitGroups.ContainsKey("all"))
                    ZaupHomeCommand.Instance.waitGroups.TryGetValue("all", out _waitTime);
                
                else
                {
                    if (player.IsAdmin && ZaupHomeCommand.Instance.waitGroups.ContainsKey("admin"))
                        ZaupHomeCommand.Instance.waitGroups.TryGetValue("admin", out _waitTime);
                    
                    else
                    {
                        // Either not an admin or they don't get special wait restrictions.
                        List<RocketPermissionsGroup> hg = R.Permissions.GetGroups(player, true);
                        if (hg.Count <= 0)
                            Rocket.Core.Logging.Logger.Log("There was an error as a player has no groups!");
                        
                        byte[] time2 = new byte[hg.Count];
                        for (byte g=0;g<hg.Count;g++)
                        {
                            
                            RocketPermissionsGroup hgr = hg[g];
                            ZaupHomeCommand.Instance.waitGroups.TryGetValue(hgr.Id, out time2[g]);
                            if (time2[g] <= 0)
                            {
                                time2[g] = 60;
                            }
                        }
                        Array.Sort(time2);
                        // Take the lowest time.
                        _waitTime = time2[0];
                    }
                }

                UnturnedChat.Say(player,
                    movementRestricted
                        ? string.Format(ZaupHomeCommand.Instance.Configuration.Instance.FoundBedWaitNoMoveMsg,
                            player.CharacterName, _waitTime)
                        : string.Format(ZaupHomeCommand.Instance.Configuration.Instance.FoundBedNowWaitMsg,
                            player.CharacterName, _waitTime));
            }
            else
                canGoHome = true;
            
            _goingHome = true;
            StartCoroutine(DoGoHome());
        }
        private IEnumerator DoGoHome()
        {
            if (_player.Dead)
            {
                // Abort teleport, they died.
                UnturnedChat.Say(_player, string.Format(ZaupHomeCommand.Instance.Configuration.Instance.NoTeleportDiedMsg, _player.CharacterName));
                _goingHome = false;
                canGoHome = false;
                CurrentHomePlayers.Remove(_player);
                yield break;
            }
            Rocket.Core.Logging.Logger.Log("starting dogohome");
            if (!canGoHome)
            {
                CurrentHomePlayers.Remove(_player);
                yield break;
            }
            UnturnedChat.Say(_player, string.Format(ZaupHomeCommand.Instance.Configuration.Instance.TeleportMsg, _player.CharacterName));
            _player.Teleport(_bedPos, _bedRot);
            canGoHome = false;
            _goingHome = false;
            CurrentHomePlayers.Remove(_player);
        }
        public void FixedUpdate()
        {
            if (_waitRestricted && (DateTime.Now - _lastCalledHomeCommand).TotalSeconds < _waitTime || !_goingHome) return;
            
            // We made it this far, we can go home.
            canGoHome = true;
            StartCoroutine(DoGoHome());
        }
    }
}
