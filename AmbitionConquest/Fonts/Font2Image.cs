// Font2Image.cs
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

    public class Font2Image : IConverter<Font, IImage>
    {
        const int CharWidth = 28;
        const int CharHeight = 19;

        const int CharsPerLine = 16;
        const int BorderThickness = 2;
        static readonly Pen BorderPen = new Pen(Color.Olive, BorderThickness);

        public IImage Convert(Font source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            int numChars = source.Glyphs.Count;

            // Gets the number of columns and rows from the CharsPerLine value.
            int numColumns = (numChars < CharsPerLine) ? numChars : CharsPerLine;
            int numRows = (int)Math.Ceiling((double)numChars / numColumns);

            // Char width + border from one side + border from the other side only at the end
            int width = (numColumns * CharWidth) + ((numColumns + 1) * BorderThickness);
            int height = (numRows * CharHeight) + ((numRows + 1) * BorderThickness);

            Bitmap image = new Bitmap(width, height);
            Graphics graphic = Graphics.FromImage(image);

            // Draw chars
            for (int r = 0; r < numRows; r++) {
                for (int c = 0; c < numColumns; c++) {
                    int index = (r * numColumns) + c; // Index of the glyph
                    if (index >= numChars)
                        break;

                    // Gets coordinates
                    int x = c * (CharWidth + BorderThickness);
                    int y = r * (CharHeight + BorderThickness);

                    // Alignment due to rectangle drawing method.
                    int borderAlign = (int)Math.Floor(BorderThickness / 2.0);

                    graphic.DrawRectangle(
                        BorderPen,
                        x + borderAlign,
                        y + borderAlign,
                        CharWidth + BorderThickness,
                        CharHeight + BorderThickness);

                    graphic.DrawImage(
                        GlyphToImage(source.Glyphs[index]),
                        x + BorderThickness,
                        y + BorderThickness);
                }
            }

            graphic.Dispose();
            return new ImageImpl { Image = image };
        }

        static Bitmap GlyphToImage(Glyph glyph)
        {
            Bitmap bmp = new Bitmap(
                glyph.Image.GetLength(0) + 1,
                glyph.Image.GetLength(1) + 1);

            for (int w = 0; w < glyph.Image.GetLength(0); w++) {
                for (int h = 0; h < glyph.Image.GetLength(1); h++) {
                    bmp.SetPixel(w, h, glyph.Image[w, h]);
                }
            }

            return bmp;
        }
    }
}
