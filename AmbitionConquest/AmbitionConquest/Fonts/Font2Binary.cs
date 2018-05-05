//
//  Font2Binary.cs
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
namespace AmbitionConquest.Fonts
{
    using System;
    using System.Drawing;
    using Mono.Addins;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    [Extension]
    public class Font2Binary : IConverter<BinaryFormat, Font>
    {
        const int CharWidth = 28;
        const int CharHeight = 19;
        const int CharSize = 68; // 1 byte for width + (28 * 19 / 8 ~~ 67)

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
                source.Stream.Position = 0x1CC + (i * CharSize);
                glyph.Width = reader.ReadByte();
                glyph.Image = new Color[CharWidth, CharHeight];

                byte buffer = 0;
                byte bufferSize = 0;
                for (int y = 0; y < CharHeight; y++) {
                    for (int x = 0; x < CharWidth; x++, bufferSize--) {
                        if (bufferSize == 0) {
                            buffer = reader.ReadByte();
                            bufferSize = 8;
                        }

                        glyph.Image[x, y] = (((buffer >> (bufferSize - 1)) & 1) == 1) ?
                            Color.Black : Color.White;
                    }
                }

                font.Glyphs.Add(glyph);
            }

            return font;
        }
    }
}
