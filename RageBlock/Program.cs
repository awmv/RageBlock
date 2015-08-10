using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using System.IO;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using System.Collections;
using System.Text.RegularExpressions;

namespace RageBlock
{
    class Program
    {        
        static Menu M;
        static List<string> muted = new List<string>();
        static String timeStamp = GetTimestamp(DateTime.Now);
        static string[] flame;
        static string[] neverFlame;

        static void Main(string[] args)
        {
            #region Flame
            flame = new string[] {
"bronze", "silver", "gold", "platinum", "plat", "diamond", "master", "challenger", "feed", "retard", "ks",
"killsteal", "on purpose", "camped", "stupid", "anal", "anus", "arrse", "arse", "ass-fucker", "assfucker",
"assfukka", "asshole", "assholes", "asswhole", "mutter", "mother", "b!tch", "b17ch", "b1tch", "ballbag",
"ballsack", "bastard", "beastial", "beastiality", "scrub", "bellend", "bestial", "bestiality", "bi+ch",
"biatch", "bitch", "bitcher", "bitchers", "bitches", "bitchin", "bitching", "bloody", "blow job", "blowjob",
"blowjobs", "boiolas", "bollock", "bollok", "boner", "buceta", "bugger", "bum", "bunny fucker", "hure",
"lappen", "dumm", "dämlich", "missgeburt", "butthole", "buttmuch", "buttplug", "c0ck", "c0cksucker", "stfu",
"carpet muncher", "cawk", "chink", "cipa", "cl1t", "clit", "clitoris", "clits", "cnut", "cock", "cock-sucker",
"cockface", "cockhead", "cockmunch", "cockmuncher", "cocks", "cocksuck", "cocksucked", "cocksucker", "smurf",
"cocksucking", "cocksucks", "cocksuka", "cocksukka", "cok", "cokmuncher", "coksucka", "coon", "cox", "crap",
"cum", "cummer", "cumming", "cums", "cumshot", "cunilingus", "cunillingus", "cunnilingus", "cunt", "cuntlick",
"cuntlicker", "cuntlicking", "cunts", "cyalis", "cyberfuc", "cyberfuck", "cyberfucked", "cyberfucker",
"cyberfuckers", "cyberfucking", "d1ck", "damn", "dick", "dickhead", "dildo", "dildos", "dink", "dinks", "dirsa",
"dlck", "dog-fucker", "doggin", "dogging", "donkeyribber", "doosh", "duche", "dyke", "ejaculate", "ejaculated",
"ejaculates", "ejaculating", "ejaculatings", "ejaculation", "ejakulate", "f u c k", "f u c k e r", "f4nny", "fag",
"fagging", "faggitt", "faggot", "faggs", "fagot", "fagots", "fags", "fanny", "fannyflaps", "fannyfucker", "fanyy",
"fatass", "fcuk", "fcuker", "fcuking", "feck", "fecker", "felching", "fellate", "fellatio", "fingerfuck", "fu",
"fingerfucked", "fingerfucker", "fingerfuckers", "fingerfucking", "fingerfucks", "fistfuck", "fistfucked",
"fistfucker", "fistfuckers", "fistfucking", "fistfuckings", "fistfucks", "flange", "fook", "fooker", "fuck",
"fucka", "fucked", "fucker", "fuckers", "fuckhead", "fuckheads", "fuckin", "fucking", "fuckings", "script",
"fuckingshitmotherfucker", "fuckme", "fucks", "fuckwhit", "fuckwit", "fudge packer", "ass", "fudgepacker",
"fuk", "fuker", "fukker", "fukkin", "fuks", "fukwhit", "fukwit", "fux", "fux0r", "f_u_c_k", "gangbang", "scripts",
"gangbanged", "gangbangs", "gaylord", "gaysex", "goatse", "God", "god-dam", "god-damned", "goddamn", "goddamned",
"hardcoresex", "hell", "heshe", "hoar", "hoare", "hoer", "homo", "hore", "fick", "horniest", "horny", "hotsex",
"jack-off", "jackoff", "jerk-off", "jism", "jiz", "jizm", "jizz", "kawk", "knob", "knobead", "knobed", "knobend",
"knobhead", "knobjocky", "knobjokey", "kock", "kondum", "kondums", "kum", "kummer", "kumming", "kums",
"kunilingus", "l3i+ch", "l3itch", "labia", "lmfao", "lusting", "m0f0", "m0fo", "m45terbate", "ma5terb8",
"ma5terbate", "masochist", "master-bate", "useless", "masterb8", "masterbat*", "masterbat3", "masterbate",
"masterbation", "masterbations", "masturbate", "mo-fo", "mof0", "mofo", "mothafuck", "mothafucka", "mothafuckas",
"mothafuckaz", "mothafucked", "mothafucker", "mothafuckers", "mothafuckin", "mothafucking", "mothafuckings",
"mothafucks", "mother fucker", "motherfuck", "motherfucked", "motherfucker", "motherfuckers", "motherfuckin",
"motherfucking", "stupid", "motherfuckings", "motherfuckka", "motherfucks", "muff", "mutha", "muthafecker",
"muthafuckker", "muther", "mutherfucker", "n1gga", "n1gger", "nazi", "nigg3r", "nigg4h", "nigga", "niggah",
"niggas", "niggaz", "nigger", "niggers", "nob", "nob jokey", "nobhead", "nobjocky", "nobjokey", "numbnuts",
"nutsack", "orgasim", "orgasims", "orgasm", "orgasms", "p0rn", "pawn", "pecker", "penis", "penisfucker",
"phonesex", "phuck", "phuk", "phuked", "phuking", "phukked", "phukking", "phuks", "phuq", "pigfucker", "pimpis",
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
            #region Jokes
            neverFlame = new string[] {
//"You should not flame sweety.", "Gotcha, next time maybe.", "Stop it.", "You want skins do you?",
"Shacos darkest secret is that he's afraid of children.", "Yorick walked into a bar. There was no counter.",
"Once Kog'maw had a cold, but no one could tell.", "For some reason Alistar suffers from headaches.",
"Taric's employer thinks he's a real gem.", "Sheep are suspicious of the newcomer: Urf the manatee",
"Trundle never ceases to be amazed about how everyone on the internet knows he's a troll.",
"Mordekaiser really hates airport security.", "Ryze's big secret is a real shocker!",
"Why Mundo can't be jailed for sexual assault? Mundo pleases who ever he pleases!",
"Why didn't Sivir win the spelling bee? Because she could only spell shield",
"Why did Fizz fall off of his trident? Because he's incredibly unbalanced.".,
"Also, Evelynn is constantly being bothered by an old man named Gargamel.",
"Cassiopeia always knows how much she weighs thanks to all her scales.",
"You know Zilean is having a bad day when he asks you for the time.",
"Sometimes Karthus picks up a sword and pretends that he's Arthas.",
"Tristana is disappointed that it's never a rocket in your pocket."
            };
            #endregion
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad; 
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            #region Menu
            var r = "RageBlock";
            M = new Menu(r, r, true);
            M.AddItem(new MenuItem("Status", "Enable").SetValue(true));
            M.AddToMainMenu();
            #endregion
            Game.OnChat += Game_OnChat;
            Game.OnInput += Game_OnInput;
        }

        private static String GetTimestamp(DateTime value) { return value.ToString("HH:mm"); }

        private static void Log(string Value) { 
            Game.PrintChat("[" + timeStamp + "] <font color='#eb7577'>RageBlock</font>: " + Value);
        }

        private static void Game_OnChat(GameChatEventArgs args)
        {            
            if (!M.Item("Status").GetValue<bool>()) return;
            if (!args.Sender.IsMe)
            {
                Regex regex = new Regex(string.Join("|\\b", flame), RegexOptions.IgnoreCase);
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
        }

        private static void Game_OnInput(GameInputEventArgs args)
        {
            if (!M.Item("Status").GetValue<bool>()) return;
            Regex regex = new Regex(string.Join("|\\b", flame), RegexOptions.IgnoreCase);
            Match match = regex.Match(args.Input);
            if (match.Success)
            {
                args.Process = false;
                Log(neverFlame[new Random().Next(0, neverFlame.Length)]);
            }
        }
    }
}