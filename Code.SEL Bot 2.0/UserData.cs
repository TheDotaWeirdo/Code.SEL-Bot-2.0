using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Code.SEL_Bot_2._0
{
	public partial class MainForm
	{
		public static UserData[] UserDatas;

		public static void LoadUserData()
		{
			string file = "../../Data/UserData.xml";
			XmlSerializer formatter = new XmlSerializer(new List<UserData>().GetType());
			FileStream DataFile = new FileStream(file, FileMode.Open);
			byte[] buffer = new byte[DataFile.Length];
			DataFile.Read(buffer, 0, (int)DataFile.Length);
			MemoryStream stream = new MemoryStream(buffer);
			UserDatas = ((List<UserData>)formatter.Deserialize(stream)).ToArray();
		}

		public static void SaveUserData()
		{
			string path = "../../Data/UserData.xml";
			FileStream outFile = File.Create(path);
			XmlSerializer formatter = new XmlSerializer(new List<UserData>().GetType());
			formatter.Serialize(outFile, UserDatas.ToList());
		}

		public class UserData
		{
			public ulong UserID = 0;
			public int MessagesSent = 0;
			public int HangStarted = 0;
			public int HangSolved = 0;
			public int CharactersSent = 0;
			public int SongsPlayed = 0;
			public int TimesMoved = 0;
			public int TotalBans = 0;
			public double ExileTime = 0;
			public List<string> Games = new List<string>();
			public List<ulong> BanList = new List<ulong>();
			public DateTime LastOnline;
			public DateTime OnlineSince;
			public DateTime UnbanTime;

			// Preferences
			public bool AutoMove = true;
			public bool AFKMove = true;
			public bool Track = true;

			// Volatile Data
			public ulong CurrentGame = 0;

			public int BanCount { get => BanList.Count; }

			public void ClearVolatiles()
			{
				CurrentGame			 = 0;
				if(!Track)
				{
					MessagesSent	 = 0;
					HangStarted		 = 0;
					HangSolved		 = 0;
					CharactersSent = 0;
					SongsPlayed		 = 0;
					TimesMoved		 = 0;
					ExileTime			 = 0;
					Games					 = new List<string>();
					LastOnline		 = new DateTime();
					OnlineSince		 = new DateTime();
				}
			}

			public override string ToString()
			{
				var U = CodeSelBot.CodeSEL.GetUser(UserID);
				var Out = U.Mention + ((U.Nickname == null) ? "" : "  AKA `" + U.Name + "`")
					+ "\n\n`Discriminator:`     #" + U.Discriminator
					+ "\n`Long ID      :`     " + UserID
					+ "\n`Join Date    :`     " + U.JoinedAt.ToShortDateString()
					+ "\n`Currently    :`     " + ((U.Status.ToString() == "idle") ? "AFK, " : "") + ((U.Status.ToString() == "dnd") ? "Busy, " : "")
					+ ((U.Status.ToString() != "offline") ?
					"Online since " + OnlineSince.AddHours(-2).ToShortTimeString() + " GMT"
					: "Offline" + ((LastOnline.ToString() != "01-Jan-01 12:00:00 AM") ? ", Last Seen on " + LastOnline.AddHours(-2).ToString() : ""))
					+ ((U.Status.ToString() == "online" && U.CurrentGame.HasValue && !U.IsBot) ? "\n`Playing      :`     " + U.CurrentGame.Value.Name : "")
					+ "\n`Msgs Sent    :`     " + MessagesSent
					+ "\n`Chars Sent   :`     " + CharactersSent
					+ ((ExileTime > 0) ? "\n`Exile Time   :`     " + ExileTime : "")
					+ ((BanCount > 0) ? "\n`Ban Votes    :`     " + BanCount : "")
					+ ((TimesMoved > 0) ? "\n`Times Moved  :`     " + TimesMoved : "")
					+ ((!U.IsBot && false) ?
						"\n`Hangs Started:`     " + HangStarted +
						"\n`Hangs Solved :`     " + HangSolved +
						"\n`Songs Played :`     " + SongsPlayed : "")
					+ "\n`AFKmove      :`     " + AFKMove.ToString()
					+ "\n`Automove     :`     " + AutoMove.ToString()
					+ "\n`Allow Track  :`     " + Track.ToString();
				if (Games.Count > 0 && !U.IsBot)
				{
					Out += "\n`Games        :`     ";
					foreach (var g in Games)
						Out += g + ", ";
					Out = Out.Substring(0, Out.Length - 2);
				}
				Out += "\n`Roles        :`     ";
				foreach (var r in U.Roles.Where(x => x != CodeSelBot.CodeSEL.EveryoneRole).OrderBy(o => o.Position))
					Out += r.Mention + "  ";
				return Out;
			}

			public void RemoveShadowBan()
			{
				var U = CodeSelBot.CodeSEL.GetUser(UserID);
				U.Edit(false, false, null, new Role[] { CodeSelBot.CodeSEL.EveryoneRole }, U.Name);
				CodeSelBot.CodeSEL.DefaultChannel.SendMessage(U.Mention + "'s temporary ban has been lifted, ban duration increases after each one");
			}
		}
		
	}
}
