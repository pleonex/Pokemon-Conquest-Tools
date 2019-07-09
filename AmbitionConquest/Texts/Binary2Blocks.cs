// Binary2Blocks.cs
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
    using Yarhl.FileSystem;
    using Yarhl.IO;

    public class Binary2Blocks :
        IConverter<BinaryFormat, NodeContainerFormat>,
        IConverter<NodeContainerFormat, BinaryFormat>
    {
        public NodeContainerFormat Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            NodeContainerFormat container = new NodeContainerFormat();
            DataReader reader = new DataReader(source.Stream);

            int count = 0;
            while (source.Stream.Position < source.Stream.Length) {
                // Read FAT
                source.Stream.Position = count * 8;
                uint offset = reader.ReadUInt32();
                if (offset == 0)
                    break;

                int size = reader.ReadInt32();

                // Get encrypted data and decrypt
                source.Stream.Position = offset;
                byte[] data = reader.ReadBytes(size);
                Encryption.Run(data);

                // Create the child
                Node child = NodeFactory.FromMemory($"block{count}.bin");
                child.Stream.Write(data, 0, data.Length);
                container.Root.Add(child);

                count++;
            }

            return container;
        }

        public BinaryFormat Convert(NodeContainerFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            BinaryFormat binary = new BinaryFormat();
            DataWriter writer = new DataWriter(binary.Stream);
            var children = source.Root.Children;

            // Write empty FAT so we can start writing the file content too
            writer.WriteTimes(0x00, children.Count * 8);
            writer.WritePadding(0x00, 0x800);

            for (int i = 0; i < children.Count; i++) {
                binary.Stream.PushToPosition(i * 8);
                writer.Write((uint)binary.Stream.Length); // offset of new file
                writer.Write((uint)children[i].Stream.Length);
                binary.Stream.PopPosition();

                byte[] data = new byte[children[i].Stream.Length];
                children[i].Stream.Position = 0;
                children[i].Stream.Read(data, 0, data.Length);
                Encryption.Run(data);

                writer.Write(data);
                writer.WritePadding(0x00, 0x800);
            }

            return binary;
        }
    }
}