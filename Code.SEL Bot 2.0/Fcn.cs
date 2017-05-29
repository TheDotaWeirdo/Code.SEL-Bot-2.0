using Discord;
using System;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using static Code.SEL_Bot_2._0.MainForm;
using static Code.SEL_Bot_2._0.Dictionaries;
using System.Collections.Generic;

namespace Code.SEL_Bot_2._0
{
	public static class Fcn
	{
		const char PrefixChar = '-';
		const string BotMention = "<@317261382900908033>";

		public static bool Check(this Command C, MessageEventArgs e)
		{
			if (C.MaxLength > 0 && e.Message.RawText.Length > C.MaxLength)
				return false;
			return e.Message.RawText.CommandCheck(C.Triggers, C.StrictStart);
		}

		public static bool CommandCheck(this string Text, string[] Commands, bool StrictStart = true)
		{
			Text = Text.ToLower().RemoveSpaces();

			if (Text[0] == PrefixChar)
				Text = Text.Substring(1);
			else if (Text.Contains(BotMention))
				Text = Text.Remove(Text.IndexOf(BotMention), BotMention.Length);
			else
				return false;

			if (StrictStart)
				return Commands.Any(x => Text.StartsWith(x.ToLower().RemoveSpaces()));
			return Commands.Any(x => Text.Contains(x.ToLower().RemoveSpaces()));
		}

		public static bool CommandCheck(this string Text, string Command, bool StrictStart = true)
		{
			return Text.CommandCheck(new string[] { Command }, StrictStart);
		}

		public static string Decrypt(this Command C, MessageEventArgs e)
		{
			return e.Message.RawText.Decrypt(C.Triggers);
		}

		public static string Decrypt(this string Text, string[] Commands)
		{
			var testText = Text.ToLower();

			if (testText[0] == PrefixChar)
				Text = Text.Substring(1);
			else if (testText.Contains(BotMention))
				Text = Text.Remove(testText.IndexOf(BotMention), BotMention.Length);

			while (Text[0] == ' ')
				Text = Text.Substring(1);

			testText = Text.ToLower();

			foreach (var s in Commands)
				if (testText.Contains(s.ToLower()))
				{ Text = Text.Remove(testText.IndexOf(s.ToLower()), s.Length); break; }

			if(Text.Length > 0)
				while (Text.Length > 0 && Text[0] == ' ')
					Text = Text.Substring(1);

			return Text;
		}

		public static Channel GetChannel(this string S)
		{			
			foreach(var Chan in VChannels)
			{
				if (Chan.Key.Any(x => S.ToLower().Contains(x)))
					return CodeSelBot.discord.GetChannel(Chan.Value);
			}
			return null;
		}

		public static UserData GetByUser(this UserData[] L, User U)
		{
			return L.Where(x => x.UserID == U.Id).FirstOrDefault();
		}

		public static void ShadowBan(this User U)
		{
			var Dat = UserDatas.GetByUser(U);
			Dat.BanList = new List<ulong>();
			Dat.TotalBans++;
			Dat.UnbanTime = DateTime.Now.AddDays(2 * Dat.TotalBans);
			U.Edit(true, true, null, new Role[] { CodeSelBot.CodeSEL.GetRole(Role_Banned) }, U.Name + " - BANNED");
		}

		public static void ClearVolatiles(this UserData[] L)
		{
			foreach (var l in L)
				l.ClearVolatiles();
		}

		public static void WaitForIt(this Message M)
		{
			while(M.Timestamp.ToString() == "01-Jan-01 12:00:00 AM")
			{ System.Threading.Thread.Sleep(100); }
		}

		public static void Timed(this Message M, double Time)
		{
			Timer T = new Timer(Time * 1000);
			T.Start();
			T.Elapsed += async (s, e) =>
			{
				T.Dispose();
				await M.Delete();
			};
		}

		public static void Timed(this Task<Message> M, double Time)
		{
			M.Result.Timed(Time);
		}

		public static string ToReadableString(this TimeSpan T)
		{
			var Out = "";
			if (T.Days > 0)
				Out += T.Days + " Days, ";
			if (T.Hours > 0)
				Out += T.Hours + " Hours, ";
			if (T.Minutes > 0)
				Out += T.Minutes + " Minutes, ";
			if (T.Seconds > 0)
				Out += T.Seconds + " Seconds, ";
			return Out.Substring(0, Out.Length - 2);
		}

		public static int CountLines(this string S)
		{
			var Out = 0;
			for (int i = 0; i < S.Length; i++)
				if (S[i] == '\n')
					Out++;
			return Out;
		}

		public static string RemoveLine(this string S, int Lines = 1)
		{
			for (int i = 0; i < Lines; i++)
				if (S.Contains('\n'))
					S = S.Substring(S.IndexOf('\n') + 1);
			return S;
		}

		public static string RemoveSpaces(this string S)
		{
			var s = "";
			foreach (var c in S)
				if (!char.IsWhiteSpace(c))
					s += c;
			return s;
		}

		public static string RemoveMentions(this string S)
		{
			while(S.Contains('<') && S.Contains('>'))
			{
				S = S.Remove(S.IndexOf('<'), S.IndexOf('>') - S.IndexOf('<') + 1);
			}
			return S;
		}

		public static string Random(this string[] S)
		{
			return S[new Random(Guid.NewGuid().GetHashCode()).Next(S.Length)];
		}

		public static string ToEmoji(this string S)
		{
			var Out = "";
			foreach (var c in S.ToLower())
				if (Emoji.ContainsKey(c))
					Out += Emoji[c];
			return Out;
		}

		public static string ToEmoji(this int i)
		{
			return i.ToString().ToEmoji();
		}

		public static string ToEmoji(this double d)
		{
			return d.ToString().ToEmoji();
		}

		public static string ToEmoji(this char c)
		{
			c = c.ToString().ToLower()[0];
			return (Emoji.ContainsKey(c)) ? Emoji[c] : "";
		}

		public static string Between(this string S, string Start, string End)
		{
			if (S.Contains(Start) && S.Contains(End))
				return S.Substring(S.IndexOf(Start) + Start.Length, S.IndexOf(End) - (S.IndexOf(Start) + Start.Length));
			return S;
		}

		public static string Between(this string S, char Start, string End)
		{
			if (S.Contains(Start) && S.Contains(End))
				return S.Substring(S.IndexOf(Start) + 1, S.IndexOf(End) - (S.IndexOf(Start) + 1));
			return S;
		}

		public static string Between(this string S, string Start, char End)
		{
			if (S.Contains(Start) && S.Contains(End))
				return S.Substring(S.IndexOf(Start) + Start.Length, S.IndexOf(End) - (S.IndexOf(Start) + Start.Length));
			return S;
		}

		public static string Between(this string S, char Start, char End)
		{
			if (S.Contains(Start) && S.Contains(End))
				return S.Substring(S.IndexOf(Start) + 1, S.IndexOf(End) - (S.IndexOf(Start) + 1));
			return S;
		}
	}

	public partial class MainForm
	{
		public static void WriteLine(string msg)
		{
			if(!Console.IsDisposed)
				Console.Invoke(CUpdte, new object[] { msg + "\n" });
		}

		public static void Write(string msg)
		{
			Console.Invoke(CUpdte, new object[] { msg });
		}

		private static void WriteConsole(string S)
		{
			Console.Text += S;
			if (Console.Text.CountLines() > Console.Height / 11.5)
				Console.Text = Console.Text.RemoveLine(Console.Text.CountLines() - (int)(Console.Height / 11.5));
		}
	}
}
