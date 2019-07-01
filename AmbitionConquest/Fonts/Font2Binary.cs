// Font2Binary.cs
//
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
namespace AmbitionConquest.Fonts
{
    using System;
    using System.Drawing;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    public class Font2Binary : IConverter<BinaryFormat, Font>
    {
        const int GlyphWidth = 28;
        const int GlyphHeight = 19;
        const int GlyphSize = 68; // 1 byte for width + (28 * 19 / 8 ~~ 67)

        public Font Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            DataReader reader = new DataReader(source.Stream);

            // Get the number of chars
            source.Stream.Position = 0x18;
            ushort numChars = reader.ReadUInt16();

            Font font = new Font();
            for (int i = 0; i < numChars; i++) {
                Glyph glyph = new Glyph();
                glyph.Id = i;

                // Get the char code
                source.Stream.Position = 0x28 + (i * 2);
                glyph.CharCode = reader.ReadUInt16();

                // Get the image
                source.Stream.Position = 0x1CC + (i * GlyphSize);
                glyph.Width = reader.ReadByte();
                glyph.Image = ReadGlyph(source.Stream);
                font.Glyphs.Add(glyph);
            }

            return font;
        }

        Color[,] ReadGlyph(DataStream stream)
        {
            var glyph = new Color[GlyphWidth, GlyphHeight];

            byte buffer = 0;
            byte bufferSize = 0;
            for (int y = 0; y < GlyphHeight; y++) {
                for (int x = 0; x < GlyphWidth; x++, bufferSize--) {
                    if (bufferSize == 0) {
                        buffer = stream.ReadByte();
                        bufferSize = 8;
                    }

                    glyph[x, y] = (((buffer >> (bufferSize - 1)) & 1) == 1)
                        ? Color.Black
                        : Color.White;
                }
            }

            return glyph;
        }
    }
}
