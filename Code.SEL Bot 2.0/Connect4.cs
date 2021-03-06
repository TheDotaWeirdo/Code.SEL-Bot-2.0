﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Xml.Serialization;
using System.IO;

namespace Code.SEL_Bot_2._0
{
	public partial class MainForm
	{
		public static List<Connect4Data> Connect4Datas;
		public static List<MainGame> Connect4Games;

		public class Connect4Data
		{
			public int		ID;
			public ulong  P1;
			public ulong  P2;
			public ulong  Starter;
			public int		Winner;
			public int		Difficulty;
			public bool		vsAI;
			public string Moves;
		}

		public class Connect4
		{
			public static MainGame StartNew(User[] Players, Channel Chan, int Diff = 50)
			{
				if (Connect4Games.Where(x => !x.Finished).Any(x => x.Players.Any(y => Players.Any(z => z == y))))
					return null;
				var MG = new MainGame()
				{
					ID = Connect4Games.Count + Connect4Datas.Count,
					Players = Players,
					vsAI = Players.Any(x => x.IsBot),
					Diff = Diff
				};
				if (MG.vsAI && MG._Turn == 1)
					MG.Play(4);
				MG.Msg = Chan.SendMessage(ConstructMsg(MG)).Result;
				Connect4Games.Add(MG);
				return MG;
			}

			public static string ConstructMsg(MainGame MG)
			{
				var Winner = MG.Winner;
				var S = "Connect <:Connect4:318497576070414336> game between :red_circle: " +
					MG.Players[0].Mention + "  &  :large_blue_circle: " + MG.Players[1].Mention +
					"\n`Game ID:  " + MG.ID + " `\n\n";
				for (int y = 1; y <= 6; y++)
				{
					for (int x = 1; x <= 7; x++)
					{
						S += Circles[MG.Case[y][x]];
					}
					S += '\n';
				}
				S += '\n';
				if (Winner > 0)
				{
					S += MG.Players[Winner - 1].Mention + "  Won the Game! :tada: :confetti_ball:";
					Connect4Datas.Add(ConvertData(MG));
					Connect4Games.Remove(MG);
				}
				else if (MG.Tied)
				{
					S += "The Game ended in a tie :necktie:";
					Connect4Datas.Add(ConvertData(MG));
					Connect4Games.Remove(MG);
				}
				else
					S += MG.Players[MG._Turn].Mention + "'s turn to play " + Circles[MG._Turn + 1];
				return S;
			}

			public static MainGame Play(User Player, int Column)
			{
				if(Connect4Games.Where(x => !x.Finished).Any(x => x.Players[x._Turn] == Player))
				{
					var Game = Connect4Games.Where(x => !x.Finished && x.Players[x._Turn] == Player).FirstOrDefault();
					Game.Play(Column);
					Game.Turn();
					Game.Msg.Edit(ConstructMsg(Game));
					return Game;
				}
				return null;
			}

			public static Connect4Data ConvertData(MainGame MG)
			{
				return new Connect4Data
				{
					ID = MG.ID,
					P1 = MG.Players[0].Id,
					P2 = MG.Players[1].Id,
					Difficulty = MG.Diff,
					Moves = MG.Moves,
					Starter = MG.Players[MG.Starter].Id,
					Winner = (MG.Tied) ? -1 : MG.Winner - 1,
					vsAI = MG.vsAI
				};
			}
		}

		public static Dictionary<int, string> Circles = new Dictionary<int, string>()
		{
			{-2, ":large_blue_circle:"                    },
			{-1, ":red_circle:"														},
			{0 , ":white_circle:"													},
			{1 , ":red_circle:"														},
			{2 , ":large_blue_circle:"										},
			{3 , "<:Gold_Red_Circle:285466048084312065>"	},
			{4 , "<:Gold_Blue_Circle:285466046494670848>" },
		};

		public static void LoadConnect4Data()
		{
			string file = "../../Data/Connect4Data.xml";
			XmlSerializer formatter = new XmlSerializer(new List<Connect4Data>().GetType());
			FileStream DataFile = new FileStream(file, FileMode.Open);
			byte[] buffer = new byte[DataFile.Length];
			DataFile.Read(buffer, 0, (int)DataFile.Length);
			MemoryStream stream = new MemoryStream(buffer);
			Connect4Datas = (List<Connect4Data>)formatter.Deserialize(stream);
		}

		public static void SaveConnect4Data()
		{
			string path = "../../Data/Connect4Data.xml";
			FileStream outFile = File.Create(path);
			XmlSerializer formatter = new XmlSerializer(new List<Connect4Data>().GetType());
			formatter.Serialize(outFile, Connect4Datas);
		}

		public class MainGame
		{
			public int ID;
			public User[] Players;
			public Message Msg;
			public int Starter;
			public int _Turn;
			public int Undos;
			public int MoveCount { get { return Moves.Length; } }
			public int Delay { get; private set; }
			public int[] P = { 0, 1 };
			private int winner = 0;
			private int diff;
			private int Positiveness = 0;
			public string Moves { get; private set; }
			public bool LeftIsRed = true;
			public bool Finished = false;
			public bool Busy = true;
			public bool Loading = false;
			public bool vsAI = true;
			public bool PredictiveAI = true;
			public bool StrategicAI = false;
			public bool HumanizedAI = false;
			public bool FastGame = false;
			private bool Lmode = false;
			private double WinChance;
			private double BlockChance;
			private double ThreatBlockChance;
			private double ThreatChance;
			private double PredictWinChance;
			private double PredictLossChance;
			private double StrategicCheckChance;
			private double StrategicBlockChance;
			private double FutureStrategicBlockChance;
			private double SacrificeChance;
			public List<List<int>> Case = new List<List<int>>();
			private List<List<int>> _Case = new List<List<int>>();
			public Dictionary<int, List<int>> PlayerState = new Dictionary<int, List<int>>();
			public Dictionary<int, int> PlayerSeverity = new Dictionary<int, int>();
			public Dictionary<int, List<int>> AIState = new Dictionary<int, List<int>> { { 1, new List<int> { 0 } }, { 2, new List<int> { 0 } }, { 3, new List<int> { 0 } }, { 4, new List<int> { 0 } }, { 5, new List<int> { 0 } }, { 6, new List<int> { 0 } }, { 7, new List<int> { 0 } } };
			public Dictionary<int, int> AISeverity = new Dictionary<int, int> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 } };
			private Dictionary<string, Pattern> Patterns = new Dictionary<string, Pattern>();
			private Dictionary<int, List<int>> St;
			private Dictionary<int, int> Se;
			private Dictionary<int, List<int>> HumanState = new Dictionary<int, List<int>> { { 1, new List<int> { 0 } }, { 2, new List<int> { 0 } }, { 3, new List<int> { 0 } }, { 4, new List<int> { 0 } }, { 5, new List<int> { 0 } }, { 6, new List<int> { 0 } }, { 7, new List<int> { 0 } } };
			private Dictionary<int, int> HumanSeverity = new Dictionary<int, int> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 } };
			private Dictionary<int, int> Effectiveness = new Dictionary<int, int>
				{ { -10, 1 }, { -9, -1 }, { -8, 3 }, { -7, -2 }, { -6, 2 }, { -5, 3 }, { -4, 1 }, { -3, -1 }, { -2, -2 }, { -1, -2 }, { 0, 0 }, { 1, -2 }, { 2, 0 }, { 3, 0 }, { 4, 2 }, { 5, 2 }, { 6, -1 }, { 7, -1 }, { 8, 1 }, { 9, -2 }, { 10, 2 } };

			// States of each column are cases where specified conditions are met, a column can have more than one state at a time
			// State Severity of a column is calculated based on the multiple states applied in a column, the higher the severity the more likely the column will be played

			//-10: FuturePredictiveCheck | checks if a play by the bot will create a Predicted Threat (8) state in another column
			// -9: FuturePredictiveBlock | checks if a play by the player will create a Predicted ThreatBlocked (7) state in another column
			// -8: Threat Sacrifice | sacrifice a Threat in order to force another one (win in 3 moves)
			// -7: Possible Loss Prediction | checks if a play by the Player will create a possible Loss (6) (Loss in 4 moves)
			// -6: Possible Win Prediction | checks if a play by the Bot will create a possible Win (5) (win in 2 moves)
			// -5: Block/Mistake Force Prediction | checks if a play by the Bot will force the player into surrendering a Threat Block or Mistake
			// -4: StrategicCheck | checks if a play by the bot will create a Threat state in another column
			// -3: StrategicBlock | checks if a play by the player will create a Threat Block state in another column
			// -2: FutureStrategicBlock | checks if a ThreatBlock will be created after 2 moves in the column
			// -1: Block/Mistake Cancel Prediction | checks if a play by the player will force the bot into surrendering a Threat Block or Mistake
			//  0: Free | no certain states for the column
			//  1: ThreatBlocked | case where playing in the column will result in a possible loss (loss in 2 moves)
			//  2: NearlyFull | case where the column is 1 token away from being full
			//  3: Full | case where the whole column is full
			//  4: Threat | case where playing in the column will result in a loss of a possible win
			//  5: Possible Win | (Win in 1 move)
			//  6: Possible Loss | (Loss in 1 move)
			//  7: Predicted ThreatBlocked | checks if a ThreatBlock will occur after one play in the column
			//  8: Predicted Threat | checks if a Threat will occur after one play in the column
			//  9: Relevant Future Threat Block | checks if there is any 3+ layer threat possibility that is relevant by odd/even standards
			//10: Relevant Future Threat | checks if there is any 3+ layer threat possibility that is relevant by odd/even standards

			public int Diff
			{
				get { return diff; }
				set
				{
					diff = value;
					Delay = (100 - value) * ((FastGame) ? 4 : 12);
					WinChance = (value * .1) + 90;
					BlockChance = (value * .2) + 80;
					ThreatBlockChance = (value * .6) + 40;
					ThreatChance = (value * .7) + 30;
					PredictWinChance = (value * .5) + 50;
					PredictLossChance = (value * .6) + 40;
					StrategicCheckChance = (value * .6) + 40;
					StrategicBlockChance = (value * .7) + 30;
					FutureStrategicBlockChance = (value * .8) + 20;
					SacrificeChance = (value * .7) + 30;
					PredictiveAI = value >= 33;
					StrategicAI = value >= 66;
					HumanizedAI = value >= 99;
				}
			}
			public int Winner
			{
				get
				{
					if (winner != 0) return winner;
					int output = 0;
					List<Point> LP = new List<Point>();
					for (var x = 1; x < 8; x++)
					{
						for (var y = 6; y > 0; y--)
						{
							if (Case[y][x] == 0) break;
							CheckData CD;
							if (y < 4 && x < 5)
							{
								CD = Case.CheckDiagDown(x, y);
								if (CD.Check)
								{
									output = CD.Color;
									for (var i = 0; i < CD.Points.Count; i++)
										LP.Add(CD.Points[i]);
								}
							}
							if (y > 3 && x < 5)
							{
								CD = Case.CheckDiagUp(x, y);
								if (CD.Check)
								{
									output = CD.Color;
									for (var i = 0; i < CD.Points.Count; i++)
										LP.Add(CD.Points[i]);
								}
							}
							if (x < 5)
							{
								CD = Case.CheckHorizontal(x, y);
								if (CD.Check)
								{
									output = CD.Color;
									for (var i = 0; i < CD.Points.Count; i++)
										LP.Add(CD.Points[i]);
								}
							}
							if (y < 4)
							{
								CD = Case.CheckVertical(x, y);
								if (CD.Check)
								{
									output = CD.Color;
									for (var i = 0; i < CD.Points.Count; i++)
										LP.Add(CD.Points[i]);
								}
							}
						}
					}
					if (output > 0)
					{
						Finished = true;
						for (var y = 1; y < 7; y++)
							for (var x = 1; x < 8; x++)
								Case[y][x] = -Case[y][x];
						foreach (Point P in LP)
							Case[P.Y][P.X] = output + 2;
					}
					return winner = output;
				}
			}
			public bool Tied
			{
				get
				{
					for (var x = 1; x < 8; x++)
						if (Case[1][x] == 0)
							return false;
					for (var y = 1; y < 7; y++)
						for (var x = 1; x < 8; x++)
							Case[y][x] = -Case[y][x];
					Finished = true;
					return true;
				}
			}
			public bool LearnMode
			{
				get { return Lmode && vsAI; }
				set { Lmode = value; }
			}

			public MainGame()
			{
				_Turn = (new Random().NextDouble() < .5) ? 0 : 1;
				Moves = "";
				Diff = 50;
				// Case initialization
				Case.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 });
				Case.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 });
				Case.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 });
				Case.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 });
				Case.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 });
				Case.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 });
				Case.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 });
				// Pattern initialization
				// 2 - Token Diagonal
				Patterns.Add("2TDiag1", new Pattern('c', 12, 1, -1));
				Patterns["2TDiag1"].Shape.Add(new List<int> { -1, -1, -1, 0 });
				Patterns["2TDiag1"].Shape.Add(new List<int> { -1, 1, 0, -1 });
				Patterns["2TDiag1"].Shape.Add(new List<int> { -1, 1, -1, -1 });
				Patterns["2TDiag1"].Shape.Add(new List<int> { 1, -1, -1, -1 });
				Patterns.Add("2TDiag2", Patterns["2TDiag1"].Mirror);
				// 2 - Token Diagonal
				Patterns.Add("2TDiag1_2", new Pattern('c', 12, 1, -1));
				Patterns["2TDiag1_2"].Shape.Add(new List<int> { -1, -1, -1, 0 });
				Patterns["2TDiag1_2"].Shape.Add(new List<int> { -1, 1, 1, -1 });
				Patterns["2TDiag1_2"].Shape.Add(new List<int> { -1, 0, -1, -1 });
				Patterns["2TDiag1_2"].Shape.Add(new List<int> { 1, -1, -1, -1 });
				Patterns.Add("2TDiag2_2", Patterns["2TDiag1_2"].Mirror);
				// 2 - Token Diagonal
				Patterns.Add("2TDiag1_3", new Pattern('c', 12, 1, -1));
				Patterns["2TDiag1_3"].Shape.Add(new List<int> { -1, -1, -1, 0 });
				Patterns["2TDiag1_3"].Shape.Add(new List<int> { -1, 1, 1, -1 });
				Patterns["2TDiag1_3"].Shape.Add(new List<int> { -1, 1, -1, -1 });
				Patterns["2TDiag1_3"].Shape.Add(new List<int> { 0, -1, -1, -1 });
				Patterns.Add("2TDiag2_3", Patterns["2TDiag1_3"].Mirror);
				// 2 - Token Straight
				Patterns.Add("2TStr1", new Pattern('c', 12, 1, -1));
				Patterns["2TStr1"].Shape.Add(new List<int> { 1, 1, 0, 0 });
				Patterns.Add("2TStr2", Patterns["2TStr1"].Mirror);
				// Seven
				Patterns.Add("Seven1", new Pattern('c', 20, 4, -3));
				Patterns["Seven1"].Shape.Add(new List<int> { 1, -1, 1, -1 });
				Patterns["Seven1"].Shape.Add(new List<int> { -1, 1, 1, -1 });
				Patterns["Seven1"].Shape.Add(new List<int> { -1, -1, 1, -1 });
				Patterns["Seven1"].Shape.Add(new List<int> { -1, -1, 0, 0 });
				Patterns.Add("Seven2", Patterns["Seven1"].Mirror);
				// Arrow
				Patterns.Add("Arrow1", new Pattern('c', 22, 2, -3));
				Patterns["Arrow1"].Shape.Add(new List<int> { 0, 0, -1, -1 });
				Patterns["Arrow1"].Shape.Add(new List<int> { 0, 1, 1, 1 });
				Patterns["Arrow1"].Shape.Add(new List<int> { -1, 1, 1, -1 });
				Patterns["Arrow1"].Shape.Add(new List<int> { -1, 1, -1, 1 });
				Patterns.Add("Arrow2", Patterns["Arrow1"].Mirror);
				// L
				Patterns.Add("L1", new Pattern('c', 24, 3, -2));
				Patterns["L1"].Shape.Add(new List<int> { -1, 0, -1, -1 });
				Patterns["L1"].Shape.Add(new List<int> { 0, 1, 1, 1 });
				Patterns["L1"].Shape.Add(new List<int> { -1, 1, -1, -1 });
				Patterns["L1"].Shape.Add(new List<int> { -1, 1, -1, -1 });
				Patterns.Add("L2", Patterns["L1"].Mirror);
				// Diamond
				Patterns.Add("Diamond1", new Pattern('c', 26, 5, -3) { Parallax = true });
				Patterns["Diamond1"].Shape.Add(new List<int> { -1, 0, -1, 0, -1 });
				Patterns["Diamond1"].Shape.Add(new List<int> { 0, -1, 1, -1, 0 });
				Patterns["Diamond1"].Shape.Add(new List<int> { -1, 1, -1, 1, -1 });
				Patterns["Diamond1"].Shape.Add(new List<int> { -1, -1, 1, -1, -1 });
				Patterns.Add("Diamond2", Patterns["Diamond1"].Mirror);
				// Square
				Patterns.Add("Square1", new Pattern('c', 28, 2, -1) { Parallax = true });
				Patterns["Square1"].Shape.Add(new List<int> { -1, -1, -1, -1 });
				Patterns["Square1"].Shape.Add(new List<int> { -1, 1, 1, -1 });
				Patterns["Square1"].Shape.Add(new List<int> { -1, 1, 1, -1 });
				Patterns.Add("Square2", Patterns["Square1"].Mirror);
				// T
				Patterns.Add("T1", new Pattern('c', 30, 2, -1) { Parallax = true });
				Patterns["T1"].Shape.Add(new List<int> { -1, -1, 0, -1, -1 });
				Patterns["T1"].Shape.Add(new List<int> { 0, 1, 1, 1, 0 });
				Patterns["T1"].Shape.Add(new List<int> { -1, -1, 1, -1, -1 });
				Patterns["T1"].Shape.Add(new List<int> { -1, -1, 1, -1, -1 });
				Patterns.Add("T2", Patterns["T1"].Mirror);
				// Z
				Patterns.Add("Z1", new Pattern('c', 32, 2, -1));
				Patterns["Z1"].Shape.Add(new List<int> { 0, -1, -1, -1 });
				Patterns["Z1"].Shape.Add(new List<int> { 0, 1, 1, 0 });
				Patterns["Z1"].Shape.Add(new List<int> { -1, -1, 1, -1 });
				Patterns["Z1"].Shape.Add(new List<int> { -1, -1, 1, 1 });
				Patterns.Add("Z2", Patterns["Z1"].Mirror);

				// Situations
				// House
				Patterns.Add("House1", new Pattern('s', -21, 2, -2) { Parallax = true });
				Patterns["House1"].Shape.Add(new List<int> { -1, -2, -1, -2, -1 });
				Patterns["House1"].Shape.Add(new List<int> { -1, -1, -1, -1, -1 });
				Patterns["House1"].Shape.Add(new List<int> { -1, -1, -1, -1, -1 });
				Patterns["House1"].Shape.Add(new List<int> { -1, -1, -1, -1, -1 });
				Patterns["House1"].Shape.Add(new List<int> { 0, 0, 1, 0, 0 });
				Patterns["House1"].Shape.Add(new List<int> { -1, 1, -1, 1, -1 });
				Patterns["House1"].Shape.Add(new List<int> { -1, 1, -1, 1, -1 });
				Patterns.Add("House2", Patterns["House1"].Mirror);
			}

			// Initializes the Moves variable with the difficulty and game starter.
			public void StartMoveCounting()
			{
				if (diff < 10)
					Moves = "00" + diff.ToString() + Starter.ToString();
				else if (diff < 100)
					Moves = "0" + diff.ToString() + Starter.ToString();
				else
					Moves = "100" + Starter.ToString();
			}

			// Plays a token in the index column.
			public void Play(int index)
			{
				Moves += index.ToString();
				Case[Case.GetLow(index)][index] = _Turn + 1;
			}

			// Switches the Turn.
			public void Turn()
			{
				_Turn = (_Turn == 0) ? 1 : 0;
			}

			// Undos the last Move.
			public Point Undo()
			{
				if (MoveCount > 0 && Undos > 0)
				{
					var t = int.Parse(Moves.Last().ToString());
					var p = new Point(t, Case.GetLow(t) + 1);
					Case[p.Y][p.X] = 0;
					Moves = Moves.Substring(0, Moves.Length - 1);
					Undos--;
					return p;
				}
				throw new InvalidOperationException();
			}

			// Assigns PlayerState/Severity for the Player.
			public void AsignHelps()
			{
				int p0 = P[0], p1 = P[1], d = diff;
				bool b1 = PredictiveAI, b2 = StrategicAI;
				P[0] = p1; P[1] = p0;
				St = PlayerState; Se = PlayerSeverity;
				Diff = 100;
				StrategicAI = PredictiveAI = true;
				_Case = Case.CreateCopy();
				CalculateStates();
				PredictiveAI = b1; StrategicAI = b2;
				Diff = d;
				P[0] = p0; P[1] = p1;
			}

			// Returns the AI's move for the turn.
			public int PlayAI()
			{
				// Calculate the State & Severity of each column
				St = AIState;
				Se = AISeverity;
				Positiveness = 0;
				_Case = Case.CreateCopy();
				CalculateStates();
				AISeverity[4] = (int)((AISeverity[4] + 1) * 1.5);
				// Run the Humanization function
				if (HumanizedAI)
				{
					for (var i = 1; i < 8; i++)
						AISeverity[i] *= 2;
					Humanize_1Step();
				}
				if (Finished)
					return 0; // In case the game was declared finished while calculations were ongoing
										// Chooses a column based on the severity of all of them
										// It will choose a random column from the highest severity 
				Se = AISeverity;
				return ChooseColumn();
			}

			// Calculates all the states and severities for each column.
			public void CalculateStates()
			{
				var RND = new Random(Guid.NewGuid().GetHashCode());
				// Loops through all columns to calculate the states and severity
				for (var x = 1; x < 8; x++)
				{
					// Resets the column's old values
					Se[x] = 0;
					St[x] = new List<int> { 0 };
					var y = _Case.GetLow(x);
					// If the Column is full, state = 3
					if (y == 0)
					{
						St[x].Add(3);
						Se[x] = -100;
						Positiveness += Effectiveness[3];
					}
					else
					{
						// Checks for possible wins
						_Case[y][x] = P[1] + 1;
						if (CheckWin(ref _Case) == P[1] + 1)
						{
							St[x].Add(5);
							Se[x] += 150 / ((RND.NextDouble() * 100 <= WinChance) ? 1 : 2);
							Positiveness += Effectiveness[5];
						}
						else
						{
							// Checks for possible losses
							_Case[y][x] = P[0] + 1;
							if (CheckWin(ref _Case) == P[0] + 1 && RND.NextDouble() * 100 <= BlockChance)
							{
								St[x].Add(6);
								Se[x] += 30 / ((RND.NextDouble() * 100 <= BlockChance) ? 1 : 2);
								Positiveness += Effectiveness[6];
							}
						}
						_Case[y][x] = 0;
						// In case the column is 1-token short from being full
						if (y == 1)
						{
							St[x].Add(2);
							Se[x] -= 1;
							Positiveness += Effectiveness[2];
						}
						else
						{
							// In case playing in the column will result in an open possible loss
							if (CheckThreat(x, P[0], 1))
							{
								St[x].Add(1);
								Se[x] -= 13 / ((RND.NextDouble() * 100 <= ThreatBlockChance) ? 1 : 2);
								Positiveness += Effectiveness[1];
							}
							// In case playing in the column will result in a loss of a possible win
							if (CheckThreat(x, P[1], 1))
							{
								St[x].Add(4);
								Se[x] -= (6 - ThreatUsefulness(P[1], x, 1)) / ((RND.NextDouble() * 100 <= ThreatChance) ? 1 : 2);
								Positiveness += Effectiveness[4];
							}
						}
					}
				}

				for (var x = 1; x < 8; x++)
				{
					var y = _Case.GetLow(x);
					// Predictive AI calculations
					if (PredictiveAI)
					{
						// In case playing the column will result in a possible win state
						int temp;
						_Case[y][x] = P[1] + 1;
						if ((temp = CountPossWin(x)) > 0)
						{
							St[x].Add(-6);
							Se[x] += (int)Math.Floor(1.4 * (temp + 1)) / ((RND.NextDouble() * 100 <= PredictWinChance) ? 1 : 2);
							Positiveness += Effectiveness[-6];
						}
						// In case the Player playing the column will result in a possible loss state
						_Case[y][x] = P[0] + 1;
						if ((temp = CountPossLoss(x)) > 0)
						{
							St[x].Add(-7);
							Se[x] += (int)Math.Floor(1.4 * (temp + 1)) / ((RND.NextDouble() * 100 <= PredictLossChance) ? 1 : 2);
							Positiveness += Effectiveness[-7];
						}
						_Case[y][x] = 0;

						if (y - 2 > 0)
						{
							// In case playing in the column will result in an open possible loss
							if (CheckThreat(x, P[0], 2))
							{
								St[x].Add(7);
								Se[x] -= ThreatUsefulness(P[0], x, 2) / ((RND.NextDouble() * 100 <= ThreatBlockChance) ? 1 : 2);
								Positiveness += Effectiveness[7];
							}
							// In case playing in the column will result in a loss of a possible win
							if (CheckThreat(x, P[1], 2))
							{
								St[x].Add(8);
								Se[x] += 2 + ThreatUsefulness(P[1], x, 2) / ((RND.NextDouble() * 100 <= ThreatChance) ? 1 : 2);
								Positiveness += Effectiveness[8];
							}
						}

						// Checks if after 2 plays, a ThreatBlock will be created(1)
						if (CheckThreat(x, P[0], 3))
						{
							St[x].Add(-2);
							Se[x] -= (RND.NextDouble() * 100 <= FutureStrategicBlockChance) ? 1 : 0;
							Positiveness += Effectiveness[2];
						}
					}
					// Strategic AI calculations
					if (StrategicAI)
					{
						// Checks if playing in the column will create a Threat state (4)
						if (StrategicCheck(x, 1, 1))
						{
							St[x].Add(-4);
							Se[x] += 2 / ((RND.NextDouble() * 100 <= StrategicCheckChance) ? 1 : 2);
							Positiveness += Effectiveness[-4];
						}
						// Checks if the Player playing in the column will create a Threat Block state (1)
						if (StrategicCheck(x, 0, 1))
						{
							St[x].Add(-3);
							Se[x] += 2 / ((RND.NextDouble() * 100 <= StrategicBlockChance) ? 1 : 2);
							Positiveness += Effectiveness[-3];
						}
						// Checks if a column with Threat will still have a Threat after one move in it
						if (CheckThreat(x, P[1], 1) && !CheckThreat(x, P[0], 1) && CheckThreat(x, P[1], 2))
						{
							St[x].Add(-8);
							Se[x] += 7 / ((RND.NextDouble() * 100 <= SacrificeChance) ? 1 : 2);
							Positiveness += Effectiveness[-8];
						}
						// Checks if playing in the column will create a Predicted Threat state (8)
						if (StrategicCheck(x, 1, 2))
						{
							St[x].Add(-10);
							Se[x] += ((RND.NextDouble() * 100 <= StrategicCheckChance) ? 1 : 0);
							Positiveness += Effectiveness[-10];
						}
						// Checks if the Player playing in the column will create a Predicted Threat Block state (7)
						if (StrategicCheck(x, 0, 2))
						{
							St[x].Add(-9);
							Se[x] += ((RND.NextDouble() * 100 <= StrategicBlockChance) ? 1 : 0);
							Positiveness += Effectiveness[-9];
						}
					}

					if (HumanizedAI)
					{
						for (var i = 3; i < 7; i++)
						{
							if (StrategicCheck(x, 0, i))
							{
								St[x].Add(9);
								Se[x] += 2;
								Positiveness += Effectiveness[9];
							}
							if (StrategicCheck(x, 1, i))
							{
								St[x].Add(10);
								Se[x] += 2;
								Positiveness += Effectiveness[10];
							}
						}
					}
				}

				if (HumanizedAI)
					Humanize_Patterns();
			}

			// Checks the positiveness for each column for the AI.
			public void Humanize_1Step()
			{
				_Case = Case.CreateCopy();
				int p0 = P[0], p1 = P[1];
				P[1] = p0; P[0] = p1;
				St = PlayerState; Se = PlayerSeverity;
				var PlayerPositiveness = new List<int> { 0 };
				for (var x = 1; x < 8; x++)
				{
					if (!_Case.IsFull(x))
					{
						int y = _Case.GetLow(x);
						_Case[y][x] = P[0] + 1;
						Positiveness = 0;
						CalculateStates();
						_Case[y][x] = 0;
						PlayerPositiveness.Add(-Positiveness);
					}
					else
						PlayerPositiveness.Add(0);
				}
				for (var j = 1; j < 8; j++)
					if (!_Case.IsFull(j))
						AISeverity[j] += (int)Math.Round(PlayerPositiveness[j] / 5d);
				P[1] = p1; P[0] = p0;
				Humanize_EndPrediction();
			}

			// Checks for win/loss outcomes within the next 10 moves.
			public void Humanize_EndPrediction()
			{
				int p0 = P[0], p1 = P[1];
				var SaveCase = new List<List<int>>();
				var Winner = 0;
				var turn = 0;
				var TotMoves = 0;
				for (var i = 1; i < 8; i++)
				{
					if (Case.IsFull(i)) continue;
					Winner = 0;
					turn = 0;
					TotMoves = 0;
					SaveCase = Case.CreateCopy();
					SaveCase[SaveCase.GetLow(i)][i] = P[1] + 1;
					while ((Winner = CheckWin(ref SaveCase)) == 0 && TotMoves < 10)
					{
						TotMoves++;
						St = HumanState; Se = HumanSeverity;
						if (turn == 0)
						{ P[0] = p1; P[1] = p0; }
						else
						{ P[0] = p0; P[1] = p1; }
						CalculateStates();
						if (St.All(x => x.Value.Contains(3))) break;
						int temp = ChooseColumn();
						SaveCase[SaveCase.GetLow(temp)][temp] = P[1] + 1;
						turn = (turn == 0) ? 1 : 0;
					}
					P[0] = p0; P[1] = p1;
					if (TotMoves > 0)
					{
						if (Winner == P[0] + 1)
							AISeverity[i] -= (int)Math.Round(100 / Math.Pow(TotMoves + 2, .55) - 24.5);
						else if (Winner == P[1] + 1)
							AISeverity[i] += (int)Math.Round(100 / Math.Pow(TotMoves + 2, .55) - 24.5);
					}
				}
				P[0] = p0; P[1] = p1;
			}

			// Checks for patterns and situations.
			private void Humanize_Patterns()
			{
				_Case = Case.CreateCopy();
				for (var x = 1; x < 8; x++)
				{
					var y = _Case.GetLow(x);
					_Case[y][x] = P[0] + 1;
					foreach (Pattern P in Patterns.Values.Where(p => p.PlayerID == P[0]))
					{
						if (P.Compare(ref _Case))
						{
							_Case[y][x] = 0;
							if (!P.Compare(ref _Case))
							{
								St[x].Add(P.StateID);
								Se[x] += P.Severity;
								if (LeftIsRed)
									Positiveness += P.Effectiveness;
								else
									Positiveness -= P.Effectiveness;
							}
							_Case[y][x] = this.P[0] + 1;
						}
					}
					_Case[y][x] = P[1] + 1;
					foreach (Pattern P in Patterns.Values.Where(p => p.PlayerID == P[1]))
					{
						if (P.Compare(ref _Case))
						{
							_Case[y][x] = 0;
							if (!P.Compare(ref _Case))
							{
								St[x].Add(P.StateID);
								Se[x] += P.Severity;
								if (LeftIsRed)
									Positiveness += P.Effectiveness;
								else
									Positiveness -= P.Effectiveness;
							}
							_Case[y][x] = this.P[1] + 1;
						}
					}
					_Case[y][x] = 0;
				}
				foreach (Pattern P in Patterns.Values.Where(p => p.Type == 's'))
				{
					if (P.Compare(ref _Case))
					{
						foreach (var i in P.SituationPlays)
						{
							St[i].Add(P.StateID);
							Se[i] += P.Severity;
							if (LeftIsRed)
								Positiveness += P.Effectiveness;
							else
								Positiveness -= P.Effectiveness;
						}
					}
				}
			}

			// Counts the total of loss possibilities in a column.
			private int CountPossLoss(int n)
			{
				var output = 0;
				var y = 0;

				for (var x = 1; x < 8; x++)
				{
					//In _Case of Possible Win or Loss
					if ((y = _Case.GetLow(x)) > 0 && x != n)
					{
						_Case[y][x] = P[0] + 1;
						if (CheckWin(ref _Case) == P[0] + 1)
						{
							output++;
							if (StrategicAI && (St[x].Contains(1) || St[x].Contains(4)))
							{ St[n].Add(-1); Se[n] += 2; }
						}
						_Case[y][x] = 0;
					}
				}

				if (output > 0 && _Case[1][n] == 0)
				{
					_Case[_Case.GetLow(n)][n] = P[0] + 1;
					if (CheckWin(ref _Case) == P[0] + 1)
					{
						output++;
						if (StrategicAI && (St[n].Contains(1) || St[n].Contains(4)))
						{ St[n].Add(-1); Se[n] += 2; }
					}
					_Case[_Case.GetLow(n)][n] = 0;
				}
				return (int)Math.Pow(output, 2);
			}

			// Counts the total of win possibilities in a column.
			private int CountPossWin(int n)
			{
				var output = 0;
				var y = 0;

				for (var x = 1; x < 8; x++)
				{
					//In _Case of Possible Win or Loss
					if ((y = _Case.GetLow(x)) > 0 && x != n)
					{
						_Case[y][x] = P[1] + 1;
						if (CheckWin(ref _Case) == P[1] + 1)
						{
							output++;
							if (StrategicAI && (St[x].Contains(1) || St[x].Contains(4)))
							{ St[n].Add(-5); Se[n] += 3; Positiveness += 2; }
							for (var i = 1; i < 8; i++)
							{
								if (!_Case.IsFull(i))
								{
									int y2 = _Case.GetLow(i);
									_Case[y2][i] = P[0] + 1;
									if (CheckWin(ref _Case) == P[0] + 1)
									{
										if (St[n].Contains(-5))
											St[n].Remove(-5);
										Se[x] -= 3;
										Positiveness -= 2;
									}
									_Case[y2][i] = 0;
								}
							}
						}
						_Case[y][x] = 0;
					}
				}

				if (output > 0 && _Case[1][n] == 0)
				{
					_Case[_Case.GetLow(n)][n] = P[1] + 1;
					if (CheckWin(ref _Case) == P[0] + 1)
					{
						output++;
						if (StrategicAI && (St[n].Contains(1) || St[n].Contains(4)))
						{ St[n].Add(-5); Se[n] += 3; }
					}
					_Case[_Case.GetLow(n)][n] = 0;
				}
				return (int)Math.Pow(output, 2);
			}

			// Chooses a column to play based on the highest Severity.
			private int ChooseColumn()
			{
				var Max = Se.Values.Max();
				var d = Se.Where(x => x.Value == Max);
				return d.ElementAt(new Random().Next(d.Count())).Key;
			}

			// Checks if a play in the column by Player#ID will create a threat in another column.
			private bool StrategicCheck(int x, int ID, int Severity)
			{
				var y = _Case.GetLow(x);
				_Case[y][x] = P[ID] + 1;
				for (var i = 1; i < 8; i++)
				{
					if (x != i)
					{
						if (Severity == 1)
						{
							if (!St[i].Contains((ID == 0) ? 1 : 4) && CheckThreat(i, P[ID], Severity))
							{ _Case[y][x] = 0; return true; }
						}
						else if (Severity == 2)
						{
							if (!St[i].Contains((ID == 0) ? 7 : 8) && CheckThreat(i, P[ID], Severity))
							{ _Case[y][x] = 0; return true; }
						}
						else if (CheckThreat(i, P[ID], Severity) && ThreatUsefulness(P[ID], i, Severity) == 2)
						{
							_Case[y][x] = 0;
							if (CheckThreat(i, P[ID], Severity))
								_Case[y][x] = P[ID] + 1;
							else
								return true;
						}
					}
				}
				_Case[y][x] = 0;
				return false;
			}

			// Checks if there's a threat in the column as the Layers height.
			public bool CheckThreat(int Column, int Winner, int Layers)
			{
				var y = _Case.GetLow(Column);
				var w = Winner;
				var b = true;

				if (y - Layers <= 0) return false;

				if (Layers % 2 == 1)
					w = (w == 1) ? 0 : 1;

				for (var i = y; i >= y - Layers; i--)
				{
					_Case[i][Column] = w + 1;
					w = (w == 1) ? 0 : 1;
					if (i != y - Layers && CheckWin(ref _Case) != 0)
						b = false;
				}

				b = b && CheckWin(ref _Case) == Winner + 1;
				for (var i = y - Layers; i <= y; i++)
					_Case[i][Column] = 0;
				return b;
			}

			// Returns 2 if a threat satisfies the odd/even rule in Connect 4, returns -2 if not.
			private int ThreatUsefulness(int Pl, int Column, int Layers)
			{
				if (!StrategicAI) return 0;
				var y = _Case.GetLow(Column) - Layers;
				if ((y % 2 == 0) == (Starter == Pl))
					return 2;
				return -2;
			}

			// Checks checkCase for Wins without declaring the game finished.
			public int CheckWin(ref List<List<int>> checkCase)
			{
				for (var x = 1; x < 8; x++)
				{
					for (var y = 6; y > 0; y--)
					{
						if (checkCase[y][x] == 0) break;
						CheckData CD;
						if (y < 4 && x < 5)
						{
							CD = checkCase.CheckDiagDown(x, y);
							if (CD.Check)
								return CD.Color;
						}
						if (y > 3 && x < 5)
						{
							CD = checkCase.CheckDiagUp(x, y);
							if (CD.Check)
								return CD.Color;
						}
						if (x < 5)
						{
							CD = checkCase.CheckHorizontal(x, y);
							if (CD.Check)
								return CD.Color;
						}
						if (y < 4)
						{
							CD = checkCase.CheckVertical(x, y);
							if (CD.Check)
								return CD.Color;
						}
					}
				}
				return 0;
			}
		}

		public class Pattern
		{
			public char Type; // Situation (s) OR Combination (c)
			public List<List<int>> Shape = new List<List<int>>(); // Shape of the Pattern; -2: Play Here | -1: Any | 0: Free | 1/2: Circle Color
			public int Severity; // Effectiveness change if pattern was found
			public int StateID; // State ID of the pattern
			public int Effectiveness; // Positiveness added if pattern was found
			public int PlayerID = 0; // 0: Player 1 | 1: Player 2
			public List<int> SituationPlays; // Includes columns to play after a Situation (s) is found
			public bool Parallax = false; // If true, the algorithm will skip creating a Mirror of the Shape

			public Pattern(char c, int id, int s, int e) { Type = c; StateID = id; Severity = s; Effectiveness = e; }

			// Creates a new Pattern with '2's instead of '1's and changes the ID/Effectiveness accordingly.
			public Pattern Mirror
			{
				get
				{
					var shape = new List<List<int>>();
					for (var i = 0; i < Shape.Count; i++)
					{
						var tmp = new List<int>();
						for (var j = 0; j < Shape[0].Count; j++)
						{
							if (Shape[i][j] == 1)
								tmp.Add(2);
							else
								tmp.Add(Shape[i][j]);
						}
						shape.Add(tmp);
					}
					return new Pattern(Type, StateID + 1, Severity, -Effectiveness) { Shape = shape, PlayerID = 1, Parallax = Parallax };
				}
			}

			// Searches the '_case' for the current pattern.
			public bool Compare(ref List<List<int>> _case)
			{
				switch (Type)
				{
					case 's': return Compare_s(ref _case);
					case 'c': return Compare_c(ref _case);
					default:
						throw new NotImplementedException();
				}

			}

			// Situation Comparer.
			private bool Compare_s(ref List<List<int>> _case)
			{
				SituationPlays = new List<int>();
				for (var x = 1; x + Shape[0].Count <= 8; x++)
				{
					if (!Parallax)
					{
						MirrorShape();
						if (ComparePatern(x, 0, ref _case, ref Shape))
						{
							for (int i = 0; i < Shape[0].Count; i++)
							{
								if (Shape[0][i] == -2 && !SituationPlays.Contains(i + x))
									SituationPlays.Add(i + x);
							}
							return true;
						}
						MirrorShape();
					}
					if (ComparePatern(x, 0, ref _case, ref Shape))
					{
						for (int i = 0; i < Shape[0].Count; i++)
						{
							if (Shape[0][i] == -2 && !SituationPlays.Contains(i + x))
								SituationPlays.Add(i + x);
						}
						return true;
					}
				}
				return false;
			}

			// Combination Comparer.
			private bool Compare_c(ref List<List<int>> _case)
			{
				var X = Shape[0].Count;
				var Y = Shape.Count;
				List<List<int>> Flip;
				if (!Parallax)
				{
					MirrorShape();
					for (var i = 0; i < 4; i++)
					{
						Flip = FlipPatern(i);
						for (var x = 1; x < 8; x++)
						{
							for (var y = 1; y < 7; y++)
							{
								if (ComparePatern(x, y, ref _case, ref Flip))
									return true;
							}
						}
					}
					MirrorShape();
				}
				for (var i = 0; i < 4; i++)
				{
					Flip = FlipPatern(i);
					for (var x = 1; x < 8; x++)
					{
						for (var y = 1; y < 7; y++)
						{
							if (ComparePatern(x, y, ref _case, ref Flip))
								return true;
						}
					}
				}
				return false;
			}

			// Compares a specific point with the pattern with all the rotaion and flips.
			private bool ComparePatern(int x, int y, ref List<List<int>> _case, ref List<List<int>> source)
			{
				var X = source[0].Count;
				var Y = source.Count;
				if (x + X <= 8 && y + Y <= 7)
				{
					for (var i = 0; i < X; i++)
					{
						for (var j = 0; j < Y; j++)
						{
							if (source[j][i] != -1 && source[j][i] != -2 && source[j][i] != _case[y + j][x + i])
								return false;
						}
					}
					return true;
				}
				return false;
			}

			// Turns the current pattern x times.
			private List<List<int>> FlipPatern(int times, List<List<int>> source = null)
			{
				if (source == null) { source = Shape; }
				if (times == 0) return source;
				var output = new List<List<int>>();
				for (var x = 0; x < source[0].Count; x++)
				{
					var tmp = new List<int>();
					for (var y = source.Count - 1; y >= 0; y--)
					{
						tmp.Add(source[y][x]);
					}
					output.Add(tmp);
				}
				if (times == 1) return output;
				return FlipPatern(times - 1, output);
			}

			// Flips the pattern vertically.
			private void MirrorShape()
			{
				for (var i = 0; i < Shape.Count; i++)
					Shape[i].Reverse();
			}
		}
	}

	// Used to return multiple data types in the Check functions at the bottom.
	public class CheckData
	{
		public bool Check;
		public int Color;
		public List<Point> Points = new List<Point>();

		public CheckData(bool b, int col = 0, List<Point> P = null)
		{
			Check = b; Color = col; Points = P;
		}
	}

	public static class CaseExtensions
	{
		// Returns the lowest empty case in '_case'.
		public static int GetLow(this List<List<int>> _case, int col)
		{
			for (var i = 6; i > 0; i--)
				if (_case[i][col] == 0)
					return i;
			return 0;
		}

		// Returns if a column in '_case' is full or not.
		public static bool IsFull(this List<List<int>> _case, int col)
		{
			return _case[1][col] != 0;
		}

		// Creates a new List<List<int>> instance with the same values as 'From'.
		public static List<List<int>> CreateCopy(this List<List<int>> From)
		{
			var To = new List<List<int>>();
			for (var i = 0; i < 7; i++)
			{
				To.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 });
				for (var j = 0; j < 8; j++)
				{
					To[i][j] = From[i][j];
				}
			}
			return To;
		}

		// Various functions that check for a 4+ token chain if 'win' is true
		// and checks if a play in a column can create a 4 token chain.
		public static CheckData CheckHorizontal(this List<List<int>> checkCase, int x, int y)
		{
			if (x + 3 >= 1 && x + 3 <= 7)
			{
				if (checkCase[y][x] != 0 && checkCase[y][x] == checkCase[y][x + 1] && checkCase[y][x] == checkCase[y][x + 2] && checkCase[y][x] == checkCase[y][x + 3])
					return new CheckData(true, checkCase[y][x], new List<Point> { new Point(x, y), new Point(x + 1, y), new Point(x + 2, y), new Point(x + 3, y) });
			}
			return new CheckData(false);
		}

		public static CheckData CheckVertical(this List<List<int>> checkCase, int x, int y)
		{
			if (y + 3 >= 1 && y + 3 <= 6)
			{
				if (checkCase[y][x] != 0 && checkCase[y][x] == checkCase[y + 1][x] && checkCase[y][x] == checkCase[y + 2][x] && checkCase[y][x] == checkCase[y + 3][x])
					return new CheckData(true, checkCase[y][x], new List<Point> { new Point(x, y), new Point(x, y + 1), new Point(x, y + 2), new Point(x, y + 3) });
			}
			return new CheckData(false);
		}

		public static CheckData CheckDiagDown(this List<List<int>> checkCase, int x, int y)
		{
			if ((y + 3 >= 1 && y + 3 <= 6) && (x + 3 >= 1 && x + 3 <= 7))
			{
				if (checkCase[y][x] != 0 && checkCase[y][x] == checkCase[y + 1][x + 1] && checkCase[y][x] == checkCase[y + 2][x + 2] && checkCase[y][x] == checkCase[y + 3][x + 3])
					return new CheckData(true, checkCase[y][x], new List<Point> { new Point(x, y), new Point(x + 1, y + 1), new Point(x + 2, y + 2), new Point(x + 3, y + 3) });
			}
			return new CheckData(false);
		}

		public static CheckData CheckDiagUp(this List<List<int>> checkCase, int x, int y)
		{
			if ((y - 3 >= 1 && y - 3 <= 6) && (x + 3 >= 1 && x + 3 <= 7))
			{
				if (checkCase[y][x] != 0 && checkCase[y][x] == checkCase[y - 1][x + 1] && checkCase[y][x] == checkCase[y - 2][x + 2] && checkCase[y][x] == checkCase[y - 3][x + 3])
					return new CheckData(true, checkCase[y][x], new List<Point> { new Point(x, y), new Point(x + 1, y - 1), new Point(x + 2, y - 2), new Point(x + 3, y - 3) });
			}
			return new CheckData(false);
		}
	}
}
