// BlockMessages2Po.cs
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
namespace AmbitionConquest.Texts
{
    using System;
    using System.Linq;
    using Yarhl.FileFormat;
    using Yarhl.Media.Text;

    public class BlockMessages2Po :
        IConverter<BlockMessages, Po>,
        IConverter<Po, BlockMessages>
    {
        public Po Convert(BlockMessages source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Po po = new Po {
                Header = new PoHeader("Pokemon Conquest", "benito356@gmail.com", "en-US"),
            };

            Table table = Table.Instance;
            foreach (var message in source.Messages) {
                if (message.IsEmpty) {
                    continue;
                }

                PoEntry entry = new PoEntry();
                if (string.IsNullOrEmpty(message.Text)) {
                    entry.Original = "<empty>";
                    entry.Translated = "<empty>";
                } else {
                    entry.Original = table.Decode(message.Text);
                }

                entry.Context = $"group:{message.GroupId},id:{message.ElementId}";
                entry.ExtractedComments = $"context:{message.Context},box:{message.BoxConfig}";
                entry.Flags = "max-length:9999,c-format,brace-format";

                po.Add(entry);
            }

            return po;
        }

        public BlockMessages Convert(Po source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Table table = Table.Instance;
            BlockMessages block = new BlockMessages();
            foreach (var entry in source.Entries) {
                Message msg = new Message();
                string text = (entry.Text == "<empty>") ? string.Empty : entry.Text;
                msg.Text = table.Encode(text);

                string[] context = entry.Context.Split(',');
                string groupId = context[0].Substring("group:".Length);
                msg.GroupId = int.Parse(groupId);
                string elementId = context[1].Substring("id:".Length);
                msg.ElementId = int.Parse(elementId);

                // Some control codes from "context" may have arguments
                // separated by commas. For that reason we can't split by commas
                // but by the second argument "box".
                string[] comments = entry.ExtractedComments.Split(
                    new[] { ",box:" },
                    StringSplitOptions.None);
                msg.Context = comments[0].Substring("context:".Length);
                msg.BoxConfig = comments[1];

                // Before adding this message, we need to add some logic
                // to know if there was some 100% empty message before.
                // I am not sure why and the meaning of the blank messages.
                // I decided to not export them to make smaller translation
                // files and to not bother that much translators.
                // I was able to find a (somehow-complex) logic for when to
                // add a blank message. It has been tested against the US
                // version only. If it doesn't work, we may need to export
                // these blank messages as "empty" too.
                AddEmptyBeforeGroupEnd(block, msg.GroupId);
                AddMissingEmptyGroups(block, msg.GroupId);
                AddMissingEmptyElements(block, msg.GroupId, msg.ElementId);

                block.Add(msg);
            }

            AddEmptyBeforeFileEnd(block);
            return block;
        }

        /// <summary>
        /// Adds an empty message when the current group ID implies it skipped
        /// other groups. That is, when last group + 1 is not the current group..
        /// </summary>
        /// <param name="block">The list of messages.</param>
        /// <param name="currentGroup">The current group ID.</param>
        static void AddMissingEmptyGroups(BlockMessages block, int currentGroup)
        {
            int expectedGroup;
            if (block.Messages.Count == 0) {
                // We expect the group to be 0, when there are no messages yet.
                expectedGroup = 0;
            } else {
                // Otherwise, we expect it's an incremental number.
                expectedGroup = block.Messages.Last().GroupId + 1;
            }

            // We add empty messages for every group missing between the expected
            // and the current. The groups will have just one element.
            for (int i = expectedGroup; i < currentGroup; i++) {
                Message empty = new Message();
                empty.GroupId = i;
                empty.ElementId = 0;
                empty.Text = string.Empty;
                empty.BoxConfig = string.Empty;
                empty.Context = string.Empty;
                block.Add(empty);
            }
        }

        /// <summary>
        /// Adds an empty message when the current element ID implies it skipped
        /// other elements.false That is, when the last element ID + 1 is not
        /// the current ID.
        /// </summary>
        /// <param name="block">The list of messages.</param>
        /// <param name="currentGroup">The current group ID.</param>
        /// <param name="currentElement">The current element ID.</param>
        static void AddMissingEmptyElements(BlockMessages block, int currentGroup, int currentElement)
        {
            int expectedElement;
            if (block.Messages.Count == 0) {
                // No elements? Then we expect element 0
                expectedElement = 0;
            } else {
                Message last = block.Messages.Last();
                if (last.GroupId != currentGroup) {
                    // If we are not in the same group, we expect element 0
                    expectedElement = 0;
                } else {
                    // Otherwise, we expect + 1 of the last element.
                    expectedElement = last.ElementId + 1;
                }
            }

            // For every missing element upto the current, add an empty message.
            for (int i = expectedElement; i < currentElement; i++) {
                Message empty = new Message();
                empty.GroupId = currentGroup;
                empty.ElementId = i;
                empty.Text = string.Empty;
                empty.BoxConfig = string.Empty;
                empty.Context = string.Empty;
                block.Add(empty);
            }
        }

        /// <summary>
        /// Adds an empty message at the end of a group when the group contains
        /// more than one message.
        /// </summary>
        /// <param name="block">List of messages.</param>
        /// <param name="currentGroup">The current group ID.</param>
        static void AddEmptyBeforeGroupEnd(BlockMessages block, int currentGroup)
        {
            // If we don't have any group, just skip.
            if (block.Messages.Count == 0) {
                return;
            }

            // We only add empty messages when a group has more than one element.
            // If the last one is 0, it means we don't have more yet.
            Message last = block.Messages.Last();
            if (last.ElementId == 0) {
                return;
            }

            bool addEmpty = currentGroup != last.GroupId;
            if (addEmpty) {
                Message empty = new Message();
                empty.GroupId = last.GroupId;
                empty.ElementId = last.ElementId + 1;
                empty.Text = string.Empty;
                empty.BoxConfig = string.Empty;
                empty.Context = string.Empty;
                block.Add(empty);
            }
        }

        /// <summary>
        /// Adds an empty message at the end of last group when the group
        /// contains more than one non-empty message.
        /// </summary>
        /// <param name="block">List of messages.</param>
        static void AddEmptyBeforeFileEnd(BlockMessages block)
        {
            // If just have one element in the group, we don't need
            // empty elements.
            Message last = block.Messages.Last();
            if (last.ElementId == 0) {
                return;
            }

            // Check if all the messages are empty.
            bool allEmpty = block.Messages
                .Where(m => m.GroupId == last.GroupId)
                .Where(m => m.IsEmpty)
                .Any();

            if (!allEmpty) {
                Message empty = new Message();
                empty.GroupId = last.GroupId;
                empty.ElementId = last.ElementId + 1;
                empty.Text = string.Empty;
                empty.BoxConfig = string.Empty;
                empty.Context = string.Empty;
                block.Add(empty);
            }
        }
    }
}
