// Font2Json.cs
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
    using System.Text;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    public class Font2Json : IConverter<Font, BinaryFormat>
    {
        public BinaryFormat Convert(Font source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            BinaryFormat json = new BinaryFormat();
            TextWriter writer = new TextWriter(json.Stream);

            // To try to convert the char code
            Encoding latin1 = Encoding.GetEncoding("latin1");

            bool first = true;
            writer.WriteLine("[");
            foreach (var glyph in source.Glyphs) {
                if (!first)
                    writer.WriteLine(",");
                else
                    first = false;

                string ch = latin1.GetString(BitConverter.GetBytes(glyph.CharCode))
                    .Substring(0, 1);
                if (ch == "\"" || ch == "\\")
                    ch = "\\" + ch;

                writer.WriteLine("  {");
                writer.WriteLine($"    \"image\": {glyph.Id},");
                writer.WriteLine($"    \"code\": \"{glyph.CharCode:X4}\",");
                writer.WriteLine($"    \"latin1\": \"{ch}\",");
                writer.WriteLine($"    \"width\": {glyph.Width}");
                writer.Write("  }");
            }

            writer.WriteLine();
            writer.WriteLine("]");

            return json;
        }
    }
}