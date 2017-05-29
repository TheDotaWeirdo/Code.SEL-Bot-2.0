using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Code.SEL_Bot_2._0
{
	public partial class MainForm : Form
	{
		public delegate void ForceDisconnect(object sender, EventArgs e);

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			Console = Cons;
			LoadUserData();
			LoadBotData();
			LoadConnect4Data();
			Connect4Games = new List<MainGame>();
			MainForm_Resize(null, null);
			var T1 = new System.Timers. Timer(1000);
			T1.Start();
			T1.Elapsed += (s, t) =>
			{
				T1.Dispose();
				MyBot = new CodeSelBot();
			};
			var T2 = new System.Timers.Timer(10);
			T2.Start();
			T2.Elapsed += (s, t) =>
			{
				try
				{
					if (Cons.Text.EndsWith("Log: Disconnected\n"))
						ConnectionState = "Disconnected";
					else if (Cons.Text.EndsWith("Log: Unknown Opcode: 9\n"))
						ConnectionState = "Connection Error";
					else if (CodeSelBot.discord != null)
						ConnectionState = CodeSelBot.discord.State.ToString();

					try
					{
						pictureBox_ServerStatus.Invoke(new Action(UpdateServerAvailability_PictureBox));
						label_ServerStatus.Invoke(new Action(UpdateServerAvailability_Label));
					}
					catch (Exception) { }
				}
				catch (Win32Exception) { }
			};
#if !DEBUG
			WindowState = FormWindowState.Minimized;
#endif
		}

		private void Start_Click(object sender, EventArgs e)
		{
			Stop_Click(null, null);
			BotThread = new Thread(new ThreadStart(RunBot))
			{ IsBackground = true, Priority = ThreadPriority.BelowNormal };
			BotThread.Start();
		}

		private void RunBot()
		{
			CodeSelBot.Connect();
		}

		private void Stop_Click(object sender, EventArgs e)
		{
			if (CodeSelBot.discord != null)
				CodeSelBot.discord.Disconnect();
		}

		private void UpdateServerAvailability_PictureBox()
		{
			try
			{
				pictureBox_ServerStatus.BackColor = ServerStatus[ConnectionState];
			}
			catch (Exception) { }
		}

		private void UpdateServerAvailability_Label()
		{
			try
			{
				label_ServerStatus.BackColor = ServerStatus[ConnectionState];
				label_ServerStatus.Text = ConnectionState;
				label_ServerStatus.Location = new Point(Width - 27 - label_ServerStatus.Width, 15 + Cons.Height);
			}
			catch (Exception) { }
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			CodeSelBot.discord.Disconnect();
			UserDatas.ClearVolatiles();
			notifyIcon.Visible = false;
			if (SaveData)
			{
				SaveUserData();
				SaveBotData();
				SaveConnect4Data();
			}
			Thread.Sleep(1000);
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{

			if (FormWindowState.Minimized == WindowState)
			{
#if !DEBUG
				notifyIcon.Visible = true;
				Hide();
#endif
				return;
			}
#if !DEBUG
			else if (FormWindowState.Normal == WindowState)
			{
				notifyIcon.Visible = false;
				Show();
			}
#endif

			Cons.Size = new Size(Width - 40, 110 + Height / 5);
			pictureBox_ServerStatus.Location = new Point(13, 12 + Cons.Height);
			pictureBox_ServerStatus.Width = Width - 40;
			label_ServerStatus.Location = new Point(Width - 27 - label_ServerStatus.Width, 15 + Cons.Height);
			pictureBox_Start.Location = new Point(15, pictureBox_ServerStatus.Location.Y + 20);
			pictureBox_Stop.Location = new Point(80, pictureBox_ServerStatus.Location.Y + 20);
			pictureBox_SaveData.Location = new Point(145, pictureBox_ServerStatus.Location.Y + 20);
			pictureBox_Listen.Location = new Point(200, pictureBox_ServerStatus.Location.Y + 20);
		}

		private void Listen_Click(object sender, EventArgs e)
		{
			Listening = !Listening;
			pictureBox_Listen.Image = (Listening) ? Properties.Resources.Listen_On : Properties.Resources.Listen_Off;
		}

		private void SaveData_Click(object sender, EventArgs e)
		{
			SaveData = !SaveData;
			pictureBox_SaveData.Image = (SaveData) ? Properties.Resources.SaveData : Properties.Resources.SaveData_No;
		}

		private void Restore_Click(object sender, EventArgs e)
		{
			notifyIcon.Visible = false;
			Show();
			WindowState = FormWindowState.Normal;
		}
	}
}
