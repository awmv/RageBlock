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
            M.AddItem(new MenuItem("Block", "Block modus:").SetValue(new StringList(new[] { 
                "Mute people", "Only censor people" 
            })));
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
            if (!M.Item("Status").GetValue<bool>())
            {
                Regex regex = new Regex(@"\b" + string.Join(@"\b|\b", RageBlock.Rage.flame) + @"\b", RegexOptions.IgnoreCase);
                Match match = regex.Match(args.Message);
                if (!args.Sender.IsMe && !muted.Contains(args.Sender.Name) && match.Success)
                {
                    if (M.Item("Block").GetValue<StringList>().SelectedIndex == 0) 
                    {
                        muted.Add(args.Sender.Name);
                    }
                    args.Process = false;
                    Utility.DelayAction.Add(new Random().Next(127, 723), () => 
                        Game.Say("/mute " + args.Sender.Name)
                    );
                    Notifications
                        .AddNotification(new Notification(args.Sender.ChampionName + " has been muted.", 3500)
                        .SetTextColor(Color.OrangeRed)
                        .SetBoxColor(Color.Black));
                }
            }
            //if (M.Item("Status").GetValue<bool>() || M.Item("Block").GetValue<StringList>().SelectedIndex != 0 && muted != null)
            //{
            //    for (int i = 0; i < muted.Count; i++) {
            //        Utility.DelayAction.Add(new Random().Next(127, 723), () => 
            //            Game.Say("/mute " + muted[i])
            //        );
            //    }
            //}
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