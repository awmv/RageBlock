namespace RageBlock
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program
    {
        #region Constants

        private const string R = "RageBlock";

        #endregion

        #region Static Fields

        private static readonly List<string> Muted = new List<string>();

        private static Menu m;

        private static readonly string TimeStamp = GetTimestamp(DateTime.Now);

        #endregion

        #region Methods

        private static void Game_OnChat(GameChatEventArgs args)
        {
            if (!m.Item("Status").GetValue<bool>())
            {
                return;
            }
            var regex = new Regex(@"\b" + string.Join(@"\b|\b", Rage.Flame) + @"\b", RegexOptions.IgnoreCase);
            var match = regex.Match(args.Message);
            if (args.Sender.IsMe || Muted.Contains(args.Sender.Name) || !match.Success)
            {
                return;
            }
            if (m.Item("Block").GetValue<StringList>().SelectedIndex == 0)
            {
                Muted.Add(args.Sender.Name);
                Utility.DelayAction.Add(new Random().Next(127, 723), () => 
                    Game.Say("/mute " + args.Sender.Name)
                );
            }
            args.Process = false;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            m = new Menu(R, R, true);
            m.AddItem(new MenuItem("Status", "Enable").SetValue(true)).ValueChanged += ProgramValueChanged;
            m.AddItem(
                new MenuItem("Block", "Block modus:").SetValue(
                    new StringList(new[] { "Mute people", "Only censor people" }))).ValueChanged += ItemValueChanged;
            m.AddToMainMenu();

            Game.OnChat += Game_OnChat;
            Game.OnInput += Game_OnInput;
        }

        private static void Game_OnInput(GameInputEventArgs args)
        {
            if (!m.Item("Status").GetValue<bool>())
            {
                return;
            }
            var regex = new Regex(
                @"^(?!\/(?:whisper|w|reply|r)\b).*\b(" + string.Join(@"\b|\b", Rage.Flame) + @"\b)",
                RegexOptions.IgnoreCase);
            var match = regex.Match(args.Input);
            if (!match.Success)
            {
                return;
            }
            args.Process = false;
            Log(Rage.Jokes[new Random().Next(0, Rage.Jokes.Length)]);
        }

        private static string GetTimestamp(DateTime value)
        {
            return value.ToString("HH:mm");
        }

        private static void ItemValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (e.GetNewValue<StringList>().SelectedIndex == 0)
            {
                return;
            }
            if (Muted == null)
            {
                return;
            }
            foreach (var t in Muted)
            {
                Utility.DelayAction.Add(new Random().Next(127, 723), () =>
                    Game.Say("/mute " + t)
                );
            }
        }

        private static void Log(string value)
        {
            Game.PrintChat("[" + TimeStamp + "] <font color='#eb7577'>" + R + "</font>: " + value);
        }

        private static void Main()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void ProgramValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (e.GetNewValue<bool>())
            {
                return;
            }
            if (Muted == null)
            {
                return;
            }
            foreach (var t in Muted)
            {
                Utility.DelayAction.Add(new Random().Next(127, 723), () =>
                    Game.Say("/mute " + t)
                );
            }
        }

        #endregion
    }
}