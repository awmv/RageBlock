namespace RageBlock
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Color = SharpDX.Color;

    /// <summary>
    ///     Program class.
    /// </summary>
    internal class RageBlock
    {
        #region Constants

        /// <summary>
        ///     RageBlock string.
        /// </summary>
        public const string R = "RageBlock";

        #endregion

        #region Static Fields

        /// <summary>
        ///     List of heroes.
        /// </summary>
        private static readonly List<string> Heroes =
            new List<string>(HeroManager.AllHeroes.Where(me => !me.IsMe).Select(hero => hero.Name).ToList());

        /// <summary>
        ///     Checks if the champion names are unique.
        /// </summary>
        private static readonly bool DuplicateSmName = Heroes.Distinct().Count() != Heroes.Count;

        /// <summary>
        ///     Count of a hero.
        /// </summary>
        private static readonly Dictionary<int, Count> HeroCount = new Dictionary<int, Count>();

        /// <summary>
        ///     Queue Worker in terms of a list.
        /// </summary>
        private static readonly List<Task> Queue = new List<Task>();

        /// <summary>
        ///     Last Tick of Environment.TickCount.
        /// </summary>
        private static float lastTick;

        /// <summary>
        ///     Menu m.
        /// </summary>
        private static Menu m;

        #endregion

        #region Methods

        /// <summary>
        ///     Subscribe to the Game.OnGameLoad event.
        /// </summary>
        private static void Main()
        {
            CustomEvents.Game.OnGameLoad += GameOnGameLoad;
        }

        /// <summary>
        /// Subscribe to the Game.OnGameLoad event.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void GameOnGameLoad(EventArgs args)
        {
            m = new Menu(R, R, true);
            m.AddItem(new MenuItem("Status", "Enable").SetValue(true)).ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    if (eventArgs.GetNewValue<bool>())
                    {
                        m.Item("Block").Show();
                        m.Item("CallOut").Show();
                        m.Item("Ping").Show();
                    }

                    if (eventArgs.GetNewValue<bool>())
                    {
                        return;
                    }

                    m.Item("Block").Show(false);
                    m.Item("CallOut").Show(false);
                    m.Item("Ping").Show(false);
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
            m.AddItem(new MenuItem("CallOut", "Block words for scripting"))
             .SetTooltip("Add/Remove words for scripting to the word filter.")
             .SetValue(true)
             .ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
             {
                 if (eventArgs.GetNewValue<bool>())
                 {
                     foreach (var entry in WordFilter.IDont)
                     {
                         WordFilter.Flame.Add(entry);
                     }

                     WordFilter.Flame.Add(GetRegex(WordFilter.IDont));
                 }

                 if (eventArgs.GetNewValue<bool>())
                 {
                     return;
                 }

                 foreach (var entry in WordFilter.IDont)
                 {
                     WordFilter.Flame.Remove(entry);
                 }

                 WordFilter.Flame.Remove(GetRegex(WordFilter.IDont));
             };

            m.AddItem(new MenuItem("Ping", "Block ping abuser"))
             .SetTooltip("Block every third or further ping once in a while.")
             .SetValue(true);

            m.AddSubMenu(new Menu("Mute list", "Muted"));

            m.SubMenu("Muted").AddSubMenu(new Menu("Allies", "Allies"));

            m.SubMenu("Muted").AddSubMenu(new Menu("Enemies", "Enemies"));

            foreach (var ally in HeroManager.Allies.Where(ally => !ally.IsMe && !ally.IsBot))
            {
                m.SubMenu("Muted")
                 .SubMenu("Allies")
                 .AddItem(new MenuItem(ally.NetworkId.ToString(), ally.ChampionName))
                 .SetFontStyle(FontStyle.Regular, new Color(135, 206, 250))
                 .SetTooltip(DuplicateSmName ? ally.Name : ("@" + ally.ChampionName))
                 .SetValue(false)
                 .ValueChanged += (sender, eventArgs) =>
                 {
                     var name = (MenuItem)sender;

                     if (m.Item("Block").GetValue<StringList>().SelectedIndex != 0)
                     {
                         return;
                     }

                     Queue.Add(new Task(delegate { Game.Say("/mute " + name.Tooltip); }));
                     Log("Release a key to mute " + name.Tooltip + ".");
                 };
            }

            foreach (var enemy in HeroManager.Enemies.Where(enemy => !enemy.IsBot))
            {
                m.SubMenu("Muted")
                 .SubMenu("Enemies")
                 .AddItem(new MenuItem(enemy.NetworkId.ToString(), enemy.ChampionName))
                 .SetFontStyle(FontStyle.Regular, new Color(135, 206, 250))
                 .SetTooltip(DuplicateSmName ? enemy.Name : ("@" + enemy.ChampionName))
                 .SetValue(false)
                 .ValueChanged += (sender, eventArgs) =>
                 {
                     var name = (MenuItem)sender;

                     if (m.Item("Block").GetValue<StringList>().SelectedIndex != 0)
                     {
                         return;
                     }

                     Queue.Add(new Task(delegate { Game.Say("/mute " + name.Tooltip); }));
                     Log("Release a key to mute " + name.Tooltip + ".");
                 };
            }

            m.AddToMainMenu();

            if (m.Item("CallOut").GetValue<bool>())
            {
                foreach (var entry in WordFilter.IDont)
                {
                    WordFilter.Flame.Add(entry);
                }

                WordFilter.Flame.Add(GetRegex(WordFilter.IDont));
            }

            foreach (var hero in
                HeroManager.AllHeroes.Where(me => !me.IsMe && !me.IsBot).Select(hero => hero.NetworkId).ToList())
            {
                HeroCount.Add(hero, new Count());
            }

            Game.OnChat += GameOnChat;
            Game.OnInput += GameOnInput;
            Game.OnPing += GameOnPing;
            Game.OnUpdate += GameOnUpdate;
        }

        /// <summary>
        /// Subscribe to the Game.OnChat event.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void GameOnChat(GameChatEventArgs args)
        {
            if (!m.Item("Status").GetValue<bool>()
                || args.Sender == null
                || args.Sender.IsMe
                || args.Sender.IsBot
                || !args.Sender.IsValid
                || m.Item(args.Sender.NetworkId.ToString()).GetValue<bool>()
                || !new Regex(@"(?<!\S)(" + string.Join(@"|", WordFilter.Flame) + @")(?!\S)", RegexOptions.IgnoreCase)
                        .Match(Regex.Replace(args.Message, @"\p{P}\p{S}", string.Empty)).Success)
            {
                return;
            }

            args.Process = false;

            if (m.Item("Block").GetValue<StringList>().SelectedIndex != 0)
            {
                return;
            }

            m.SubMenu("Muted").SubMenu(args.Sender.IsAlly ? "Allies" : "Enemies").Item(args.Sender.Name).SetValue(true);
        }

        /// <summary>
        /// Subscribe to the Game.OnInput event.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void GameOnInput(GameInputEventArgs args)
        {
            if (!m.Item("Status").GetValue<bool>()
                || !new Regex(
                        @"^(?!\/(?:whisper|w|reply|r)(?!\S)).*(?<!\S)(" + string.Join(@"|", WordFilter.Flame)
                        + @")(?!\S)", 
                        RegexOptions.IgnoreCase).Match(Regex.Replace(args.Input, @"\p{P}\p{S}", string.Empty)).Success)
            {
                return;
            }

            args.Process = false;
            Log(WordFilter.Jokes[new Random().Next(0, WordFilter.Jokes.Length)]);
        }

        /// <summary>
        /// Subscribe to the GameOnPing event.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void GameOnPing(GamePingEventArgs args)
        {
            var hero = (Obj_AI_Hero)args.Source;
            if (!m.Item("Status").GetValue<bool>()
                || !m.Item("Ping").GetValue<bool>()
                || hero == null
                || !hero.IsValid
                || hero.IsMe
                || !HeroCount.ContainsKey(hero.NetworkId))
            {
                return;
            }

            HeroCount[hero.NetworkId].Value++;

            if (HeroCount[hero.NetworkId].Value < 3)
            {
                return;
            }

            args.Process = false;
        }

        /// <summary>
        /// Subscribe to the GameOnUpdate event.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void GameOnUpdate(EventArgs args)
        {
            if (!m.Item("Status").GetValue<bool>()
                || !m.Item("Ping").GetValue<bool>())
            {
                return;
            }

            foreach (var hero in
                HeroManager.AllHeroes.Where(me => !me.IsMe && !me.IsBot).Select(hero => hero.NetworkId).ToList())
            {
                LowerCount(hero);
            }

            if (Orbwalking.Orbwalker.Instances.Count != 0
                && Orbwalking.Orbwalker.Instances.Any(o => o.ActiveMode != Orbwalking.OrbwalkingMode.None))
            {
                return;
            }

            foreach (var task in Queue.ToList().Where(task => Queue.Any()))
            {
                task.Start();
                Queue.Remove(task);
            }
        }

        /// <summary>
        /// GetRegex of Array.
        /// </summary>
        /// <param name="source">
        /// This took some time to do. I hope you appreciate it.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetRegex(IEnumerable<string> source)
        {
            var res = (from str in source
                       where str.Length >= 5
                       select
                           str.Select((t, i) => str.Remove(i, 1).Insert(i, "\\w"))
                              .Aggregate(string.Empty, (current, alt) => current + (alt + "|"))).Aggregate(
                                  string.Empty, 
                                  (current1, pattern) => current1 + pattern);
            res = res.Remove(res.Length - 1);
            return res;
        }

        /// <summary>
        /// Lower the int value of LowerCount.
        /// </summary>
        /// <param name="hero">
        /// The hero.
        /// </param>
        private static void LowerCount(int hero)
        {
            while (!(Environment.TickCount - lastTick < 10000f))
            {
                lastTick = Environment.TickCount;

                if (HeroCount[hero].Value <= 0)
                {
                    continue;
                }

                HeroCount[hero].Value--;
            }
        }

        /// <summary>
        ///     UnmuteAll Summoners at once.
        /// </summary>
        private static void UnMuteAll()
        {
            if (!m.Children.Any(muted => muted.Children.Any(team => team.Items.Any(item => item.GetValue<bool>()))))
            {
                return;
            }

            foreach (var sm in
                m.Children.SelectMany(
                    muted => muted.Children.SelectMany(team => team.Items.Where(item => item.GetValue<bool>()))))
            {
                sm.SetValue(false);
            }
        }

        /// <summary>
        /// Console (Log).
        /// </summary>
        /// <param name="value">
        /// I'm used to work with JavaScript. This looks more familiar to me.
        /// </param>
        private static void Log(object value)
        {
            Game.PrintChat("[" + DateTime.Now.ToString("HH:mm") + "] <font color='#eb7577'>" + R + "</font>: " + value);
        }

        #endregion

        /// <summary>
        ///     Class count.
        /// </summary>
        private class Count
        {
            #region Fields

            /// <summary>
            ///     Int value.
            /// </summary>
            public int Value;

            #endregion
        }
    }
}