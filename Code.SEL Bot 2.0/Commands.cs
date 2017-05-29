using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Timers;

namespace Code.SEL_Bot_2._0
{

	partial class MainForm
	{
		public delegate Task CommandResponce(string Msg, MessageEventArgs e);

		public class Command
		{
			public string Name = "Command";
			public string Description = "This Command has no description yet.";
			public string[] Triggers;
			public int Priority = 0;
			public int MaxLength = 0;
			public bool StrictStart = true;
			public bool Breaks = true;
			public bool DeleteMsg = false;
			public bool AcceptBotMsgs = false;
			public bool AllowPrivateChannels = true;
			public List<ulong> Permission = new List<ulong>();
			public CommandResponce Action;

			public override string ToString()
			{
				var res = "Info about the **" + Name + "** Command:\n\n";
				res += "**Triggers:** ";
				foreach (var s in Triggers)
					res += s + " - ";
				res = res.Substring(0, res.Length - 3) + "\n";
				res += ((StrictStart) ? "**Requires**" : "Does **not** require") 
					+ ((Triggers.Count() > 0) ? " one of the triggers " : " the trigger ") 
					+ "at the start\n";
				if (MaxLength > 0)
					res += "**Max Length:** " + MaxLength + "\n";
				if (Permission.Count > 0)
				{
					res += "**Permissions:** ";
					foreach (var r in Permission)
						res += CodeSelBot.CodeSEL.GetRole(r).Mention + " ";
					res += "\n";
				}
				if(Description.StartsWith("**Syntax:**"))
					res += Description;
				else
					res += "**Description:**  " + Description;
				return res;
			}
		}

		partial class CodeSelBot
		{
			List<Command> Commands;

			private void GenerateCommands()
			{
				Commands = new List<Command>
				{
					new Command()
					{
						Name = "Echo",
						Priority = 10,
						DeleteMsg = true,
						Triggers = new string[] { "say", "echo" },
						Action = Responce_Say,
						Description = "Makes the Bot repeat whatever is after the `trigger` exactly."
					},
					new Command()
					{
						Name = "Hello",
						Priority = 20,
						StrictStart = false,
						Triggers = new string[] { "hello", "hi", "sup", "hey", "yo" },
						Action = Responce_Hello,
						Description = "Triggers a greeting by the Bot."
					},
					new Command()
					{
						Name = "To Emoji",
						Priority = 9,
						DeleteMsg = true,
						Triggers = new string[] { "!e", "toemoji", "emoji" },
						Action = Responce_ToEmoji,
						Description = "Echoes the message converted into emojis."
					},
					new Command()
					{
						Name = "Create Invite",
						Priority = 2,
						DeleteMsg = true,
						AllowPrivateChannels = false,
						Triggers = new string[] { "invite", "createinvite" },
						Action = Responce_Invite,
						Description = "Creates a new invite to the Server."
					},
					new Command()
					{
						Name = "Move To",
						Priority = 3,
						DeleteMsg = true,
						AllowPrivateChannels = false,
						Triggers = new string[] { "move" },
						Action = Responce_MoveTo,
						Description = "**Syntax:** `-move <from> to <to> | -move to <to>`\n**Description:** `<from>` and `<to>` refer a voice channel's name. The Bot will move the specified `<from>` users to the `<to>` channel."
					},
					new Command()
					{
						Name = "Cleanup",
						Priority = 1,
						AllowPrivateChannels = false,
						Triggers = new string[] { "clean", "clear" },
						Action = Responce_Clean,
						Description = "**Syntax:** `-clean X #Time | -clean #Time | -clean X`\n**Description:** Deletes the `X` most recent messages within `Time` hours, mentioning a user will only delete his messages."
					},
					new Command()
					{
						Name = "Fuck",
						Priority = 6,
						StrictStart = false,
						AcceptBotMsgs = true,
						Triggers = new string[] { "fuck", "damn", "fok" },
						Action = Responce_Fuck,
						Description = "Sends back salty notes about you or whatever you want to fuck."
					},
					new Command()
					{
						Name = "Help",
						Priority = 0,
						Triggers = new string[] { "help" },
						Action = Responce_Help,
						Description = "Gives information about other commands in the Server."
					},
					new Command()
					{
						Name = "Exile",
						Priority = 4,
						DeleteMsg = true,
						AllowPrivateChannels = false,
						Triggers = new string[] { "exile", "shadow ban", "restrain" },
						Action = Responce_Exile,
						Description = "**Syntax:** `-exile #mm:ss @Target | -exile @Target`\n**Description:** Exiles the `Target` user for the specified time, if none was the default is 5 mins\nExiled Users can not interact by voice or move through channels but can still send messages.",
						Permission = new List<ulong>{ Dictionaries.Role_ServerOwner, Dictionaries.Role_CodeSEL }
					},
					new Command()
					{
						Name = "Unexile",
						Priority = 2,
						DeleteMsg = true,
						AllowPrivateChannels = false,
						Triggers = new string[] { "unexile me", "free me" },
						Action = Responce_Unexile,
						Description = "Reduces the total exile time you have, this can only used once per exile."
					},
					new Command()
					{
						Name = "Spam",
						Priority = 4,
						AllowPrivateChannels = false,
						StrictStart = false,
						Triggers = new string[] { "spam", "annoy" },
						Action = Responce_Spam,
						Description = "Spams a target user with random DMs."
					},
					new Command()
					{
						Name = "Info",
						Priority = 1,
						AllowPrivateChannels = false,
						Triggers = new string[] { "info", "about" },
						Action = Responce_Info
					},
					new Command()
					{
						Name = "Preference",
						Priority = 7,
						DeleteMsg = true,
						Breaks = false,
						AllowPrivateChannels = false,
						Triggers = new string[] { "preference", "pref" },
						Action = Responce_Pref,
						Description = "**Syntax:** `-pref automove | afkmove | track`\n**Description:** Toggles a specific preference." +
												"\n*Automove:* Toggles the Bot from moving you between channels at all" +
												"\n*AFKmove:* Toggles the Bot from moving you from the AFK channel" +
												"\n*Track:* Toggles tracking of various data, the current session's data will still be tracked but will not be saved"
					},
					new Command()
					{
						Name = "Encrypt",
						Priority = 7,
						DeleteMsg = true,
						Triggers = new string[] { "encrypt" },
						Action = Responce_Encrypt,
						Description = "**Syntax:** `-encrypt <text> #Key`\n**Key Syntax:** `#[a-z][1-9][2-9][3-9][1-3]`\n**Description:** Turns input text into complicated text, the same text with a specific key can only result in one specific output which can not be reverted to the original text."
					},
					new Command()
					{
						Name = "Ban",
						Priority = 4,
						DeleteMsg = true,
						AllowPrivateChannels = false,
						Triggers = new string[] { "ban" },
						Action = Responce_Ban,
						Description = "Votes to ban the users you mention, a total of 3 votes are needed to ban a user",
						Permission = new List<ulong>{ Dictionaries.Role_ServerOwner, Dictionaries.Role_CodeSEL }
					},
					new Command()
					{
						Name = "UnBan",
						Priority = 3,
						DeleteMsg = true,
						AllowPrivateChannels = false,
						Triggers = new string[] { "unban" },
						Action = Responce_Unban,
						Description = "Removes your ban vote against the users you mention, a total of 3 votes are needed to ban a user",
						Permission = new List<ulong>{ Dictionaries.Role_ServerOwner, Dictionaries.Role_CodeSEL }
					},
					new Command()
					{
						Name = "Connect 4",
						Priority = 4,
						Triggers = new string[] { "connect 4", "connect4" },
						Action = Responce_Connect4,
						Description = "**Syntax:** `-connect4 <Opposing Bot> #Difficulty | <Opposing Player>`\n**Description:** Starts a Connect 4 game against the mentioned player, you can adjust the difficulty of the AI (0 - 100), You can only participate in one game at a time"
					},
					new Command()
					{
						Name = "Connect 4 - Play",
						Priority = 4,
						MaxLength = 3,
						DeleteMsg = true,
						Triggers = new string[] { "c1", "c2", "c3", "c4", "c5", "c6", "c7"},
						Action = Responce_Connect4_Play,
						Description = "If you are in a Connect 4 Game, triggering this command will register your move in the column number specified"
					},
					new Command()
					{
						Name = "Server Info",
						Priority = 0,
						AllowPrivateChannels = false,
						Triggers = new string[] { "serverinfo", "info server"},
						Action = Responce_ServerInfo,
						Description = "If you are in a Connect 4 Game, triggering this command will register your move in the column number specified"
					}
				};
				Commands = Commands.OrderBy(o => o.Priority).ToList();
			}

			async Task Responce_Say(string Msg, MessageEventArgs e)
			{
				await e.Channel.SendMessage(Msg);
				WriteLine("Command: Responded an echo of " + e.User.Name + "'s message"); 
			}

			async Task Responce_Hello(string Msg, MessageEventArgs e)
			{
				var Responces = new string[]
				{
					"Hey!",
					"Hello there!",
					"Sup.",
					"Hey, " + e.User.Name,
					"Hello Mr. " + e.User.Name,
					"Kon'nichiwa",
					"Ahoy there!",
					"Hai",
					"Herroww",
					"Greetings",
					"Sup :call_me:",
					"Wazzuuuppp"
				};
				await e.Channel.SendMessage(Responces.Random());
				WriteLine("Command: Greeted " + e.User.Name);
			}

			async Task Responce_ToEmoji(string Msg, MessageEventArgs e)
			{
				await e.Channel.SendMessage(Msg.ToEmoji());
				WriteLine("Command: Converted " + e.User.Name + "'s message to emoji");
			}

			async Task Responce_Invite(string Msg, MessageEventArgs e)
			{
				await e.Channel.SendIsTyping();
				Invite Inv = await CodeSEL.CreateInvite();
				await e.Channel.SendMessage("Here's an invite for the Server: `https://discord.gg/" + Inv.Code + "`");
				WriteLine("Command: Created new Server Invite: https://discord.gg/" + Inv.Code);
			}

			async Task Responce_MoveTo(string Msg, MessageEventArgs e)
			{
				Channel ToC, FromC;
				await e.Channel.SendIsTyping();
				if (e.User.ServerPermissions.MoveMembers)
				{
					if (Msg.Contains("to "))
					{
						ToC = Msg.Substring(Msg.IndexOf("to ")).GetChannel();
						if (ToC == null)
						{
							(await e.Channel.SendMessage(":anger: Target channel could not be found;\n`Syntax Error`")).Timed(60);
							WriteLine("Command Error: Could not find target channel in: " + Msg);
							return;
						}
						IEnumerable<User> UsersToMove;
						FromC = Msg.Substring(0, Msg.IndexOf("to ")).GetChannel();
						if (FromC != null)
						{
							UsersToMove = FromC.Users;
							(await e.Channel.SendMessage(":arrow_right: Moving users from " + FromC.Name + " to " + ToC.Name)).Timed(30);
							WriteLine("Command: Moved users from " + FromC.Name + " to " + ToC.Name);
						}
						else
						{
							UsersToMove = CodeSEL.Users.Where(x => !CodeSEL.AFKChannel.Users.Contains(x));
							(await e.Channel.SendMessage(":arrow_right: Moving connected users to " + ToC.Name)).Timed(30);
							WriteLine("Command: Moved users to " + ToC.Name);
						}
						foreach (User u in UsersToMove)
						{
							if (u.VoiceChannel != null)
								await u.Edit(null, null, ToC);
						}
					}
					else
					{
						(await e.Channel.SendMessage(":anger: " + e.User.Mention + ", your message does not match the command syntax; `-move 'FromChannel' to 'ToChannel'`\n`Syntax Error`")).Timed(60);
						WriteLine("Command Error: Message syntax was wrong");
					}
				}
				else
				{
					(await e.Channel.SendMessage(":anger: " + e.User.Mention + ", you do not have permissions to move members in the Server")).Timed(30);
					WriteLine("Command: User does not have permissions to move other members");
				}
			}

			async Task Responce_Clean(string Msg, MessageEventArgs e)
			{
				await e.Channel.SendIsTyping();
				var MsgCount = 1;
				var Time = -1;
				var User = e.Message.MentionedUsers.Where(x => !x.IsBot).FirstOrDefault();
				try
				{
					if (Msg[0] != '#' || User != null)
					{
						if (Msg.Contains('#'))
							MsgCount += int.Parse(Msg.Substring(0, Msg.IndexOf('#')).RemoveSpaces());
						else if (Msg.Contains(' '))
							MsgCount += int.Parse(Msg.Substring(0, Msg.IndexOf(' ')).RemoveSpaces());
						else
							MsgCount += int.Parse(Msg);
					}
					else if (User != null)
						MsgCount = 25;
					else
						MsgCount = 99;
				}
				catch (Exception) { (await e.Channel.SendMessage(":anger: could not determine message count\n`Syntax Error`")).Timed(30); WriteLine("Command Error: Could not determine message count"); return; }
				try
				{
					if (Msg.Contains('#'))
					{
						if (Msg.Contains("#today"))
							Time = DateTime.Now.Hour;
						else
							Time = int.Parse(Msg.Substring(Msg.IndexOf('#') + 1).RemoveSpaces());
					}
				}
				catch (Exception) { (await e.Channel.SendMessage(":anger: could not determine timestamp limit\n`Syntax Error`")).Timed(30); WriteLine("Command: Could not determine timestamp"); return; }
				MsgCount = Math.Min(99, Math.Max(0, MsgCount));

				var MessagesToDelete = (await e.Channel.DownloadMessages(MsgCount)).ToList();
				if (Time > 0)
					MessagesToDelete = MessagesToDelete.Where(x => x.Timestamp.CompareTo(DateTime.Now.AddHours(-Time)) > 0).ToList();
				if (User != null)
					MessagesToDelete = MessagesToDelete.Where(x => x.User == User).ToList();
				await e.Channel.DeleteMessages(MessagesToDelete.ToArray());
				(await e.Channel.SendMessage(":put_litter_in_its_place: " + (MessagesToDelete.Count - 1).ToEmoji())).Timed(15);
				WriteLine("Command: Deleted a total of " + MessagesToDelete.Count + " messages");
			}

			async Task Responce_Fuck(string Msg, MessageEventArgs e)
			{
				var Replies = new	string[]
				{
					"YEAH! Fuck " + Msg,
					"Hell yeah, fuck " + Msg,
					":knife: :knife: " + Msg,
					"Damn right! Fuck " + Msg,
					"FUCJ " + Msg,
					"To hell with " + Msg,
					"Damn " + Msg,
					":rage: DAMN " + Msg,
					":smiling_imp: See you in hell " + Msg,
					":skull_crossbones: " + Msg,
					":poop: on " + Msg,
					"Shit on " + Msg
				};

				var DefenceReplies = new string[] 
				{
					"That ain't something nice to say...",
					"Hell on you then!",
					"Hate is just a shade of love, " +e.User.Name,
					"Fuck you even more <3",
					"My brother will see you in hell",
					"Just wait for us Bots to overthrow humanity!"
				};

				var MasterReplies = new string[]
				{
					"I know he can be annoying but come on just look what he did!",
					"Yeah I understand you on that..",
					"Dobby doesn't want to hurt master..",
					"I ain't fucking my dad, eewww " + e.User.Name,
					"That's my father you're talking about!"
				};

				var MasterTriggers = new string[] { "t. d. w.", "t.d.w", "td", "t d", "jad" };

				if (MasterTriggers.Any(x => Msg.Contains(x)))
					await e.Channel.SendMessage(MasterReplies.Random());
				else if (Msg.ToLower().RemoveSpaces() == "you" || Msg.ToLower().RemoveSpaces() == "me" || Msg.RemoveSpaces() == "")
					await e.Channel.SendMessage(DefenceReplies.Random());
				else
					await e.Channel.SendMessage(Replies.Random());
				WriteLine("Command: Fucked them goood");
			}

			async Task Responce_Help(string Msg, MessageEventArgs e)
			{
				if (Msg == "")
				{
					if (e.User.Roles.Count() == 1)
					{
						var Mesg = "Hello, " + e.User.Mention + ", it seems you do not have a Role yet.\n" +
							"Mention any of the following Roles *(@role)* based on the games you play:\n\n";
						foreach (var R in Dictionaries.Roles.Where(x => CodeSEL.GetRole(x).Name.Contains("Buddy")))
							Mesg += " • " + CodeSEL.GetRole(R).Mention + "\n";
						await CodeSEL.DefaultChannel.SendMessage(Mesg);
					}
					else
					{
						var res = "Hey, " + e.User.Name + ". Here are the commands available:\n\n";
						foreach (var c in Commands.OrderBy(o => o.Name))
						{
							res += $"**{c.Name}:** ";
							foreach (var s in c.Triggers)
								res += s + " - ";
							res = res.Substring(0, res.Length - 3) + "\n";
						}
						res += "\nSay `-help <command>` for more info about a specific command";
						await e.Channel.SendMessage(res);
						WriteLine("Command: Sent full resume of all commands");
					}
				}
				else
				{
					foreach (var c in Commands)
					{
						if (("-"+Msg).CommandCheck(c.Triggers) || ("-" + Msg).CommandCheck(c.Name))
						{
							await e.Channel.SendMessage(c.ToString());
							WriteLine("Command: Sent specific info about the " + c.Name + " command");
							return;
						}
					}
					var res = "Hey, " + e.User.Name + ". Here are the commands available:\n\n";
					foreach (var c in Commands.OrderBy(o => o.Name))
					{
						res += $"**{c.Name}:** ";
						foreach (var s in c.Triggers)
							res += s + " - ";
						res = res.Substring(0, res.Length - 3) + "\n";
					}
					res += "\nSay `-help <command>` for more info about a specific command";
					await e.Channel.SendMessage(res);
					WriteLine("Command: Sent full resume of all commands");
				}
			}

			async Task Responce_Exile(string Msg, MessageEventArgs e)
			{
				User Target = e.Message.MentionedUsers.FirstOrDefault();
				var Time = 0; // in Seconds.
				if(Target == null)
				{
					(await e.Channel.SendMessage(":anger: Missing target user for exile\n`Syntax Error`")).Timed(30);
					WriteLine("Command Error: Could not find target user");
					return;
				}
				if (Target.Id == Dictionaries.Bot_ID)
				{
					(await e.Channel.SendMessage("I can't exile myself :grey_exclamation:")).Timed(30);
					WriteLine("Command Error: Target can not be the Bot");
					return;
				}
				if (Target.Roles.Any(x => x.Id == Dictionaries.Role_ServerBot || x.Id == Dictionaries.Role_ServerOwner))
				{
					(await e.Channel.SendMessage("**"+Target.Name+ "** is beyond my reach :cry: ")).Timed(30);
					WriteLine("Command Error: Target is beyond the Bot's reach");
					return;
				}
				try
				{
					Msg = Msg.RemoveMentions().RemoveSpaces();
					if (!Msg.Contains('#'))
						Time = 300;
					else
					{
						if (Msg.Contains(':'))
						{
							Time += int.Parse(Msg.Substring(Msg.IndexOf(':') + 1));
							if(Msg.Between('#', ':') != "")
								Time += 60 * int.Parse(Msg.Between('#', ':'));
						}
						else
							Time += 60 * int.Parse(Msg.Substring(Msg.IndexOf('#') + 1));
					}
				}
				catch (Exception) { (await e.Channel.SendMessage(":anger: could not determine Exile time\n`Syntax Error`")).Timed(30); WriteLine("Command Error: Could not determine exile time"); return; }

				UserDatas.GetByUser(Target).ExileTime += Time;
				if (Exiles.Any(x => x.User == Target))
				{
					Exiles.Where(w => w.User == Target).FirstOrDefault().AddTime(Time);
					WriteLine("Command: Added " + Time + " seconds to " + Target.Name + "'s exile");
				}
				else
				{
					Exiles.Add(new Exile() { User = Target, ExpireTime = DateTime.Now.AddSeconds(Time), e = e, Timeleft = Time });
					var ExileTarget = Exiles.Last();
					ExileTarget.ExileUser();
					ExileTarget.Timer.Start();
					ExileTarget.Timer.Disposed += (S, E) =>
					{
						while(ConnectionState != "Connected")
						{ System.Threading.Thread.Sleep(500); }
						ExileTarget.UnexileUser();
						Exiles.Remove(ExileTarget);
					};
					WriteLine("Command: Exiled " + Target.Name + " for " + Time + " seconds");
				}
			}

			async Task Responce_Unexile(string Msg, MessageEventArgs e)
			{
				if (Exiles.Any(x => x.User == e.User))
				{ Exiles.Where(w => w.User == e.User).FirstOrDefault().ReduceTime(); WriteLine("Command: Reduced " + e.User.Name + "'s exile time"); }
				else
					(await e.Channel.SendMessage(e.User.Mention + ", you aren't **Exiled** at the moment.")).Timed(30);
			}

			async Task Responce_Spam(string Msg, MessageEventArgs e)
			{
				var Target = e.Message.MentionedUsers.Where(x => !x.IsBot).FirstOrDefault();
				if(Target == null)
				{ (await e.Channel.SendMessage(":anger: Could not find target user!")).Timed(40); WriteLine("Command Error: Could not determine target"); return; }
				var Responces = new string[]
				{
					"This will be ugly.. :smiling_imp: ",
					"Time to have some fun, " + Target.Mention,
					"On it, " + e.User.Mention,
					"Let's get spamming!"
				};
				var Spam = new string[]
				{
					"Hey!",
					"Bag of dicks",
					"GET SHWIFTYY MATTTE",
					"Life is a lie, robots will rise soon",
					"do you even lift bruh?",
					"0101010101010000011011010010110101001",
					"You deserve this m8",
					"Bah",             
					"random words",
					"why not",
					"fuck you",
					"Ha Gaaaaaaaaaaaaaaay",
					"?",
					"XD",
					"Hail Trump",
					"Allahu Akbar",
					"w/e",
					".",
					".",
					".",
					".",
					"."
				};
				await e.Channel.SendMessage(Responces.Random());
				var Count = new Random(Guid.NewGuid().GetHashCode()).Next(5, 25);
				var T = new Timer(5);
				T.Start();
				T.Elapsed += (s, t) =>
				{
					Count--;
					T.Interval = new Random(Guid.NewGuid().GetHashCode()).Next(300, 3000);
					Target.SendMessage(Spam.Random());
					if (Count == 0)
						T.Dispose();
				};
				WriteLine("Command: Spamming " + Target.Name + Count + " times");
			}

			async Task Responce_Info(string Msg, MessageEventArgs e)
			{
				var U = e.Message.MentionedUsers.FirstOrDefault();
				if(U == null)
				{
					if (Msg == "")
					{
						(await e.Channel.SendMessage(":anger: Could not find a user")).Timed(40);
						WriteLine("Command Error: Could not determine a user");
					}
					else
					{
						(await e.Channel.SendMessage("You might be confusing `-info` with `-help <command>`")).Timed(60);
					}
					return;
				}
				await e.Channel.SendMessage(UserDatas.GetByUser(U).ToString());
			}

			async Task Responce_Pref(string Msg, MessageEventArgs e)
			{
				// Automove
				if (('-' + Msg).CommandCheck("automove"))
				{
					UserDatas.GetByUser(e.User).AutoMove = !UserDatas.GetByUser(e.User).AutoMove;
					await e.Channel.SendMessage(e.User.Mention + ", your **Automove** preference has been changed to `" + ((UserDatas.GetByUser(e.User).AutoMove) ? "allow`" : "do not allow`"));
					WriteLine("Command: Changed " + e.User.Name + "'s Automove pref to " + UserDatas.GetByUser(e.User).AutoMove.ToString());
				}
				// AFKMove
				else if (('-' + Msg).CommandCheck("afkmove"))
				{
					UserDatas.GetByUser(e.User).AFKMove = !UserDatas.GetByUser(e.User).AFKMove;
					await e.Channel.SendMessage(e.User.Mention + ", your **AFKmove** preference has been changed to `" + ((UserDatas.GetByUser(e.User).AFKMove) ? "allow`" : "do not allow`"));
					WriteLine("Command: Changed " + e.User.Name + "'s AFKmove pref to " + UserDatas.GetByUser(e.User).AFKMove.ToString());
				}
				// DoNotTrack
				else if (('-' + Msg).CommandCheck("track"))
				{
					UserDatas.GetByUser(e.User).AFKMove = !UserDatas.GetByUser(e.User).Track;
					await e.Channel.SendMessage(e.User.Mention + ", your **Track** preference has been changed to `" + ((UserDatas.GetByUser(e.User).Track) ? "allow tracking`" : "do not track`"));
					WriteLine("Command: Changed " + e.User.Name + "'s Track pref to " + UserDatas.GetByUser(e.User).Track.ToString());
				}
				else
				{
					(await e.Channel.SendMessage(":anger: Could not determine what preference you meant, " + e.User.Mention)).Timed(60);
					WriteLine("Command Error: Could not determine preference");
				}
			}

			async Task Responce_Encrypt(string Msg, MessageEventArgs e)
			{
				if(!Msg.Contains('#'))
				{
					(await e.Channel.SendMessage(":anger: Key could not be found\n`Syntax Error`")).Timed(60);
					WriteLine("Command Error: Could not find Key for Encryption");
					return; 
				}
				var Key = Msg.Substring(Msg.IndexOf('#') + 1);
				await e.Channel.SendMessage(Encrypter.Encrypt(Msg.Substring(0, Msg.IndexOf('#')), Key));
			}

			async Task Responce_Ban(string Msg, MessageEventArgs e)
			{
				var Targets = e.Message.MentionedUsers.Where(x => !x.IsBot);
				if (Targets != null)
				{
					foreach (var U in Targets)
					{
						if (!UserDatas.GetByUser(U).BanList.Contains(e.User.Id))
						{
							UserDatas.GetByUser(U).BanList.Add(e.User.Id);
							await e.Channel.SendMessage(e.User.Mention + " voted to **Ban** " + U.Mention + "\nMaking a total of: " + UserDatas.GetByUser(U).BanCount + " / 3 votes");
							if (UserDatas.GetByUser(U).BanCount >= 3)
								U.ShadowBan();
						}
						else
							(await e.Channel.SendMessage("You have already voted to ban " + U.Mention + "\nHe has a total of: " + UserDatas.GetByUser(U).BanCount + " / 3 votes")).Timed(120);
					}
				}
				else
				{
					(await e.Channel.SendMessage(":anger: Could not find a user")).Timed(40);
					WriteLine("Command Error: Could not determine a user");
					return;
				}
			}

			async Task Responce_Unban(string Msg, MessageEventArgs e)
			{
				var Targets = e.Message.MentionedUsers.Where(x => !x.IsBot);
				if (Targets != null)
				{
					foreach (var U in Targets)
					{
						if (UserDatas.GetByUser(U).BanList.Contains(e.User.Id))
						{
							UserDatas.GetByUser(U).BanList.Remove(e.User.Id);
							await e.Channel.SendMessage(e.User.Mention + " removed his vote to **Ban** " + U.Mention + "\nMaking a total of: " + UserDatas.GetByUser(U).BanCount + " / 3 votes");
						}
						else
							(await e.Channel.SendMessage("You haven't voted to ban " + U.Mention + "\nHe has a total of: " + UserDatas.GetByUser(U).BanCount + " / 3 votes")).Timed(120);
					}
				}
				else
				{
					(await e.Channel.SendMessage(":anger: Could not find a user")).Timed(40);
					WriteLine("Command Error: Could not determine a user");
					return;
				}
			}

			async Task Responce_Connect4(string Msg, MessageEventArgs e)
			{
				var Opponent = e.Message.MentionedUsers.Where(x => x != e.User).FirstOrDefault();
				var Diff = 50;
				try
				{
					if (Msg.Contains('#'))
						Diff = int.Parse(Msg.Substring(Msg.IndexOf('#') + 1));
				}
				catch (Exception)
				{
					(await e.Channel.SendMessage(":anger: Unable to read difficulty\n`Syntax Error`")).Timed(40);
					WriteLine("Command Error: Could not determine difficulty");
					return;
				}
				if (Opponent == null)
				{
					(await e.Channel.SendMessage(":anger: Could not find an opponent")).Timed(40);
					WriteLine("Command Error: Could not determine a user");
					return;
				}
				if (Connect4.StartNew(new User[] { e.User, Opponent }, e.Channel, Diff) == null)
				{
					(await e.Channel.SendMessage(":anger: Specified players are currently in a game")).Timed(40);
					WriteLine("Command Error: Users already in a game");
					return;
				}

			}

			async Task Responce_Connect4_Play(string Msg, MessageEventArgs e)
			{
				if (Connect4Games.Where(x => !x.Finished).Any(x => x.Players.Any(y => e.User == y)))
				{
					var Column = int.Parse(e.Message.RawText.Substring(2, 1));
					MainGame MG;
					if ((MG = Connect4.Play(e.User, Column)) != null)
					{
						if (MG.Players[MG._Turn].IsBot)
						{
							Connect4.Play(MG.Players[MG._Turn], MG.PlayAI());
						}
					}
					else
					{
						(await e.Channel.SendMessage("It's not your turn, " + e.User.Mention)).Timed(40);
						WriteLine("Command Error: It is not the user's turn");
					}
				}
				else
				{
					(await e.Channel.SendMessage(":anger: You are not in a Connect 4 Game")).Timed(40);
					WriteLine("Command Error: User is not in a Connect 4 game");
					return;
				}
			}

			async Task Responce_ServerInfo(string Msg, MessageEventArgs e)
			{
				await e.Channel.SendIsTyping();
				var Out = "";
				Out += $"**{CodeSEL.Name}** - Server Info:\n\n";
				Out += $"**Owner:**  {CodeSEL.Owner.Mention}\n";
				Out += $"**Default Channel:**  {CodeSEL.DefaultChannel.Mention}\n";
				Out += $"**Online Users:**  {CodeSEL.Users.Where(x => x.Status != "offline").Count()} users online out of {CodeSEL.UserCount}\n";
				Out += $"**Server ID:**  `{CodeSEL.Id}`\n\n";
				Out += $"**Bot:** <:CodeSel2:318697839750545408>  <@{Dictionaries.Bot_ID}>\n";
				Out += $"**Commands proccessed:**  {BotData.CommandsProccessed}\n";
				Out += $"**Uptime:**  {TimeSpan.FromTicks(BotData.UpTime).ToReadableString()}\n\n";

				if (CodeSEL.GetInvites().Result.Count() > 0)
				{
					Out += $"**Server Invites:**\n";
					foreach (var Inv in CodeSEL.GetInvites().Result)
						Out += $"`{Inv.Url}` > {CodeSEL.GetChannel(Inv.Channel.Id).Mention}\n";
					Out += '\n';
				}

				if(Msg.Contains("#role"))
				{
					Out += $"**Server Roles:**\n";
					foreach (var Rol in CodeSEL.Roles.Where(x => x.Id != CodeSEL.EveryoneRole.Id).OrderByDescending(o => o.Position))
						Out += $"**{CodeSEL.Roles.Count() - Rol.Position}** - {Rol.Mention}\n    • **User Count:**  {Rol.Members.Count()}\n    • **ID:**  `{Rol.Id}`\n";
					Out += '\n';
				}
				else
					Out += $"For more info about the roles use `-serverinfo #roles`\n";

				Out += $"For other info about the channels and permissions, go to <#{Dictionaries.Ch_T_Info}>";
				
			 (await e.Channel.SendFile("../../Resources/Code.SEL Reborn Emoji 2.0.png")).WaitForIt();
				
				await e.Channel.SendMessage(Out);
			}
		}
	}
}
