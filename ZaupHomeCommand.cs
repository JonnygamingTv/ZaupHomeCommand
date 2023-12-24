using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace ZaupHomeCommand
{
    public class ZaupHomeCommand : RocketPlugin<HomeCommandConfiguration>
    {
        public Dictionary<string, byte> waitGroups;
        public static ZaupHomeCommand Instance;

        protected override void Load()
        {
            Instance = this;
            waitGroups = new Dictionary<string, byte>();
            
            foreach (HomeGroup hg in Configuration.Instance.WaitGroups)
                waitGroups.Add(hg.Id, hg.Wait);


            UnturnedPlayerEvents.OnPlayerUpdatePosition += OPUP;
        }
        public override void UnloadPlugin(PluginState state = PluginState.Unloaded)
        {
            waitGroups.Clear();
            waitGroups = null;
            Instance = null;
            UnturnedPlayerEvents.OnPlayerUpdatePosition -= OPUP;
        }
        void OPUP(UnturnedPlayer player, Vector3 position)
        {
            if (!HomePlayer.CurrentHomePlayers.ContainsKey(player) ||
                !HomePlayer.CurrentHomePlayers[player].movementRestricted) return;
            HomePlayer.CurrentHomePlayers[player].canGoHome = false;
            UnturnedChat.Say(player, string.Format(Instance.Configuration.Instance.UnableMoveSinceMoveMsg, player.CharacterName));
        }
        // All we are doing here is checking the config to see if anything like restricted movement or time restriction is enforced.
        public static object[] CheckConfig(UnturnedPlayer player)
        {
            
            object[] returnv = { false, null, null };
            // First check if command is enabled.
            if (!Instance.Configuration.Instance.Enabled)
            {
                // Command disabled.
                UnturnedChat.Say(player, string.Format(Instance.Configuration.Instance.DisabledMsg, player.CharacterName));
                return returnv;
            }
            // It is enabled, but are they in a vehicle?
            if (player.Stance == EPlayerStance.DRIVING || player.Stance == EPlayerStance.SITTING)
            {
                // They are in a vehicle.
                UnturnedChat.Say(player, string.Format(Instance.Configuration.Instance.NoVehicleMsg, player.CharacterName));
                return returnv;
            }
            // They aren't in a vehicle, so check if they have a bed.    
            if (!BarricadeManager.tryGetBed(player.CSteamID, out var bedPos, out var bedRot))
            {
                // Bed not found.
                UnturnedChat.Say(player, string.Format(Instance.Configuration.Instance.NoBedMsg, player.CharacterName));
                return returnv;
            }
            object[] returnv2 = { true, bedPos, bedRot };
            return returnv2;
        }
    }
}
