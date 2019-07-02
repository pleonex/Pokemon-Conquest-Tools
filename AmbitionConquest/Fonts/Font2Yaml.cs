// Font2Yaml.cs
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
namespace AmbitionConquest.Fonts
{
    using System;
    using System.Text;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    public class Font2Yaml : IConverter<Font, BinaryFormat>
    {
        public BinaryFormat Convert(Font source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var serializer = new SerializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .WithAttributeOverride<Glyph>(g => g.Image, new YamlIgnoreAttribute())
                .WithAttributeOverride<Glyph>(g => g.Id, new YamlIgnoreAttribute())
                .WithAttributeOverride<Glyph>(g => g.CharCode, new YamlMemberAttribute {
                    Order = 0,
                })
                .Build();
            string yaml = serializer.Serialize(source);

            byte[] yamlData = Encoding.UTF8.GetBytes(yaml);
            var binary = new BinaryFormat();
            binary.Stream.Write(yamlData, 0, yamlData.Length);

            return binary;
        }
    }
}