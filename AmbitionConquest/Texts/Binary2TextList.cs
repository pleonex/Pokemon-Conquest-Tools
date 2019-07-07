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
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using Yarhl.Media.Text;
    using Yarhl.Media.Text.Encodings;

    public class Binary2TextList : IInitializer<TextListKind>, IConverter<BinaryFormat, Po>
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

            DataReader reader = new DataReader(source.Stream) {
                DefaultEncoding = new EscapeOutRangeEncoding("ascii"),
            };

            while (!source.Stream.EndOfStream) {
                string segment = reader.ReadString(textSize).Replace("\0", string.Empty);
                po.Add(new PoEntry(segment));
                source.Stream.Position += dataSize;
            }

            return po;
        }
    }
}
