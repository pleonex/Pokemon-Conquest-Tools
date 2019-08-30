// Copyright (c) 2019 Benito Palacios Sanchez
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace AmbitionConquest.Texts
{
    using System;
    using System.Text;
    using Yarhl.IO;

    public class PnaTextWriter
    {
        static readonly Encoding ShiftJis;
        readonly DataWriter writer;

        string message;
        int pos;
        bool kanjiSingleByteMode;
        bool furiganaMode;

        static PnaTextWriter()
        {
            // Make sure that the shift-jis encoding is initialized in
            // .NET Core
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ShiftJis = Encoding.GetEncoding("shift_jis");
        }

        public PnaTextWriter(DataStream stream)
        {
            writer = new DataWriter(stream);
        }

        public void WriteMessage(Message msg)
        {
            Reset(msg);

            if (message == "{close}") {
                writer.Write(0x00);
                return;
            }

            while (pos < message.Length) {
                char ch = message[pos++];
                byte[] data = GetCharBytes(ch);

                if (data.Length == 0) {
                    continue;
                } else if (ch == '{' || (ch == '[' || ch == ']')) {
                    WriteControlCode();
                } else if (data[0] < 0xA6 || data[0] > 0xDF) {
                    WriteAscii(data);
                } else {
                    WriteShiftJis(data);
                }
            }

            // End of segment.
            WriteSjisEndToken();
            writer.Write(0x05);
            writer.Write(0x05);
            writer.Write(0x05);
        }

        void Reset(Message msg)
        {
            pos = 0;
            message = msg.Context + msg.BoxConfig + msg.BoxConfig;

            kanjiSingleByteMode = false;
            // kanjiPage2Mode = false;
            furiganaMode = false;
        }

        byte[] GetCharBytes(char ch)
        {
            // Ignore Windows new lines
            if (ch == '\r') {
                return Array.Empty<byte>();
            }

            // Fast return
            if (ch <= 0x7F) {
                return new byte[] { (byte)ch };
            }

            // Encode with ShiftJis BigEndian
            byte[] encoded = ShiftJis.GetBytes(new[] { ch });
            byte tmp = encoded[0];
            encoded[0] = encoded[1];
            encoded[1] = tmp;

            // Return only the last byte if the first one is 0x00
            return (encoded[0] == 0) ? new[] { encoded[1] } : encoded;
        }

        void WriteAscii(byte[] ch)
        {
            if ((ch[0] >= 0x81 && ch[0] <= 0x9F) || (ch[0] >= 0xE0 && ch[0] <= 0xFC)) {
                // It's a single byte kanji
                writer.Write(ch);
            } else {
                // It's English char (ASCII)
                writer.Write(ch[0]);
            }
        }

        void WriteShiftJis(byte[] ch)
        {
            if (ch.Length == 1) {
                kanjiSingleByteMode = true;
                writer.Write(0x1B);
                writer.Write(0x6B);

                writer.Write(ch[0]);
                return;
            }

            if (ch[0] == 0x82) {
                WriteSjisEndToken();
                if (ch[1] >= 0xDE) {
                    ch[1]++;
                }

                ch[1] -= 0x5F;
            } else if (ch[0] == 0x83) {
                WriteSJisStartToken();
            }

            if (ch[1] == 0x94) {
                writer.Write(0xB3);
                writer.Write(0xDE);
                return;
            }

            // TODO
        }

        void WriteControlCode()
        {

        }

        void WriteSJisStartToken()
        {
            if (!kanjiSingleByteMode) {
                writer.Write(0x1B);
                writer.Write(0x4B);
            }

            kanjiSingleByteMode = true;
        }

        void WriteSjisEndToken()
        {
            if (kanjiSingleByteMode) {
                writer.Write(0x1B);
                writer.Write(0x48);
            }

            kanjiSingleByteMode = false;
        }
    }
}