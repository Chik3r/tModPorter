using System;
using System.IO;
using System.Text;

namespace tModPorter
{
    public static class EncodingUtils
    {
        // from: https://stackoverflow.com/a/40511863/8360707
        /// <summary>
        /// UTF8    : EF BB BF
        /// UTF16 BE: FE FF
        /// UTF16 LE: FF FE
        /// UTF32 BE: 00 00 FE FF
        /// UTF32 LE: FF FE 00 00
        /// </summary>
        public static Encoding DetectEncoding(Stream i_Stream)
        {
            if (!i_Stream.CanSeek || !i_Stream.CanRead)
                throw new Exception("DetectEncoding() requires a seekable and readable Stream");

            // Try to read 4 bytes. If the stream is shorter, less bytes will be read.
            byte[] u8_Buf = new byte[4];
            int s32_Count = i_Stream.Read(u8_Buf, 0, 4);
            if (s32_Count >= 2)
            {
                if (u8_Buf[0] == 0xFE && u8_Buf[1] == 0xFF)
                {
                    i_Stream.Position = 2;
                    return new UnicodeEncoding(true, true);
                }

                if (u8_Buf[0] == 0xFF && u8_Buf[1] == 0xFE)
                {
                    if (s32_Count >= 4 && u8_Buf[2] == 0 && u8_Buf[3] == 0)
                    {
                        i_Stream.Position = 4;
                        return new UTF32Encoding(false, true);
                    }
                    else
                    {
                        i_Stream.Position = 2;
                        return new UnicodeEncoding(false, true);
                    }
                }

                if (s32_Count >= 3 && u8_Buf[0] == 0xEF && u8_Buf[1] == 0xBB && u8_Buf[2] == 0xBF)
                {
                    i_Stream.Position = 3;
                    return Encoding.UTF8;
                }

                if (s32_Count >= 4 && u8_Buf[0] == 0 && u8_Buf[1] == 0 && u8_Buf[2] == 0xFE && u8_Buf[3] == 0xFF)
                {
                    i_Stream.Position = 4;
                    return new UTF32Encoding(true, true);
                }
            }

            i_Stream.Position = 0;
            return Encoding.Default;
        }
    }
}