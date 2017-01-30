using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace CloudSoft.Extensions
{
	public static class GZipExtensions
	{
		public static string GZipToBase64String(this byte[] buffer)
		{
			string result = null;
			using (var ms = new System.IO.MemoryStream())
			{
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

		public static byte[] UnGzipFromBase64(this string input)
		{
			var file = Convert.FromBase64String(input);
			int blockSize = 512;
			byte[] fileData = null;

			using (var compressedStream = new System.IO.MemoryStream(file, false))
			{
				if (compressedStream.CanSeek)
				{
					compressedStream.Seek(0, System.IO.SeekOrigin.Begin);
				}
				using (var uncompressedStream = new System.IO.MemoryStream())
				{
					using (var unzip = new System.IO.Compression.GZipStream(compressedStream, System.IO.Compression.CompressionMode.Decompress))
					{
						var bf = new byte[blockSize];
						while (true)
						{
							// Bug ! if zippedbuffer smaller than 4096 bytes, read byte one by one
							if (file.Length <= 4096)
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
						//result = System.Text.Encoding.UTF8.GetString(uncompressedStream.ToArray());
						fileData = uncompressedStream.ToArray();
						unzip.Close();
					}
					uncompressedStream.Close();
				}
				compressedStream.Close();
			}

			return fileData;
		}
	}
}
