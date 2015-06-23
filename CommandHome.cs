﻿using Rocket.API;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Zamirathe_HomeCommand
{
    public class CommandHome : IRocketCommand
    {
        public bool RunFromConsole
        {
            get
            {
                return false;
            }
        }
        public string Name
        {
            get
            {
                return "home";
            }
        }
        public string Help
        {
            get
            {
                return "Teleports you to your bed if you have one.";
            }
        }
        public string Syntax
        {
            get
            {
                return "";
            }
        }
        public List<string> Aliases
        {
            get { return new List<string>(); }
        }
        public void Execute(RocketPlayer playerid, string[] bed)
        {
            HomePlayer homeplayer = playerid.Player.transform.GetComponent<HomePlayer>();
            object[] cont = HomeCommand.CheckConfig(playerid);
            if (!(bool)cont[0]) return;
            // A bed was found, so let's run a few checks.
            homeplayer.GoHome((Vector3)cont[1], (byte)cont[2], playerid);
        }
    }
}