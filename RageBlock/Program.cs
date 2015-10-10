namespace RageBlock
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The Program class.
    /// </summary>
    internal class Program
    {
        #region Constants

        /// <summary>
        ///     The RageBlock.
        /// </summary>
        public const string R = "RageBlock";

        #endregion

        #region Static Fields

        /// <summary>
        ///     The Menu.
        /// </summary>
        private static Menu m;

        /// <summary>
        ///     List of Players.
        /// </summary>
        private static List<string> muted = new List<string>();

        #endregion

        #region Methods

        /// <summary>
        ///     Subscribe to the Game.OnGameLoad event.
        /// </summary>
        private static void Main()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        /// <summary>
        ///     Subscribe to the Game.OnGameLoad event.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void Game_OnGameLoad(EventArgs args)
        {
            m = new Menu(R, R, true);
            m.AddItem(new MenuItem("Status", "Enable").SetValue(true)).ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    if (eventArgs.GetNewValue<bool>())
                    {
                        return;
                    }

                    UnMuteAll();
                };
            m.AddItem(
                new MenuItem("Block", "Block modus:").SetValue(new StringList(new[] { "Block and Mute", "Block" })))
             .ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
             {
                 if (eventArgs.GetNewValue<StringList>().SelectedIndex == 0)
                 {
                     return;
                 }

                 UnMuteAll();
             };
            m.AddItem(new MenuItem("CallOut", "Include words that get used to call out")).SetValue(true).ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    if (eventArgs.GetNewValue<bool>())
                    {
                        foreach (var entry in Rage.IDont)
                        {
                            Rage.Flame.Add(entry);
                        }
                    }

                    if (eventArgs.GetNewValue<bool>())
                    {
                        return;
                    }

                    foreach (var entry in Rage.IDont)
                    {
                        Rage.Flame.Remove(entry);
                    }
                };

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
        ///     Subscribe to the Game.OnChat event.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void Game_OnChat(GameChatEventArgs args)
        {
            if (!m.Item("Status").GetValue<bool>() // If RageBlock → Enable → On
                || args.Sender == null // Prevents Exceptions in the console for buying items.
                || args.Sender.IsMe // Prevents the event to run for myself.
                || muted.Any(args.Sender.Name.Contains) // Checks if the SM name exists in muted.
                || !new Regex(@"\b" + string.Join(@"\b|\b", Rage.Flame) + @"\b", RegexOptions.IgnoreCase).Match(
                    args.Message).Success)
            {
                return;
            }

            args.Process = false;

            if (m.Item("Block").GetValue<StringList>().SelectedIndex != 0)
            {
                return;
            }

            muted.Add(args.Sender.Name);
            Utility.DelayAction.Add(new Random().Next(127, 723), () => Game.Say("/mute " + args.Sender.Name));
        }

        /// <summary>
        ///     Subscribe to the Game.OnInput event.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void Game_OnInput(GameInputEventArgs args)
        {
            if (!m.Item("Status").GetValue<bool>()
                || !new Regex(
                        @"^(?!\/(?:whisper|w|reply|r)\b).*\b(" + string.Join(@"\b|\b", Rage.Flame) + @"\b)",
                        RegexOptions.IgnoreCase).Match(args.Input).Success)
            {
                return;
            }

            args.Process = false;
            Log(Rage.Jokes[new Random().Next(0, Rage.Jokes.Length)]);
        }

        /// <summary>
        ///     Console (Log).
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        private static void Log(object value)
        {
            Game.PrintChat("[" + DateTime.Now.ToString("HH:mm") + "] <font color='#eb7577'>" + R + "</font>: " + value);
        }

        /// <summary>
        ///     UnmuteAll (UnMuteAll).
        /// </summary>
        private static void UnMuteAll()
        {
            if (muted == null)
            {
                return;
            }

            for (int[] i = { muted.Count - 1 }; i[0] >= 0; i[0]--)
            {
                Utility.DelayAction.Add(
                    new Random().Next(127, 723),
                    delegate
                        {
                            Game.Say("/mute " + muted[i[0] + 1]);
                            muted.RemoveAt(i[0] + 1);
                        });
            }
        }

        #endregion
    }
}