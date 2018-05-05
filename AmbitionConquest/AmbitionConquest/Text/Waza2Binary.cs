//
//  Waza2Binary.cs
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
namespace AmbitionConquest.Text
{
    using System;
    using Mono.Addins;
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using Yarhl.Media.Text;

    [Extension]
    public class Waza2Binary : IConverter<BinaryFormat, Po>
    {
        const int TextSize = 0x0F;
        const int DataSize = 0x15;
        const int BlockSize = TextSize + DataSize;

        public Po Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

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
                po.Add(new PoEntry(reader.ReadString(TextSize).Replace("\0", "")));
                source.Stream.Position += DataSize;
            }

            return po;
        }
    }

    public class WazaPlainText2Binary : IConverter<BinaryFormat, BinaryFormat>
    {
        const int TextSize = 0x0F;
        const int DataSize = 0x15;
        const int BlockSize = TextSize + DataSize;

        public BinaryFormat Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            BinaryFormat textFormat = new BinaryFormat();
            TextWriter writer = new TextWriter(textFormat.Stream);

            DataReader reader = new DataReader(source.Stream) {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEnconding("ascii")
            };

            source.Stream.Position = 0;
            while (!source.Stream.EndOfStream) {
                writer.WriteLine("[[");
                writer.WriteLine(reader.ReadString(TextSize).Replace("\0", ""));
                writer.WriteLine("]]");
                writer.WriteLine();
                writer.WriteLine();
                source.Stream.Position += DataSize;
            }

            return textFormat;
        }
    }
}
