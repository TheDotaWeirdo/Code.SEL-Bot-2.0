using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.API;
using Discord.Net;
using System.Net;
using System.Timers;

namespace Code.SEL_Bot_2._0
{
	partial class MainForm
	{
		public partial class CodeSelBot
		{
			public static DiscordClient discord;
			public static Server CodeSEL;
			public List<Exile> Exiles = new List<Exile>();

			public CodeSelBot()
			{
				GenerateCommands();

				discord = new DiscordClient(x =>
				{
					x.LogLevel = LogSeverity.Info; ;
					x.LogHandler = Log;
				});

				discord.MessageReceived += OnMessage;
				discord.UserJoined += OnUserJoined;
				discord.ServerAvailable += OnServerAvailable;
				discord.ServerUnavailable += OnServerUnavailable;
				discord.Ready += OnReady;

				var T = new Timer(1000);
				T.Start();
				T.Elapsed += TimeLooper;

				Connect();
			}

			public static void Connect()
			{
				try
				{
					discord.ExecuteAndWait(async () =>
					{
						await discord.Connect("MzE3MjYxMzgyOTAwOTA4MDMz.DAhQPg.PB7pRRj_fjAapHRKCe_bbgBJ4Wg", TokenType.Bot);
					});
				}
				catch (WebException) { WriteLine("Network Error - could not connect to the Internet"); }
				catch (Exception e) { WriteLine("Error: " + e.Message); }
			}

			private void TimeLooper(object sender, ElapsedEventArgs e)
			{
				if (CodeSEL == null || ConnectionState != "Connected") return;

				BotData.UpTime += TimeSpan.TicksPerSecond;

				foreach(var U in CodeSEL.Users)
				{
					if (U.Status != "offline")
					{
						UserDatas.GetByUser(U).LastOnline = DateTime.Now;
						if (UserDatas.GetByUser(U).OnlineSince.ToString() == "01-Jan-01 12:00:00 AM")
							UserDatas.GetByUser(U).OnlineSince = DateTime.Now;
					}
					else
						UserDatas.GetByUser(U).OnlineSince = DateTime.Parse("01-Jan-01 12:00:00 AM");

					if (U.CurrentGame.HasValue && !UserDatas.GetByUser(U).Games.Contains(U.CurrentGame.Value.Name))
						UserDatas.GetByUser(U).Games.Add(U.CurrentGame.Value.Name);

				}

				foreach (var VChannel in CodeSEL.VoiceChannels.Where(c => c.Users.Count() > 0))
				{
					var counter = new Dictionary<ulong, int>();
					foreach (var v in Dictionaries.VChannels.Values)
						counter.Add(v, 0);

					foreach (var u in VChannel.Users)
						if (u.CurrentGame.HasValue && u.CurrentGame.Value.Name.GetChannel() != null)
							counter[u.CurrentGame.Value.Name.GetChannel().Id]++;

					var TargetChannel = CodeSEL.GetChannel(counter.Where(y => y.Value == counter.Values.Max()).FirstOrDefault().Key);
					if (counter.Values.Max() >= Math.Max(1, VChannel.Users.Count() / 2)
						&& TargetChannel != VChannel && VChannel.Users.All(x => UserDatas.GetByUser(x).CurrentGame != TargetChannel.Id))
					{
						foreach (var u in VChannel.Users)
						{
							if ((UserDatas.GetByUser(u).AutoMove)
								&& (u.VoiceChannel != CodeSEL.AFKChannel || UserDatas.GetByUser(u).AFKMove))
							{
								if (!(VChannel.Id == CodeSEL.AFKChannel.Id && (!u.CurrentGame.HasValue || u.CurrentGame.Value.Name.GetChannel().Id != TargetChannel.Id)))
								{
									u.Edit(null, null, TargetChannel);
									UserDatas.GetByUser(u).CurrentGame = TargetChannel.Id;
									UserDatas.GetByUser(u).TimesMoved++;
								}
							}
							else if (VChannel.Users.Count() > 1)
								(CodeSEL.DefaultChannel.SendMessage(u.Mention + ", you have not been moved automatically due to your **AutoMove** preferences")).Timed(100);
						}

					}
					else if ((VChannel.Id != Dictionaries.Ch_V_Main 
						&& VChannel.Id != Dictionaries.Ch_V_AFK 
						&& VChannel.Id != Dictionaries.Ch_V_Music)
						&& VChannel.Users.Count() > 0 
						&& VChannel.Users.All(u => !u.CurrentGame.HasValue 
						|| (u.CurrentGame.Value.Name.GetChannel() != null 
						&& u.CurrentGame.Value.Name.GetChannel().Id != u.VoiceChannel.Id)))
					{
						foreach (var u in VChannel.Users)
						{
							if (UserDatas.GetByUser(u).AutoMove)
							{
								u.Edit(null, null, CodeSEL.GetChannel(Dictionaries.Ch_V_Main));
								UserDatas.GetByUser(u).CurrentGame = 0;
								UserDatas.GetByUser(u).TimesMoved++;
							}
						}
					}
				}
			}

			private void OnReady(object sender, EventArgs e)
			{
				// Unbanning
				foreach (var Dat in UserDatas)
					if (Dat.TotalBans > 0 && Dat.UnbanTime.ToString() != "01-Jan-01 12:00:00 AM"
						&& CodeSEL.GetUser(Dat.UserID).HasRole(CodeSEL.GetRole(Dictionaries.Role_Banned))
						&& Dat.UnbanTime.CompareTo(DateTime.Now) > 0)
						Dat.RemoveShadowBan();
			}

			private void OnServerUnavailable(object sender, ServerEventArgs e)
			{

			}

			private void OnServerAvailable(object sender, ServerEventArgs e)
			{
				CodeSEL = discord.Servers.FirstOrDefault();
			}

			private async void OnUserJoined(object sender, UserEventArgs e)
			{
				var Msg = "Hello, " + e.User.Mention + ", Welcome to Code.SEL <:codesel:249266261928706048>\n" +
					"Mention any of the following Roles *(@role)* based on the games you play:\n\n";
				foreach(var R in Dictionaries.Roles.Where(x => CodeSEL.GetRole(x).Name.Contains("Buddy")))
					Msg += CodeSEL.GetRole(R).Mention + "\n";
				await CodeSEL.DefaultChannel.SendMessage(Msg);
				var temp = UserDatas.ToList();
				temp.Add(new UserData() { UserID = e.User.Id });
				UserDatas = temp.ToArray();
			}

			private async void OnMessage(object sender, MessageEventArgs e)
			{
				if(e.Message.RawText != "")
					foreach (var C in Commands)
					{
						if ((C.AcceptBotMsgs || !e.User.IsBot) && C.Check(e))
						{
							if (C.Permission.Count == 0 || (e.User.Roles.Any(x => C.Permission.Any(y => y == x.Id))))
							{
								BotData.CommandsProccessed++;
								if (!C.AllowPrivateChannels && e.Channel.IsPrivate)
									await e.Channel.SendMessage($"The **{C.Name}** command is not available in private chat");
								else
								{
									if (C.DeleteMsg)
										e.Message.Timed(5);
									WriteLine(C.Name + " Command found in | " + e.Message);
									await C.Action(e.Message.RawText.Decrypt(C.Triggers), e);
									if (C.Breaks)
										break;
								}
							}
							else
								(await e.Channel.SendMessage("You do not have the correct permissions to use this command")).Timed(60);
						}

						// In case of role selection
						if(e.User.Roles.Count() == 1 && e.Message.MentionedRoles != null && e.Message.MentionedRoles.Count() > 0)
						{
							var Msg   = e.User.Mention + ", you've been given the ";
							var Roles = e.Message.MentionedRoles.Where(x => x.Name.Contains("Buddy"));
							foreach(var R in Roles)
								Msg += R.Mention + "  ";
							await e.Channel.SendMessage(Msg);
							await e.User.Edit(null, null, null, Roles);
						}
					}
				UserDatas.GetByUser(e.User).MessagesSent++;
				UserDatas.GetByUser(e.User).CharactersSent += e.Message.RawText.Length;
				if (Listening)
					if (e.Message.RawText != "")
						WriteLine($"#{e.Channel.Name} {e.User.Name}: {e.Message.RawText}");
					else
						WriteLine($"#{e.Channel.Name} {e.User.Name}: File Sent");
			}

			private void Log(object sender, LogMessageEventArgs e)
			{
				WriteLine("Log: " + e.Message);
			}
		}
	}
}
