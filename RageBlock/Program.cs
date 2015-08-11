using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using System.Text.RegularExpressions;

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
            M.AddItem(new MenuItem("Status", "Enable").SetValue(true));
            M.AddToMainMenu();

            Game.OnChat += Game_OnChat;
            Game.OnInput += Game_OnInput;
        }

        private static String GetTimestamp(DateTime Value) { return Value.ToString("HH:mm"); }

        private static void Log(string value) { 
            Game.PrintChat("[" + timeStamp + "] <font color='#eb7577'>" + r + "</font>: " + value);
        }

        private static void Game_OnChat(GameChatEventArgs args)
        {
            if (!M.Item("Status").GetValue<bool>() && !args.Sender.IsMe) return;
            Regex regex = new Regex(string.Join("|\\b", RageBlock.Rage.flame), RegexOptions.IgnoreCase);
            Match match = regex.Match(args.Message);
            if (match.Success)
            {
                muted.Add(args.Sender.Name);
                Utility.DelayAction.Add(new Random().Next(127, 723), () => Game.Say("/mute " + args.Sender.Name));
                Notifications
                    .AddNotification(new Notification(args.Sender.ChampionName + " has been muted.", 3500)
                    .SetTextColor(Color.OrangeRed)
                    .SetBoxColor(Color.Black));
            }
        }

        private static void Game_OnInput(GameInputEventArgs args)
        {
            if (!M.Item("Status").GetValue<bool>()) return;
            Regex regex = new Regex(string.Join("|\\b", RageBlock.Rage.flame), RegexOptions.IgnoreCase);
            Match match = regex.Match(args.Input);
            if (match.Success)
            {
                args.Process = false;
                Log(RageBlock.Rage.jokes[new Random().Next(0, RageBlock.Rage.jokes.Length)]);
            }
        }
    }
}