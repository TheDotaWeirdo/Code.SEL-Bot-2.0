using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Code.SEL_Bot_2._0
{
	public class Encrypter
	{
		public static string Encrypt(string S, string Key)
		{
			if (S.Length > 0 && Key.Length >= 5 && char.IsDigit(Key[1]) && char.IsDigit(Key[2])
				&& char.IsDigit(Key[3]) && char.IsDigit(Key[4]) && Key[1] > '0'
				&& Key[2] > '1' && Key[3] > '2' && Key[4] < '4' && Key[4] > '0')
			{
				var Out = Pivote(S, Key.ToLower()[0]);
				Out = ScrambleCase(Out, int.Parse(Key.Substring(1, 1)));
				Out = ChaoticChar(Out, int.Parse(Key.Substring(2, 1)));
				Out = Clipper(Out, int.Parse(Key.Substring(3, 1)));
				Out = AverageOut(Out, int.Parse(Key.Substring(4, 1)));
				return '`' + Out + '`';
			}
			return (S.Length > 0) ? "Key is not Valid\n`Syntax Error`" : "Text is too short";
		}

		private static string Pivote(string S, char Key)
		{
			var s = "";
			for (int i = 0; i < S.Length; i++)
			{
				if (S.ToLower()[i] == Key)
					S = S.Substring(0, i) + S.Substring(i).Flip();
				s += S[i];
			}
			return s;
		}

		private static string ScrambleCase(string S, int Key)
		{
			var s = ((char.IsLower(S[0])) ? S.ToUpper()[0] : S.ToLower()[0]).ToString();
			var Change = Key;
			for (int i = 1; i < S.Length; i++)
			{
				if (i == Change)
				{
					s += ((char.IsLower(S[i])) ? S.ToUpper()[i] : S.ToLower()[i]).ToString();
					Change += Key;
				}
				else
					s += S[i];
			}
			return s;
		}

		private static string ChaoticChar(string S, int Key)
		{
			var s = "";
			var j = 0;
			for (int i = 0; i < S.Length; i++)
			{
				j = Convert.ToInt32(S[i]);
				s += Convert.ToChar(j - Key / 2);
				s += Convert.ToChar(j - Key);
				s += Convert.ToChar(j + Key);
				s += Convert.ToChar(j + Key / 2);
			}
			return s;
		}

		private static string Clipper(string S, int Key)
		{
			var s = "";
			for (int i = 0; i < S.Length; i++)
			{
				if (i % Key != 0)
					s += S[i];
			}
			return s;
		}

		private static string AverageOut(string S, int Key)
		{
			var s = ""; if (S.Length < Key) return S;
			for (int i = 0; true; i += Key)
			{
				if (i + Key < S.Length)
				{
					var j = 0;
					for (int k = i; k < i + Key; k++)
					{
						j += Convert.ToInt32(S[k]);
					}
					s += Convert.ToChar(j / Key);
				}
				else
				{
					s += S.Substring(i);
					break;
				}
			}
			return s;
		}
	}

	public static class Extensions
	{
		public static string Flip(this string S)
		{
			var s = "";
			foreach (var c in S)
				s = c + s;
			return s;
		}
	}
}
