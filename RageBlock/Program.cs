using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using System.IO;
using SharpDX;
using Color = System.Drawing.Color;
using System.Collections;

namespace RageBlock
{
    class Program
    {
        public static Menu M;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            #region menu
            var r = "RageBlock";
            M = new Menu(r, r, true);
            M.AddItem(new MenuItem("Status", "Enable").SetValue(true));

            M.AddToMainMenu();
            #endregion

            Game.OnUpdate += Game_OnUpdate;
            Game.OnChat += Game_OnChat;
            Console.WriteLine("[" + r + "]" + " has been loaded.");
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("HH:mm");
        }

        private static void Game_OnChat(GameChatEventArgs args)
        {
            if (!M.Item("Status").GetValue<bool>()) return;
            String timeStamp = GetTimestamp(DateTime.Now);
            #region flame
            string[] flame = {
	            "bronze", "silver", "gold", "platinum", "plat", "diamond", "master", "challenger", "feed", "retard", "ks", 
                "killsteal", "on purpose", "fu", "fail", "failed", "bad", "gank", "camped", "stupid", "anal", "anus", "arrse", 
                "arse", "ass-fucker", "assfucker", "assfukka", "asshole", "assholes", "asswhole", "mutter", "mother", 
                "b!tch", "b17ch", "b1tch", "ballbag", "ballsack", "bastard", "beastial", "beastiality", "scrub",
                "bellend", "bestial", "bestiality", "bi+ch", "biatch", "bitch", "bitcher", "bitchers", "bitches", "bitchin", 
                "bitching", "bloody", "blow job", "blowjob", "blowjobs", "boiolas", "bollock", "bollok", "boner", 
                "buceta", "bugger", "bum", "bunny fucker", "*", "hure", "lappen", "dumm", "dämlich", "missgeburt",
                "butthole", "buttmuch", "buttplug", "c0ck", "c0cksucker", "carpet muncher", "cawk", "chink", "cipa", 
                "cl1t", "clit", "clitoris", "clits", "cnut", "cock", "cock-sucker", "cockface", "cockhead", "cockmunch", 
                "cockmuncher", "cocks", "cocksuck", "cocksucked", "cocksucker", "cocksucking", "cocksucks", "cocksuka", 
                "cocksukka", "cok", "cokmuncher", "coksucka", "coon", "cox", "crap", "cum", "cummer", "cumming", "cums", 
                "cumshot", "cunilingus", "cunillingus", "cunnilingus", "cunt", "cuntlick", "cuntlicker", "cuntlicking", 
                "cunts", "cyalis", "cyberfuc", "cyberfuck", "cyberfucked", "cyberfucker", "cyberfuckers", "cyberfucking", 
                "d1ck", "damn", "dick", "dickhead", "dildo", "dildos", "dink", "dinks", "dirsa", "dlck", "dog-fucker", 
                "doggin", "dogging", "donkeyribber", "doosh", "duche", "dyke", "ejaculate", "ejaculated", "ejaculates", 
                "ejaculating", "ejaculatings", "ejaculation", "ejakulate", "f u c k", "f u c k e r", "f4nny", "fag", 
                "fagging", "faggitt", "faggot", "faggs", "fagot", "fagots", "fags", "fanny", "fannyflaps", "fannyfucker", 
                "fanyy", "fatass", "fcuk", "fcuker", "fcuking", "feck", "fecker", "felching", "fellate", "fellatio", 
                "fingerfuck", "fingerfucked", "fingerfucker", "fingerfuckers", "fingerfucking", "fingerfucks", "fistfuck", 
                "fistfucked", "fistfucker", "fistfuckers", "fistfucking", "fistfuckings", "fistfucks", "flange", "fook", 
                "fooker", "fuck", "fucka", "fucked", "fucker", "fuckers", "fuckhead", "fuckheads", "fuckin", "fucking", 
                "fuckings", "fuckingshitmotherfucker", "fuckme", "fucks", "fuckwhit", "fuckwit", "fudge packer", 
                "fudgepacker", "fuk", "fuker", "fukker", "fukkin", "fuks", "fukwhit", "fukwit", "fux", "fux0r", "f_u_c_k", 
                "gangbang", "gangbanged", "gangbangs", "gaylord", "gaysex", "goatse", "God", "god-dam", "god-damned", 
                "goddamn", "goddamned", "hardcoresex", "hell", "heshe", "hoar", "hoare", "hoer", "homo", "hore", "fick",
                "horniest", "horny", "hotsex", "jack-off", "jackoff", "jerk-off", "jism", "jiz", "jizm", "jizz", 
                "kawk", "knob", "knobead", "knobed", "knobend", "knobhead", "knobjocky", "knobjokey", "kock", "kondum", 
                "kondums", "kum", "kummer", "kumming", "kums", "kunilingus", "l3i+ch", "l3itch", "labia", "lmfao", 
                "lusting", "m0f0", "m0fo", "m45terbate", "ma5terb8", "ma5terbate", "masochist", "master-bate", "useless",
                "masterb8", "masterbat*", "masterbat3", "masterbate", "masterbation", "masterbations", "masturbate", 
                "mo-fo", "mof0", "mofo", "mothafuck", "mothafucka", "mothafuckas", "mothafuckaz", "mothafucked", 
                "mothafucker", "mothafuckers", "mothafuckin", "mothafucking", "mothafuckings", "mothafucks", "mother fucker", 
                "motherfuck", "motherfucked", "motherfucker", "motherfuckers", "motherfuckin", "motherfucking", 
                "motherfuckings", "motherfuckka", "motherfucks", "muff", "mutha", "muthafecker", "muthafuckker", "muther", 
                "mutherfucker", "n1gga", "n1gger", "nazi", "nigg3r", "nigg4h", "nigga", "niggah", "niggas", "niggaz", 
                "nigger", "niggers", "nob", "nob jokey", "nobhead", "nobjocky", "nobjokey", "numbnuts", "nutsack", 
                "orgasim", "orgasims", "orgasm", "orgasms", "p0rn", "pawn", "pecker", "penis", "penisfucker", "phonesex", 
                "phuck", "phuk", "phuked", "phuking", "phukked", "phukking", "phuks", "phuq", "pigfucker", "pimpis", 
                "piss", "pissed", "pisser", "pissers", "pisses", "pissflaps", "pissin", "pissing", "pissoff", "poop", 
                "porn", "porno", "pornography", "pornos", "prick", "pricks", "pron", "pube", "pusse", "pussi", "pussies", 
                "pussy", "pussys", "rectum", "rimjaw", "rimming", "s hit", "s.o.b.", "sadist", "schlong", "screwing", 
                "scroat", "scrote", "scrotum", "semen", "sh!+", "sh!t", "sh1t", "shag", "shagger", "shaggin", "arsch",
                "shagging", "shemale", "shit", "shitdick", "shite", "shited", "shitey", "shitfuck", "shitfull", "shithead", 
                "shiting", "shitings", "shits", "shitted", "shitter", "shitters", "shitting", "shittings", "shitty", 
                "skank", "slut", "sluts", "smegma", "smut", "snatch", "spac", "spunk", "s_h_i_t", "t1tt1e5", "t1tties", 
                "teets", "teez", "testical", "testicle", "tit", "titfuck", "tits", "titt", "ittie5", "ittiefucker", 
                "titties", "tittyfuck", "tittywank", "titwank", "tosser", "turd", "w4t", "twat", "twathead", "twatty", 
                "twunt", "twunter", "v14gra", "v1gra", "vagina", "viagra", "vulva", "w00se", "wang", "wank", "wanker", 
                "wanky", "whoar", "whore", "willies", "willy", "xrated", "xxx", "noob", "nap", "suck", "fuck", "report"
            };
            #endregion
            #region dontflame
            string[] dontFlame = {
                                    "You should not flame sweety",
                                    "Gotcha, next time maybe.",
                                    "Hey! You know why, do you?",
                                    "You want skins do you?"
                                 };
            #endregion
            List<string> muted = new List<string>();
            //ArrayList muted = new ArrayList();
            var wuu = dontFlame[new Random().Next(0, dontFlame.Length)];
            foreach (string item in args.Message.Split(' '))
            {

                if (flame.Any(args.Message.Contains))
                {
                    if (args.Sender.IsMe)
                    {
                        args.Process = false;
                        Notifications.AddNotification(new Notification(wuu, 3500).SetTextColor(Color.OrangeRed).SetBoxColor(Color.Black));
                    }

                    if (!muted.Any(str => str.Contains(args.Sender.Name)) && !args.Sender.IsMe)
                    {
                        Utility.DelayAction.Add(new Random().Next(127, 723), () => Game.Say("/mute " + args.Sender.Name));
                        Notifications.AddNotification(new Notification(args.Sender.Name + " has been muted.", 3500).SetTextColor(Color.OrangeRed).SetBoxColor(Color.Black));
                        muted.Add(args.Sender.Name);
                        Console.WriteLine("[" + timeStamp + "]: " + args.Sender.Name + " has been muted.");
                        Console.WriteLine("[" + timeStamp + "]: Mute list:");
                        foreach (string banned in muted)
                        {
                            Console.WriteLine(banned);
                        }
                    }
                }
                /*
                if (("sorry, sry").Any(args.Message.Contains) && muted.Contains(args.Sender.Name))
                {
                    Utility.DelayAction.Add(new Random().Next(127, 723), () => Game.Say("/mute " + args.Sender.Name));
                    muted.Remove(args.Sender.Name);
                    Console.WriteLine("[" + timeStamp + "]: " + args.Sender.Name + " has been unmuted.");
                    Console.WriteLine("[" + timeStamp + "]: Mute list:" + muted);
                }
                */
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
        
        }
    }
}