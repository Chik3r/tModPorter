using System;
using System.IO;
using System.Text;

namespace tModPorter
{
	public static class EncodingUtils
	{
		// from: https://stackoverflow.com/a/40511863/8360707
		/// <summary>
		///     UTF8    : EF BB BF
		///     UTF16 BE: FE FF
		///     UTF16 LE: FF FE
		///     UTF32 BE: 00 00 FE FF
		///     UTF32 LE: FF FE 00 00
		/// </summary>
		public static Encoding DetectEncoding(Stream iStream)
		{
			if (!iStream.CanSeek || !iStream.CanRead)
				throw new Exception("DetectEncoding() requires a seekable and readable Stream");

			// Try to read 4 bytes. If the stream is shorter, less bytes will be read.
			byte[] u8Buf = new byte[4];
			int s32Count = iStream.Read(u8Buf, 0, 4);
			if (s32Count >= 2) {
				if (u8Buf[0] == 0xFE && u8Buf[1] == 0xFF) {
					iStream.Position = 2;
					return new UnicodeEncoding(true, true);
				}

				if (u8Buf[0] == 0xFF && u8Buf[1] == 0xFE) {
					if (s32Count >= 4 && u8Buf[2] == 0 && u8Buf[3] == 0) {
						iStream.Position = 4;
						return new UTF32Encoding(false, true);
					}

					iStream.Position = 2;
					return new UnicodeEncoding(false, true);
				}

				if (s32Count >= 3 && u8Buf[0] == 0xEF && u8Buf[1] == 0xBB && u8Buf[2] == 0xBF) {
					iStream.Position = 3;
					return Encoding.UTF8;
				}

				if (s32Count >= 4 && u8Buf[0] == 0 && u8Buf[1] == 0 && u8Buf[2] == 0xFE && u8Buf[3] == 0xFF) {
					iStream.Position = 4;
					return new UTF32Encoding(true, true);
				}
			}

			iStream.Position = 0;
			return Encoding.Default;
		}
	}
}