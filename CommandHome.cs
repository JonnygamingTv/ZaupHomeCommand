using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using UnityEngine;

namespace ZaupHomeCommand
{
    public class CommandHome : IRocketCommand
    {
        public string Name => "home";

        public string Help => "Teleports you to your bed if you have one.";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        //?????
        public List<string> Permissions => new List<string>();

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public void Execute(IRocketPlayer caller, string[] bed)
        {
            UnturnedPlayer playerId = (UnturnedPlayer)caller;
            HomePlayer homePlayer = playerId.GetComponent<HomePlayer>();
            object[] cont = ZaupHomeCommand.CheckConfig(playerId);
            if (!(bool)cont[0]) return;
            // A bed was found, so let's run a few checks.
            HomePlayer.CurrentHomePlayers.Add(playerId, homePlayer);
            homePlayer.GoHome((Vector3)cont[1], (byte)cont[2], playerId);
        }
    }
}
