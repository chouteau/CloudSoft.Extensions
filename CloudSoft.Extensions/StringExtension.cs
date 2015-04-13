using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;
using System.Collections.Specialized;

namespace CloudSoft.Extensions
{
	/// <summary>
	/// Quelques methodes d'extension du type String
	/// </summary>
	public static class StringExtension
	{
		/// <summary>
		/// Determines whether [is null or trimmed empty] [the specified value].
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if [is null or trimmed empty] [the specified value]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullOrTrimmedEmpty(this string value)
		{
			return (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value.Trim()));
		}

		public static string DefaultValueIfNullOrTrimmedEmpty(this string value, string defaultValue)
		{
			if (!value.IsNullOrTrimmedEmpty())
			{
				return value;
			}
			return defaultValue;
		}

		public static string StringEmptyIfNull(this string value)
		{
			if (value == null)
			{
				return string.Empty;
			}
			return value;
		}

		/// <summary>
		/// Lefts the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="length">The length.</param>
		/// <returns></returns>
		public static string Left(this string value, int length)
		{
			if (value.IsNullOrTrimmedEmpty())
			{
				return value;
			}
			return value.Substring(0, Math.Min(length, value.Length));
		}

		/// <summary>
		/// Rights the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="length">The length.</param>
		/// <returns></returns>
		public static string Right(this string value, int length)
		{
			if (value.IsNullOrTrimmedEmpty())
			{
				return value;
			}
			return value.Substring(Math.Min(Math.Max(value.Length - length,0), value.Length));
		}

		/// <summary>
		/// Simplify the String.Format usage, example: "Test {0}".With(DateTime.Now)
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static string With(this string format, params object[] args)
		{
			return string.Format(format, args);
		}

		public static bool IsMatch(this string value, string pattern)
		{
			return  !string.IsNullOrEmpty(value) && Regex.IsMatch(value, pattern);
		}

		public static bool IsInteger(this string value)
		{
			return IsMatch(value, @"^[\d]{1,}$");
		}

		public static string Replace(this string value, string pattern, string replacement)
		{
			return Regex.Replace(value, pattern, replacement);
		}

		public static string Reverse(this string input)
		{
			char[] chars = input.ToCharArray();
			Array.Reverse(chars);
			return new String(chars);
		}

		public static int LineCount(this string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return 0;
			}
			Regex RE = new Regex("\n", RegexOptions.Multiline);
			MatchCollection theMatches = RE.Matches(text);
			return (theMatches.Count + 1);
		}

		public static string ToFixedLength(this string input, int length)
		{
			return ToFixedLength(input, length, ' ');
		}

		public static string ToFixedLength(this string input, int length, char paddingChar)
		{
			string result = string.Empty;

			if (string.IsNullOrEmpty(input))
			{
				result = new string(paddingChar, length);
			}
			else if (input.Length > length)
			{
				result = input.Substring(0, length);
			}
			else
			{
				//result = input;
				result = input.PadRight(length, paddingChar);
			}

			return result;
		}

		

		public static string ToMaxLength(this string input, int maxLength)
		{
			string result = string.Empty;

			if (string.IsNullOrEmpty(input))
			{
				result = string.Empty;
			}
			else if (input.Length > maxLength)
			{
				result = input.Substring(0, maxLength);
			}
			else
			{
				result = input;
			}

			return result;
		}

		/// <summary>
		/// zip the base64 input string to string.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		public static string GZipToBase64String(this string input)
		{
			string result = null;
			using (var ms = new System.IO.MemoryStream())
			{
				var buffer = System.Text.Encoding.UTF8.GetBytes(input);
				using (var zip = new System.IO.Compression.GZipStream(ms, CompressionMode.Compress, true))
				{
					zip.Write(buffer, 0, buffer.Length);
					zip.Close();
				}
				result = Convert.ToBase64String(ms.ToArray());
				ms.Close();
			}
			return result;
		}

		/// <summary>
		/// UnZip zippedBuffer
		/// </summary>
		/// <param name="zippedbuffer">The zippedbuffer.</param>
		/// <returns></returns>
		public static string UnGZip(this byte[] zippedbuffer)
		{
			string result = null;
			int blockSize = 512;
			using (var compressedStream = new System.IO.MemoryStream(zippedbuffer, false))
			{
				if (compressedStream.CanSeek)
				{
					compressedStream.Seek(0, SeekOrigin.Begin);
				}
				using (var uncompressedStream = new System.IO.MemoryStream())
				{
					using (var unzip = new System.IO.Compression.GZipStream(compressedStream, CompressionMode.Decompress))
					{
						var bf = new byte[blockSize];
						while (true)
						{
							// Bug ! if zippedbuffer smaller than 4096 bytes, read byte one by one
							if (zippedbuffer.Length <= 4096)
							{
								var pos = unzip.ReadByte();
								if (pos == -1)
								{
									break;
								}
								uncompressedStream.WriteByte((byte)pos);
							}
							else
							{
								var count = unzip.Read(bf, 0, blockSize);
								if (count == 0)
								{
									break;
								}
								uncompressedStream.Write(bf, 0, count);
							}
						}
						result = System.Text.Encoding.UTF8.GetString(uncompressedStream.ToArray());
						unzip.Close();
					}
					uncompressedStream.Close();
				}
				compressedStream.Close();
			}
			return result;
		}

		/// <summary>
		/// unzip base64 zipped string.
		/// </summary>
		/// <param name="zippedinput">The zippedinput.</param>
		/// <returns></returns>
		public static string UnGZipFromBase64String(this string zippedinput)
		{
			var buffer = Convert.FromBase64String(zippedinput);
			return buffer.UnGZip();
		}

		/// <summary>
		/// Converti les paramètres d'une url en HashTable key/value
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns></returns>
		public static NameValueCollection ToNameValueDictionary(this string query)
		{
			if (query.IsNullOrTrimmedEmpty() || query.IndexOf("?") == -1)
			{
				return new System.Collections.Specialized.NameValueCollection();
			}
			var parameters = query.Split('?')[1].Split('&');
			var result = new NameValueCollection();
			foreach (var item in parameters)
			{
				var tokens = item.Split('=');
				if (tokens.Count() == 2)
				{
					result.Add(tokens[0], tokens[1]);
				}
			}
			return result;
		}

		public static string EllipsisAt(this string input, int length, string continuousHref = null)
		{
			if (input.IsNullOrEmpty()
				|| input.Length <= length)
			{
				return input;
			}
			var result = input.Substring(0, Math.Max(1, length - 4));
			var lastSpace = result.LastIndexOf(' ');
			if (lastSpace == -1)
			{
				lastSpace = Math.Min(length, result.Length);
			}
			var continuous = " ...";
			return result.Substring(0, lastSpace) + continuous;
		}

		/// <summary>
		/// Retourne la phrase avec le nombre de mots indiqué + ... si le texte etait plus long
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="wordCount">The word count.</param>
		/// <returns></returns>
		public static string EllipsisWordsAt(this string input, int wordCount)
		{
			if (input.IsNullOrEmpty())
			{
				return input;
			}

			var wordList = input.ToWordList();
			var result = new List<string>();
			while (true)
			{
				var word = wordList.FirstOrDefault();
				if (word == null)
				{
					break;
				}
				if (wordCount == 0)
				{
					result.Add("...");
					break;
				}
				result.Add(word);
				wordList.Remove(word);
				wordCount--;
			}

			return result.JoinString(" ");
		}

		/// <summary>
		/// Retourne la chaine de caractère en limitant le nombre de lignes
		/// </summary>
		/// <param name="input"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static string TakeLines(this string input, int count)
		{
			if (input.IsNullOrTrimmedEmpty())
			{
				return input;
			}

			var lines = input.Split('\n');
			var result = new StringBuilder();
			for (int i = 0; i < count; i++)
			{
				if (i == lines.Length)
				{
					break;
				}
				var line = lines[i].Trim('\r');
				if (line.IsNullOrTrimmedEmpty())
				{
					count++;
					continue;
				}
				result.AppendLine(line);
			}

			return result.ToString();
		}

		public static string[] Words(this string input)
		{
			// Suppression des doubles espaces
			var text = input.Trim();

			var result = input.Split(' ');
			result.RemoveAll(i => i == " ");
			return result;
		}

		public static string CapitalizeWords(this string phrase)
		{
			var words = phrase.ToLower().ToWordList();
			string result = null;
			foreach (var word in words)
			{
				var f = word[0];
				var capitalizedWord = word;
				if (char.IsLetter(f))
				{
					capitalizedWord = char.ToUpper(f) + word.Substring(1);
				}
				result += capitalizedWord + " ";
			}

			return result.Trim();
		}

		public static string ToHtmlAttribute(this string input)
		{
			if (input == null)
			{
				return null;
			}
			input = input.Replace("\"", "&#34;");
			input = input.Replace(System.Environment.NewLine, "");
			return input;
		}

		public static string RemoveIllegalXmlChar(this string input)
		{
			if (input == null)
			{
				return input;
			}
			string result = string.Empty;
			for (int i = 0; i < input.Length; i++)
			{
				int character = input[i];
				if (character == 0x9 /* == '\t' == 9   */          
					|| character == 0xA /* == '\n' == 10  */         
					|| character == 0xD /* == '\r' == 13  */          
					|| (character >= 0x20 && character <= 0xD7FF) 
					|| (character >= 0xE000 && character <= 0xFFFD) 
					|| (character >= 0x10000 && character <= 0x10FFFF)
					)
				{
					result += (char)character;
				}
			}
			return result;
		}

		public static List<string> GetWordList(this string value)
		{
			if (value.IsNullOrTrimmedEmpty())
			{
				return new List<string>();
			}

			// Suppression des espaces avant/après
			var workingValue = value.Trim();
			var result = new List<string>();
			// Suppression des doubles espaces

			if (value.IndexOf(@"\""") == -1)
			{
				// Recupération des expression entre guillemets
				var reg = new System.Text.RegularExpressions.Regex(@"""(?<keyword>[^""]*?)""", System.Text.RegularExpressions.RegexOptions.Singleline);
				var kwid = 0;
				var phraseList = new List<string>();
				foreach (System.Text.RegularExpressions.Match item in reg.Matches(workingValue))
				{
					var kw = item.Groups["keyword"].Value;
					phraseList.Add(kw);
					workingValue = workingValue.Replace("\"" + kw + "\"", string.Format("[{0}]", kwid));
					kwid++;
				}
				result = workingValue.Split(' ').ToList();

				for (int i = 0; i < phraseList.Count; i++)
				{
					var phrase = phraseList[i];
					if (phrase == null)
					{
						break;
					}
					var pattern = string.Format("[{0}]", i);
					var item = result.FirstOrDefault(kw => kw.Equals(pattern));
					if (item != null)
					{
						var itemId = result.IndexOf(item);

						item = item.Replace(item, "\"" + phrase + "\"");
						result[itemId] = item;
					}
				}
			}
			else
			{
				result = workingValue.Split(' ').ToList();
			}

			// suppression des espaces seuls
			result.RemoveAll(i => i == " ");
			return result;
		}

		public static string AccentLess(this string input)
		{
			string accent = "ŠŒŽšœžŸ¢µÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝàáâãäåæçèéêëìíîïðñòóôõöùúûüý";
			string accentless = "SOZsozYcuAAAAAAACEEEEIIIIDNOOOOOxOUUUUYaaaaaaaceeeeiiiionooooouuuuy";

			for (int i = 0; i < accent.Length; i++)
			{
				if (input.IndexOf(accent[i]) > -1)
				{
					input = input.Replace(accent[i], accentless[i]);
				}
			}

			return input;
		}

		public static List<string> ToWordList(this string value)
		{
			if (value.IsNullOrTrimmedEmpty())
			{
				return new List<string>();
			}

			// Suppression des espaces avant/après
			var workingValue = value.Trim();
			var result = new List<string>();

			// Suppression des doubles espaces
			while (workingValue.IndexOf("  ") != -1)
			{
				workingValue = workingValue.Replace("  ", " ");
			}
			result = workingValue.Split(' ').ToList();
			return result;
		}

		public static string Cleanup(this string input)
		{
			if (input == null)
			{
				return null;
			}
			return input.Trim();
		}

		/// <summary>
		/// Nettoyage d'un numéro de téléphone
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string CleanupPhoneNumber(this string input)
		{
			if (input == null)
			{
				return input;
			}
			input = input.Replace(".", "");
			return input;
		}

		/// <summary>
		/// Indique si 2 chaines de caractères sont identiques in ignorant les majuscules
		/// </summary>
		/// <param name="input"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IgnoreCaseEquals(this string input, string value)
		{
			return input.Equals(value, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
