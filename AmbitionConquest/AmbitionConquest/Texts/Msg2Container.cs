//
//  Msg2Container.cs
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
    using Mono.Addins;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    [Extension]
    public class Msg2Container : IConverter<BinaryFormat, NodeContainerFormat>
    {
        public NodeContainerFormat Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var container = new NodeContainerFormat();
            DataReader reader = new DataReader(source.Stream);

            // Read blocks until find a null offset
            int count = 0;
            uint offset;
            while ((offset = reader.ReadUInt32()) != 0) {
                int size = reader.ReadInt32();

                // Get the block data
                reader.Stream.PushToPosition(offset);
                byte[] data = reader.ReadBytes(size);
                reader.Stream.PopPosition();

                // Decrypt
                Encryption(data);

                // And create a file
                BinaryFormat fileData = new BinaryFormat();
                fileData.Stream.Write(data, 0, data.Length);
                container.Root.Add(new Node($"block{count++}", fileData));
            }

            return container;
        }

        static void Encryption(byte[] data)
        {
            const string Key = "MsgLinker Ver1.00";
            for (int i = 0; i < data.Length; i++)
                data[i] ^= (byte)Key[i % Key.Length];
        }
    }
}