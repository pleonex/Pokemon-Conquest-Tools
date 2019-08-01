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
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using Yarhl.Media.Text;

    public class Binary2PokemonList :
        IConverter<BinaryFormat, PokemonList>,
        IConverter<PokemonList, BinaryFormat>
    {
        static Binary2PokemonList()
        {
            // Make sure that the shift-jis encoding is initialized in
            // .NET Core
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public PokemonList Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            PokemonList list = new PokemonList();

            DataReader reader = new DataReader(source.Stream) {
                DefaultEncoding = Encoding.GetEncoding("shift_jis"),
            };

            for (int i = 0; i < PokemonList.Count; i++) {
                PokemonInfo pokemon = new PokemonInfo();

                string text = reader.ReadString(PokemonInfo.MaxNameLength).Replace("\0", string.Empty);
                pokemon.Name = Table.Instance.Decode(text);
                pokemon.Metadata = reader.ReadBytes(PokemonInfo.MetadataSize);

                list.Pokemons.Add(pokemon);
            }

            list.Metadata = reader.ReadBytes(PokemonList.MetadataSize);

            return list;
        }

        public BinaryFormat Convert(PokemonList source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            BinaryFormat binary = new BinaryFormat();
            DataWriter writer = new DataWriter(binary.Stream) {
                DefaultEncoding = Encoding.GetEncoding("shift_jis"),
            };

            Table table = Table.Instance;
            foreach (var entry in source.Pokemons) {
                string text = table.Encode(entry.Name);
                writer.Write(text, PokemonInfo.MaxNameLength);
                writer.Write(entry.Metadata);
            }

            writer.Write(source.Metadata);

            return binary;
        }
    }
}
