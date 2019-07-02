//
//  Msg2Bin.cs
//
//  Author:
//       Benito Palacios Sanchez <benito356@gmail.com>
//
//  Copyright (c) 2018 Benito Palacios Sanchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace AmbitionConquest.Texts
{
    using System;
    using System.Text;
    using Mono.Addins;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    [Extension]
    public class Msg2Bin : IConverter<BinaryFormat, MessageBlock>
    {
        static readonly byte[] KanjiTable = {
            0x92, 0x40, 0x42, 0x44, 0x46, 0x48, 0x83, 0x85, 0x87, 0x62,
            0x00, 0x41, 0x43, 0x45, 0x47, 0x49, 0x4A, 0x4C, 0x4E, 0x50,
            0x52, 0x54, 0x56, 0x58, 0x5A, 0x5C, 0x5E, 0x60, 0x63, 0x65,
            0x67, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x71, 0x74, 0x77,
            0x7A, 0x7D, 0x7E, 0x80, 0x81, 0x82, 0x84, 0x86, 0x88, 0x89,
            0x8A, 0x8B, 0x8C, 0x8D, 0x8F, 0x93
        };

        bool kanjiSingleByteMode;
        bool kanjiPage2Mode;
        bool furiganaMode;

        DataReader reader;

        public MessageBlock Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            
            reader = new DataReader(source.Stream);
            source.Stream.Position = 0;

            MessageBlock messageBlock = new MessageBlock();
            uint numTexts = reader.ReadUInt32();
            for (int i = 0; i < numTexts; i++) {
                uint offset = reader.ReadUInt32();
                uint nextOffset;
                if (i < numTexts - 1) {
                    nextOffset = reader.ReadUInt32();
                    source.Stream.Position -= 4;
                } else {
                    nextOffset = (uint)source.Stream.Length;
                }

                source.Stream.PushToPosition(offset);
                Message msg = ReadMessage(nextOffset);
                msg.ID = i;
                messageBlock.Messages.Add(msg);
                source.Stream.PopPosition();
            }

            return messageBlock;
        }

        static string GetJapaneseString(params byte[] data)
        {
            return Encoding.GetEncoding(932).GetString(data);
        }

        Message ReadMessage(uint nextOffset)
        {
            Message msg = new Message();

            // Reset the state
            kanjiSingleByteMode = false;
            kanjiPage2Mode = false;
            furiganaMode = false;

            while (reader.Stream.Position < nextOffset) {
                byte data = reader.ReadByte();

                if (data < 0x20 && data != 0x0A)
                    ParseControlCode(data, msg);
                else if (data < 0xA6 || data > 0xDF)
                    ParseAsciiText(data, msg);
                else
                    ParseJapaneseText(data, msg);
            }

            return msg;
        }

        void ParseAsciiText(byte ch, Message msg)
        {
            if ((ch >= 0x81 && ch <= 0x9F) || (ch >= 0xE0 && ch <= 0xFC)) {
                // Japanese kanji
                byte ch2 = reader.ReadByte();
                msg.Text.Append(GetJapaneseString(ch, ch2));
            } else {
                // ASCII
                msg.Text.Append((char)ch);
            }
        }

        void ParseJapaneseText(byte ch, Message msg)
        {
            if (kanjiSingleByteMode) {
                msg.Text.Append(GetJapaneseString(ch));
                return;
            }

            byte firstByte = kanjiPage2Mode ? (byte)0x83 : (byte)0x82;
            byte secondByte = KanjiTable[ch - 0xA6];

            // There may be a second control byte
            byte control = reader.ReadByte();
            if (control == 0xDE) {
                secondByte = (ch == 0xB3) ? (byte)0x94 : (byte)(secondByte + 1);
            } else if (control == 0xDF) {
                secondByte += 2;
            } else {
                // Wrong data, go back
                reader.Stream.Position--;
            }

            if (!kanjiPage2Mode) {
                if (secondByte >= 0x80)
                    secondByte--;
                secondByte += 0x5F;
            }

            msg.Text.Append(GetJapaneseString(firstByte, secondByte));
        }

        void ParseControlCode(byte control, Message msg)
        {
            // Control codes reset the kanji page2 mode
            kanjiPage2Mode = false;

            switch (control) {
                case 0x01:
                    ParseTextStart(msg);
                    break;

                case 0x02:
                    ParseVariables(control, msg);
                    break;

                case 0x05:
                    ParseTextBound(msg);
                    break;

                case 0x1B:
                    ParseTextFormat(msg);
                    break;

                default:
                    throw new FormatException("Unsupported control code");
            }
        }

        void ParseTextStart(Message msg)
        {
            byte code = reader.ReadByte();

            if (code == 0x22) {
                // Means start of literals
                // Added when starting SJIS or ASCII text, needs after variable
            } else if (code == 0x53) {
                msg.Text.Append("{multi-start:");
                ParseVariables(reader.ReadByte(), msg);
                msg.Text.Append(",");
                ParseVariables(reader.ReadByte(), msg);
                msg.Text.Append("}");
            } else {
                throw new FormatException("Unsupported Control1 code");
            }
        }

        void ParseTextBound(Message msg)
        {
            if (reader.ReadByte() != 0x05)
                throw new FormatException("Invalid Control5 code1");

            byte code = reader.ReadByte();
            if (code == 0x04) {
                msg.Text.Append("{text-if:");
                ParseVariables(reader.ReadByte(), msg);
                msg.Text.Append("}");
                msg.MultiText = true;
            } else if (code == 0x05) {
                msg.Text.Append("{end}");
            } else {
                throw new FormatException("Unsupported Control5 code");
            }
        }

        void ParseTextFormat(Message msg)
        {
            byte code = reader.ReadByte();
            switch (code) {
                case 0x40:
                    int charIdx = ReadParameter();
                    msg.Text.Append($"{{char:{charIdx}}}");
                    break;

                case 0x48:
                    kanjiPage2Mode = false;
                    kanjiSingleByteMode = false;
                    break;

                case 0x4B:
                    kanjiPage2Mode = true;
                    break;

                case 0x63:
                    byte color = reader.ReadByte();
                    msg.Text.Append($"{{color:{color}}}");
                    break;

                case 0x66:
                    byte charImgIdx = reader.ReadByte();
                    msg.Text.Append($"{{char_img:{charImgIdx}}}");
                    break;

                case 0x6B:
                    kanjiPage2Mode = true;
                    kanjiSingleByteMode = true;
                    break;

                // Indicate the start or end of the furigana
                case 0x72:
                    furiganaMode = !furiganaMode;
                    msg.Text.Append(furiganaMode ? "[" : "]");
                    break;

                case 0x73:
                    byte speakerColor = reader.ReadByte();
                    msg.Text.Append($"{{speaker_color:{speakerColor}}}");
                    break;

                case 0x77:
                    byte wait = reader.ReadByte();
                    msg.Text.Append($"{{wait:{wait}}}");
                    break;

                default:
                    throw new FormatException("Unsupported Control1B code");
            }
        }

        void ParseVariables(byte control, Message msg)
        {
            if (control == 0x25) {
                msg.Text.Append(ReadParameter());
                return;
            }

            if (control != 0x02)
                throw new FormatException("Invalid control code for variables");

            byte varId = reader.ReadByte();
            int group = varId / 10;
            int modulo = varId % 10;
            int index = (varId < 200) ? reader.ReadByte() : 0;

            switch (group) {
                case 5:
                    if (index < 0x20) {
                        reader.Stream.Position--;
                        msg.Text.Append("{turns}");
                    } else
                        throw new FormatException("Unknown variable group 5");
                    break;

                case 6:
                    if (index < 0x20) {
                        reader.Stream.Position--;
                        msg.Text.Append("{attack}");
                    } else if (index == 100)
                        msg.Text.Append("{enemy}");
                    else
                        throw new FormatException("Unknown variable group 6");
                    break;

                case 7:
                    if (index == 100)
                        msg.Text.Append($"{{pokemon_trainer:{modulo}}}");
                    else
                        throw new FormatException("Unknown variable group 7");
                    break;

                case 8:
                    if (index == 100 && modulo == 0)
                        msg.Text.Append("{name2}");
                    else if (index == 100)
                        msg.Text.Append("{name_npc}");
                    else if (index == 101)
                        msg.Text.Append("{name5}");
                    else
                        throw new FormatException("Unknown variable group 8");
                    break;

                case 9:
                    if (index == 100)
                        msg.Text.Append("{name3}");
                    else
                        throw new FormatException("Unknown variable group 9");
                    break;

                case 10:
                    if (index == 100)
                        msg.Text.Append("{item}");
                    else
                        throw new FormatException("Unknown variable group 10");
                    break;

                case 11:
                    if (index == 100 && modulo == 0)
                        msg.Text.Append("{area}");
                    else
                        throw new FormatException("Unknown variable group 11");
                    break;

                case 12:
                    if (index == 100)
                        msg.Text.Append("{map_obj}");
                    else
                        throw new FormatException("Unknown variable group 12");
                    break;

                case 20:
                    // 0 and 1 also call ParseExpression
                    if (modulo == 2)
                        msg.Text.Append($"{{commander:{ParseExpression()}}}");
                    else if (modulo == 3)
                        msg.Text.Append("{name_blue}");
                    else if (modulo == 4)
                        msg.Text.Append("{lord_name}");
                    else
                        throw new FormatException("Unknown variable group 20");
                    break;

                default:
                    throw new FormatException("Invalid variable group");
            }
        }

        int ParseExpression()
        {
            // It should actually call ParseVariable, but we only support
            // constants (code 0x25)
            if (reader.ReadByte() != 0x25)
                throw new FormatException("Unsupported expression");
            int result = ReadParameter();

            // We can do maths if the next code is 0x03 (not supported yet)
            // There would be a next byte (0x25 - 0x2F) with the math op
            // and then another constant to apply the math. Repeat for next 0x03
            if (reader.ReadByte() == 0x03)
                throw new FormatException("Unsupported math expression");
            else
                reader.Stream.Position--;

            return result;
        }

        int ReadParameter()
        {
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < 10; i++) {
                char ch = reader.ReadChar();
                if (ch < '0' || ch > '9') {
                    reader.Stream.Position--;
                    break;
                }

                text.Append(ch);
            }

            return int.Parse(text.ToString());
        }
    }
}
