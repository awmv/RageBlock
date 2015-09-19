namespace RageBlock
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    /// The Program class.
    /// </summary>
    internal class Program
    {
        #region Constants & Static Fields

        /// <summary>
        /// The R.
        /// </summary>
        public const string R = "RageBlock";

        /// <summary>
        /// The Menu m.
        /// </summary>
        private static Menu m;

        /// <summary>
        /// The muted List of Players.
        /// </summary>
        private static List<string> muted = new List<string>();

        #endregion

        #region Methods

        /// <summary>
        /// Subscribe to the Game.OnChat event.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Game_OnChat(GameChatEventArgs args)
        {
            if (!m.Item("Status").GetValue<bool>())
            {
                return;
            }

            var regex = new Regex(@"\b" + string.Join(@"\b|\b", Rage.Flame) + @"\b", RegexOptions.IgnoreCase);
            var match = regex.Match(args.Message);
            if (args.Sender == null)
            {
                return;
            }

            if (args.Sender.IsMe)
            {
                return;
            }

            if (muted.Any(args.Sender.Name.Contains))
            {
                return;
            }

            if (!match.Success)
            {
                return;
            }

            args.Process = false;
            if (m.Item("Block").GetValue<StringList>().SelectedIndex != 0)
            {
                return;
            }

            Utility.DelayAction.Add(new Random().Next(127, 723), () => Game.Say("/mute " + args.Sender.Name));
        }

        /// <summary>
        /// Subscribe to the Game.Load event.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Game_OnGameLoad(EventArgs args)
        {
            m = new Menu(R, R, true);
            m.AddItem(new MenuItem("Status", "Enable").SetValue(true)).ValueChanged += ProgramValueChanged;
            m.AddItem(
                new MenuItem("Block", "Block modus:").SetValue(new StringList(new[] { "Block and Mute", "Block" })))
             .ValueChanged += ItemValueChanged;
            m.AddItem(new MenuItem("CallOut", "Include words that get used to call out")).SetValue(true).ValueChanged +=
                CallOutValueChanged;
            m.AddToMainMenu();

            if (m.Item("CallOut").GetValue<bool>())
            {
                foreach (var entry in Rage.IDont)
                {
                    Rage.Flame.Add(entry);
                }
            }

            Game.OnChat += Game_OnChat;
            Game.OnInput += Game_OnInput;
        }

        /// <summary>
        /// Subscribe to the Game.OnInput event.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
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

        #region ValueChanged

        /// <summary>
        /// The program value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void ProgramValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (e.GetNewValue<bool>())
            {
                return;
            }

            UnMute();
        }

        /// <summary>
        /// The item value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void ItemValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (e.GetNewValue<StringList>().SelectedIndex == 0)
            {
                return;
            }

            UnMute();
        }

        /// <summary>
        /// The call out value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void CallOutValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (!m.Item("CallOut").GetValue<bool>())
            {
                foreach (var entry in Rage.IDont)
                {
                    Rage.Flame.Add(entry);
                }
            }

            if (!m.Item("CallOut").GetValue<bool>())
            {
                return;
            }

            foreach (var entry in Rage.IDont)
            {
                Rage.Flame.Remove(entry);
            }
        }

        #endregion

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        private static void Log(object value)
        {
            Game.PrintChat("[" + DateTime.Now.ToString("HH:mm") + "] <font color='#eb7577'>" + R + "</font>: " + value);
        }

        /// <summary>
        /// The main method.
        /// </summary>
        private static void Main()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        /// <summary>
        /// The un mute.
        /// </summary>
        private static void UnMute()
        {
            if (muted == null)
            {
                return;
            }

            foreach (var t in muted)
            {
                Utility.DelayAction.Add(new Random().Next(127, 723), () => Game.Say("/mute " + t));
                muted.Remove(t);
            }
        }

        #endregion
    }
}