// Binary2BlockMessages.cs
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
    using System.Linq;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    public class Binary2BlockMessages :
        IConverter<IBinary, BlockMessages>,
        IConverter<BlockMessages, BinaryFormat>
    {
        public BlockMessages Convert(IBinary source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var dialogs = new BlockMessages();
            var reader = new DataReader(source.Stream);
            var pnaReader = new PnaTextReader(source.Stream);
            source.Stream.Position = 0;

            uint numDialogs = reader.ReadUInt32();
            for (int i = 0; i < numDialogs; i++) {
                // Get offset and size
                source.Stream.Position = (i + 1) * 4;
                uint offset = reader.ReadUInt32();

                uint endOffset;
                if (i + 1 == numDialogs) {
                    endOffset = (uint)source.Stream.Length;
                } else {
                    endOffset = reader.ReadUInt32();
                }

                // Read with our custom encoding
                source.Stream.Position = offset;
                int elementId = 0;
                while (source.Stream.Position < endOffset) {
                    Message msg = pnaReader.ReadMessage();
                    msg.GroupId = i;
                    msg.ElementId = elementId++;
                    dialogs.Add(msg);
                }
            }

            return dialogs;
        }

        public BinaryFormat Convert(BlockMessages source)
        {
            if (source == null)
                throw new ArgumentNullException();

            var binary = new BinaryFormat();
            var writer = new DataWriter(binary.Stream);
            var pnaWriter = new PnaTextWriter(binary.Stream);

            // Group the messages by groupId so we can generate the table
            var msgByGroup = source.Messages.GroupBy(m => m.GroupId).ToArray();

            // Write number and empty offset table
            writer.Write(msgByGroup.Length);
            writer.WriteTimes(0x00, 4 * msgByGroup.Length);

            // Write every message with its offset
            for (int i = 0; i < msgByGroup.Length; i++) {
                binary.Stream.RunInPosition(
                    () => writer.Write((uint)binary.Stream.Position),
                    4 + (i * 4));

                // Write all the elements of the message
                foreach (var msg in msgByGroup[i]) {
                    pnaWriter.WriteMessage(msg);
                }
            }

            return binary;
        }
    }
}