using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace Code.SEL_Bot_2._0
{
	public partial class MainForm
	{
		CodeSelBot MyBot;
		Thread BotThread;
		public static string ConnectionState = "Disconnected";
		public static bool Listening = false;
		public static bool SaveData = true;
		public static Label Console;
		private delegate void ConsoleUpdate(string S);
		private static ConsoleUpdate CUpdte = new ConsoleUpdate(WriteConsole);
		private Dictionary<string, Color> ServerStatus = new Dictionary<string, Color>
		{
			{ "Connected", Color.FromArgb(41, 198, 56) },
			{ "Connecting", Color.FromArgb(222, 184, 53) },
			{ "Disconnecting", Color.FromArgb(222, 184, 53) },
			{ "Connection Error", Color.FromArgb(222, 184, 53) },
			{ "Disconnected", Color.FromArgb(195, 46, 43) }
		};
	}
}
