using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Code.SEL_Bot_2._0
{
	public partial class MainForm
	{
		public static Data BotData;

		public static void LoadBotData()
		{
			string file = "../../Data/Data.xml";
			XmlSerializer formatter = new XmlSerializer(new Data().GetType());
			FileStream DataFile = new FileStream(file, FileMode.Open);
			byte[] buffer = new byte[DataFile.Length];
			DataFile.Read(buffer, 0, (int)DataFile.Length);
			MemoryStream stream = new MemoryStream(buffer);
			BotData = (Data)formatter.Deserialize(stream);
		}

		public static void SaveBotData()
		{
			string path = "../../Data/Data.xml";
			FileStream outFile = File.Create(path);
			XmlSerializer formatter = new XmlSerializer(new Data().GetType());
			formatter.Serialize(outFile, BotData);
		}

		public class Data
		{
			public int CommandsProccessed = 0;
			public long UpTime = 0;
		}
	}
}
