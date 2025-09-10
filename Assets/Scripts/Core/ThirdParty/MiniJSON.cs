using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ARLinguaSphere.Core.ThirdParty
{
	/// <summary>
	/// Minimal JSON serializer/deserializer (Unity-friendly). Based on Prime31/MiniJSON-style API.
	/// </summary>
	public static class MiniJSON
	{
		public static object Deserialize(string json)
		{
			if (string.IsNullOrEmpty(json)) return null;
			return new Parser(json).ParseValue();
		}

		private class Parser
		{
			private readonly StringReader _reader;

			public Parser(string json)
			{
				_reader = new StringReader(json);
			}

			public object ParseValue()
			{
				EatWhitespace();
				int c = PeekChar();
				if (c == -1) return null;
				char ch = (char)c;
				switch (ch)
				{
					case '{': return ParseObject();
					case '[': return ParseArray();
					case '"': return ParseString();
					case 't': return ParseLiteral("true", true);
					case 'f': return ParseLiteral("false", false);
					case 'n': return ParseLiteral("null", null);
					default: return ParseNumber();
				}
			}

			private Dictionary<string, object> ParseObject()
			{
				// consume '{'
				ReadChar();
				var table = new Dictionary<string, object>();
				EatWhitespace();
				if (PeekChar() == '}') { ReadChar(); return table; }
				while (true)
				{
					EatWhitespace();
					string key = ParseString();
					EatWhitespace();
					// ':'
					ReadChar();
					object value = ParseValue();
					table[key] = value;
					EatWhitespace();
					int c = ReadChar();
					if (c == '}') break;
					// expect ',' then continue
				}
				return table;
			}

			private List<object> ParseArray()
			{
				ReadChar();
				var array = new List<object>();
				EatWhitespace();
				if (PeekChar() == ']') { ReadChar(); return array; }
				while (true)
				{
					object value = ParseValue();
					array.Add(value);
					EatWhitespace();
					int c = ReadChar();
					if (c == ']') break;
				}
				return array;
			}

			private string ParseString()
			{
				var sb = new StringBuilder();
				// opening '"'
				ReadChar();
				while (true)
				{
					int c = ReadChar();
					if (c == -1) break;
					char ch = (char)c;
					if (ch == '"') break;
					if (ch == '\\')
					{
						char esc = (char)ReadChar();
						switch (esc)
						{
							case '"': sb.Append('"'); break;
							case '\\': sb.Append('\\'); break;
							case '/': sb.Append('/'); break;
							case 'b': sb.Append('\b'); break;
							case 'f': sb.Append('\f'); break;
							case 'n': sb.Append('\n'); break;
							case 'r': sb.Append('\r'); break;
							case 't': sb.Append('\t'); break;
							case 'u':
								var hex = new char[4];
								_reader.Read(hex, 0, 4);
								ushort codePoint = ushort.Parse(new string(hex), NumberStyles.HexNumber);
								sb.Append((char)codePoint);
								break;
							default:
								sb.Append(esc);
								break;
						}
					}
					else
					{
						sb.Append(ch);
					}
				}
				return sb.ToString();
			}

			private object ParseNumber()
			{
				var sb = new StringBuilder();
				int c = PeekChar();
				while (c != -1 && "-+0123456789.eE".IndexOf((char)c) != -1)
				{
					sb.Append((char)ReadChar());
					c = PeekChar();
				}
				string num = sb.ToString();
				if (num.IndexOf('.') != -1 || num.IndexOf('e') != -1 || num.IndexOf('E') != -1)
				{
					if (double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out double d)) return d;
				}
				else
				{
					if (long.TryParse(num, NumberStyles.Integer, CultureInfo.InvariantCulture, out long l)) return l;
				}
				return 0;
			}

			private object ParseLiteral(string literal, object value)
			{
				for (int i = 0; i < literal.Length; i++) ReadChar();
				return value;
			}

			private void EatWhitespace()
			{
				while (true)
				{
					int c = PeekChar();
					if (c == -1) return;
					char ch = (char)c;
					if (!char.IsWhiteSpace(ch)) return;
					ReadChar();
				}
			}

			private int PeekChar() => _reader.Peek();
			private int ReadChar() => _reader.Read();
		}
	}
}


