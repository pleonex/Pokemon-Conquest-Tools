//
//  BlockText2Po.cs
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
    using System.Collections.Generic;
    using Mono.Addins;
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using Yarhl.Media.Text;

    [Extension]
    public class BlockText2Po : IConverter<BinaryFormat, Po>
    {
        static readonly IDictionary<BlockTextFile, Tuple<int, int>> FileInfo =
            new Dictionary<BlockTextFile, Tuple<int, int>> {
                { BlockTextFile.Building, new Tuple<int, int>(0x12, 0x12) },
                { BlockTextFile.EventSpeaker, new Tuple<int, int>(0x10, 0x02) },
                { BlockTextFile.Gimmick, new Tuple<int, int>(0x10, 0x18) },
                { BlockTextFile.Item, new Tuple<int, int>(0x14, 0x10) },
                { BlockTextFile.Kuni, new Tuple<int, int>(0x0A, 0x0E) },
                { BlockTextFile.Saihai, new Tuple<int, int>(0x10, 0x0C) },
                { BlockTextFile.Tokusei, new Tuple<int, int>(0x0E, 0x06) },
                { BlockTextFile.Waza, new Tuple<int, int>(0x0F, 0x15) }
        };

        public BlockTextFile File { get; set; }

        public Po Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (!FileInfo.ContainsKey(File))
                throw new FormatException("Unknown file type");

            int textSize = FileInfo[File].Item1;
            int dataSize = FileInfo[File].Item2;

            Po po = new Po {
                Header = new PoHeader("Pokemon Conquest", "benito356@gmail.com") {
                    Language = "en-US",
                    LanguageTeam = "GradienWords",
                }
            };

            DataReader reader = new DataReader(source.Stream) {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEnconding("ascii")
            };

            while (!source.Stream.EndOfStream) {
                po.Add(new PoEntry(reader.ReadString(textSize).Replace("\0", "")));
                source.Stream.Position += dataSize;
            }

            return po;
        }
    }
}
