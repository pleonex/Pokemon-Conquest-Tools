// Author:
//       Benito Palacios Sanchez <benito356@gmail.com>
//
// Copyright (c) 2018 Benito Palacios Sanchez
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using Yarhl.Media.Text;

    public class Binary2TextList :
        IInitializer<TextListKind>,
        IConverter<BinaryFormat, Po>,
        IConverter<Po, BinaryFormat>
    {
        static readonly IDictionary<TextListKind, Tuple<int, int>> FileInfo =
            new Dictionary<TextListKind, Tuple<int, int>> {
                { TextListKind.Building, new Tuple<int, int>(0x12, 0x12) },
                { TextListKind.EventSpeaker, new Tuple<int, int>(0x10, 0x02) },
                { TextListKind.Gimmick, new Tuple<int, int>(0x10, 0x18) },
                { TextListKind.Item, new Tuple<int, int>(0x14, 0x10) },
                { TextListKind.Kuni, new Tuple<int, int>(0x0A, 0x0E) },
                { TextListKind.Saihai, new Tuple<int, int>(0x10, 0x0C) },
                { TextListKind.Tokusei, new Tuple<int, int>(0x0E, 0x06) },
                { TextListKind.Waza, new Tuple<int, int>(0x0F, 0x15) },
        };

        static Binary2TextList()
        {
            // Make sure that the shift-jis encoding is initialized in
            // .NET Core
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public TextListKind Kind { get; private set; }

        public void Initialize(TextListKind param)
        {
            Kind = param;
        }

        public Po Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (!FileInfo.ContainsKey(Kind))
                throw new FormatException("Unknown file type");

            int textSize = FileInfo[Kind].Item1;
            int dataSize = FileInfo[Kind].Item2;

            Po po = new Po {
                Header = new PoHeader("Pokemon Conquest", "benito356@gmail.com", "en-US"),
            };

            Table table = Table.Instance;
            DataReader reader = new DataReader(source.Stream) {
                DefaultEncoding = Encoding.GetEncoding("shift_jis"),
            };

            int count = 0;
            while (!source.Stream.EndOfStream) {
                string text = reader.ReadString(textSize).Replace("\0", string.Empty);
                text = table.Decode(text);
                byte[] data = reader.ReadBytes(dataSize);

                var entry = new PoEntry {
                    Original = text,
                    Context = $"{count}",
                    Flags = $"max-length:{textSize}",
                    Reference = BitConverter.ToString(data),
                };

                po.Add(entry);
                count++;
            }

            return po;
        }

        public BinaryFormat Convert(Po source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (!FileInfo.ContainsKey(Kind))
                throw new FormatException("Unknown file type");

            int textSize = FileInfo[Kind].Item1;
            int dataSize = FileInfo[Kind].Item2;

            BinaryFormat binary = new BinaryFormat();
            DataWriter writer = new DataWriter(binary.Stream) {
                DefaultEncoding = Encoding.GetEncoding("shift_jis"),
            };

            Table table = Table.Instance;
            foreach (var entry in source.Entries) {
                string text = table.Encode(entry.Text);
                writer.Write(text, textSize);

                byte[] data = entry.Reference.Split('-')
                    .Select(s => byte.Parse(s, NumberStyles.HexNumber))
                    .ToArray();
                writer.Write(data);
            }

            return binary;
        }
    }
}
