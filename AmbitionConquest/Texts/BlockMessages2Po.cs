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
    using Yarhl.FileFormat;
    using Yarhl.Media.Text;

    public class BlockMessages2Po : IConverter<BlockMessages, Po>
    {
        public Po Convert(BlockMessages source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Po po = new Po {
                Header = new PoHeader("Pokemon Conquest", "benito356@gmail.com", "en-US"),
            };

            foreach (var message in source.Messages) {
                if (string.IsNullOrEmpty(message.Text)) {
                    continue;
                }

                po.Add(new PoEntry {
                    Original = message.Text,
                    Context = $"group:{message.GroupId},id:{message.ElementId}",
                    Flags = "max-length:9999,c-format,brace-format",
                });
            }

            return po;
        }
    }
}
