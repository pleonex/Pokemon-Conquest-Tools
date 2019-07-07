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
    using System.Collections.Generic;
    using System.Drawing;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    public class Font2Binary :
        IInitializer<FontKind>,
        IConverter<BinaryFormat, Font>
    {
        static readonly Dictionary<FontKind, FontInfo> Info = new Dictionary<FontKind, FontInfo> {
            { FontKind.Debug, new FontInfo(28, 19, 0x28, 0x1CC) },
            { FontKind.Small, new FontInfo(8, 11, 0x87D8, 0x8B20) },
            { FontKind.Normal, new FontInfo(10, 14, 0x897C, 0x94EC) },
        };

        FontInfo info;

        public void Initialize(FontKind kind)
        {
            info = Info[kind];
        }

        public Font Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            DataReader reader = new DataReader(source.Stream);

            Font font = new Font();
            font.GlyphWidth = info.GlyphWidth;
            font.GlyphHeight = info.GlyphHeight;

            for (int i = 0; i < info.NumGlyphs; i++) {
                Glyph glyph = new Glyph();
                glyph.Id = i;

                // Get the char code
                source.Stream.Position = info.MapOffset + (i * 2); // utf-16
                glyph.CharCode = reader.ReadUInt16();

                // Get the image
                // Glyph size + 1 byte with the actual width
                source.Stream.Position = info.ImageOffset + (i * (info.GlyphSize + 1));
                glyph.Width = reader.ReadByte();
                glyph.Image = ReadGlyph(source.Stream);
                font.Glyphs.Add(glyph);
            }

            return font;
        }

        Color[,] ReadGlyph(DataStream stream)
        {
            var glyph = new Color[info.GlyphWidth, info.GlyphHeight];

            byte buffer = 0;
            byte bufferSize = 0;
            for (int y = 0; y < info.GlyphHeight; y++) {
                for (int x = 0; x < info.GlyphWidth; x++, bufferSize--) {
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

        struct FontInfo
        {
            public FontInfo(int width, int height, uint mapOffset, uint imageOffset)
            {
                GlyphWidth = width;
                GlyphHeight = height;
                MapOffset = mapOffset;
                ImageOffset = imageOffset;

                // 1 bpp
                GlyphSize = (int)Math.Ceiling(width * height / 8.0);
            }

            public int NumGlyphs => 0xD1;  // It's hard-coded in the game code

            public int GlyphWidth { get; }

            public int GlyphHeight { get; }

            public int GlyphSize { get; }

            public uint MapOffset { get; }

            public uint ImageOffset { get; }
        }
    }
}
