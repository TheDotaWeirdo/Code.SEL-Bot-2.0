using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace Code.SEL_Bot_2._0
{
	public class Dictionaries
	{
		public static ulong Server_ID = 155958788447404032;
		public static ulong Bot_ID = 317261382900908033;
		public static ulong Role_ServerOwner = 218442977121402891;
		public static ulong Role_ServerBot = 276033405089480714;
		public static ulong Role_Banned = 318135042415001600;
		public static ulong Role_Exiled = 279279809237090304;
		public static ulong Role_HOE = 217651275083415562;
		public static ulong Role_CodeSEL = 217294542020411392;
		public static ulong Role_BOTS = 272385653420064768;
		public static ulong Role_Dota_Buddy = 217704302892810250;
		public static ulong Role_RL_Buddy = 279698883456794624;
		public static ulong Role_CSGO_Buddy = 265924787728023553;
		public static ulong Role_GTA_Buddy = 248905559690969102;
		public static ulong Role_Overwatch_Buddy = 248905441185103873;
		public static ulong Ch_T_Main = 155958788447404032;
		public static ulong Ch_T_Announcements = 155958788447404032;
		public static ulong Ch_T_Info = 227865516260196352;
		public static ulong Ch_T_BotTesting = 272501450809278474;
		public static ulong Ch_V_AFK = 155958790124994560;
		public static ulong Ch_V_Main = 235871481307987968;
		public static ulong Ch_V_Music = 272736534418161664;
		public static ulong Ch_V_Dota2 = 155960591083634688;
		public static ulong Ch_V_RL = 218475874398371841;
		public static ulong Ch_V_CSGO = 249452569880035329;
		public static ulong Ch_V_GTA = 216988791909384194;
		public static ulong Ch_V_Overwatch = 235865341782261760;

		public static List<ulong> Roles = new List<ulong>
		{
			Role_BOTS,
			Role_CodeSEL,
			Role_CSGO_Buddy,
			Role_Dota_Buddy,
			Role_Banned,
			Role_Exiled,
			Role_GTA_Buddy,
			Role_HOE,
			Role_Overwatch_Buddy,
			Role_RL_Buddy,
			Role_ServerBot,
			Role_ServerOwner
		};

		public static Dictionary<string[], ulong> VChannels = new Dictionary<string[], ulong>
		{
			{ new string[] { "afk" }, Ch_V_AFK },
			{ new string[] { "main", "home" }, Ch_V_Main },
			{ new string[] { "music", "song" }, Ch_V_Music },
			{ new string[] { "dota", "dots", "doto" }, Ch_V_Dota2 },
			{ new string[] { "gta", "grand theft auto" }, Ch_V_GTA },
			{ new string[] { "cs", "csgo", "counter strike" }, Ch_V_CSGO },
			{ new string[] { "rl", "rocket league" }, Ch_V_RL },
			{ new string[] { "overwatch" }, Ch_V_Overwatch }
		};

		public static Dictionary<char, string> Emoji = new Dictionary<char, string>
		{
			{ ' ', "  " },
			{ '1', ":one:" },
			{ '2', ":two:" },
			{ '3', ":three:" },
			{ '4', ":four:" },
			{ '5', ":five:" },
			{ '6', ":six:" },
			{ '7', ":seven:" },
			{ '8', ":eight:" },
			{ '9', ":nine:" },
			{ '0', ":zero:" },
			{ '#', ":hash:" },
			{ '*', ":asterisk:" },
			{ 'a', "<:H_A:285782726274056192>"},
			{ 'b', "<:H_B:285782726706069504>"},
			{ 'c', "<:H_C:285782726983024641>"},
			{ 'd', "<:H_D:285782729734488065>"},
			{ 'e', "<:H_E:285782730141204482>"},
			{ 'f', "<:H_F:285782730573086720>"},
			{ 'g', "<:H_G:285782731021877248>"},
			{ 'h', "<:H_H:285782730699046913>"},
			{ 'i', "<:H_I:285782730992779264>"},
			{ 'j', "<:H_J:285782731298832393>"},
			{ 'k', "<:H_K:285782731508678657>"},
			{ 'l', "<:H_L:285782731919589376>"},
			{ 'm', "<:H_M:285783607711367168>"},
			{ 'n', "<:H_N:285783607707172865>"},
			{ 'o', "<:H_O:285783610542522368>"},
			{ 'p', "<:H_P:285783610362167296>"},
			{ 'q', "<:H_Q:285783610949369856>"},
			{ 'r', "<:H_R:285783610995507201>"},
			{ 's', "<:H_S:285783612983476226>"},
			{ 't', "<:H_T:285783613004316673>"},
			{ 'u', "<:H_U:285783613126213634>"},
			{ 'v', "<:H_V:285783613818011649>"},
			{ 'w', "<:H_W:285783615013519360>"},
			{ 'x', "<:H_X:285783617949401089>"},
			{ 'y', "<:H_Y:285783618285076480>"},
			{ 'z', "<:H_Z:285783618792456192>"},
		};
	}
}
