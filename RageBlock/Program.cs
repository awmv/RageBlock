﻿using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using System.Text.RegularExpressions;
using System.IO;
using LeagueSharp.Sandbox;

namespace RageBlock
{
    class Program
    {
        static Menu M;
        const string r = "RageBlock";
        static List<string> muted = new List<string>();
        static String timeStamp = GetTimestamp(DateTime.Now);

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }        

        private static void Game_OnGameLoad(EventArgs args)
        {
            M = new Menu(r, r, true);
            M.AddItem(new MenuItem("Status", "Enable")
                .SetValue(true))
                .ValueChanged += Program_ValueChanged;
            M.AddItem(new MenuItem("Block", "Block modus:").SetValue(new StringList(new[] { 
                "Mute people", "Only censor people" 
            })));
            M.AddToMainMenu();

            Game.OnChat += Game_OnChat;
            Game.OnInput += Game_OnInput;
        }

        private static void Program_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (!e.GetNewValue<bool>())
            {
                if (muted != null) return;
                for (int i = 0; i < muted.Count; i++)
                {
                    Utility.DelayAction.Add(new Random().Next(127, 723), () =>
                        Game.Say("/mute " + muted[i])
                    );
                }
            }
        }

        private static String GetTimestamp(DateTime Value) { return Value.ToString("HH:mm"); }

        private static void Log (string value)
        {
            Game.PrintChat("[" + timeStamp + "] <font color='#eb7577'>" + r + "</font>: " + value);
        }

        private static void Game_OnChat(GameChatEventArgs args)
        {            
            Regex regex = new Regex(@"\b" + string.Join(@"\b|\b", RageBlock.Rage.flame) + @"\b", RegexOptions.IgnoreCase);
            Match match = regex.Match(args.Message);
            if (!M.Item("Status").GetValue<bool>())
            if (!args.Sender.IsMe && !muted.Contains(args.Sender.Name) && match.Success)
            {
                if (M.Item("Block").GetValue<StringList>().SelectedIndex == 0)
                {
                    muted.Add(args.Sender.Name);
                    Utility.DelayAction.Add(new Random().Next(127, 723), () =>
                        Game.Say("/mute " + args.Sender.ChampionName)
                    );
                }
                args.Process = false;
            }
        }

        private static void Game_OnInput(GameInputEventArgs args)
        {
            if (!M.Item("Status").GetValue<bool>()) return;
            Regex regex =
                new Regex(@"^(?!\/(?:whisper|w|reply|r)\b).*\b(" + string.Join(@"\b|\b", RageBlock.Rage.flame) + @"\b)", RegexOptions.IgnoreCase);
            Match match = regex.Match(args.Input);
            if (match.Success)
            {
                args.Process = false;
                Log(RageBlock.Rage.jokes[new Random().Next(0, RageBlock.Rage.jokes.Length)]);
            }
        }
    }
}