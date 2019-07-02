// Binary2SceneDialogs.cs
//
// Author:
//       Benito Palacios Sanchez <benito356@gmail.com>
//
// Copyright (c) 2019 Benito Palacios Sanchez
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
    using Yarhl.FileFormat;
    using Yarhl.IO;

    public class Binary2SceneDialogs : IConverter<BinaryFormat, SceneDialogs>
    {
        public SceneDialogs Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var dialogs = new SceneDialogs();
            var reader = new DataReader(source.Stream) {
                DefaultEncoding = new PnaEncoding(),
            };

            uint numDialogs = reader.ReadUInt32();
            for (int i = 0; i < numDialogs; i++) {
                // Get offset and size
                source.Stream.Position = (i + 1) * 4;
                uint offset = reader.ReadUInt32();

                int size;
                if (i + 1 == numDialogs) {
                    size = (int)(source.Stream.Length - offset);
                } else {
                    size = (int)(reader.ReadUInt32() - offset);
                }

                // Read with our custom encoding
                source.Stream.Position = offset;
                dialogs.Add(reader.ReadString(size));
            }

            return dialogs;
        }
    }
}