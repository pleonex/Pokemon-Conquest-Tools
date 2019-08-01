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
    using Yarhl.Media.Text;

    public class PokemonList2Po :
        IConverter<PokemonList, Po>,
        IInitializer<PokemonList>,
        IConverter<Po, PokemonList>
    {
        PokemonList originalList;

        public void Initialize(PokemonList original)
        {
            originalList = original;
        }

        public Po Convert(PokemonList source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            PoHeader header = new PoHeader("Pokemon Conquest", "benito356@gmail.com", "en-US");
            Po po = new Po(header);

            for (int i = 0; i < source.Pokemons.Count; i++) {
                var entry = new PoEntry {
                    Original = source.Pokemons[i].Name,
                    Context = $"{i}",
                    Flags = $"max-length:{PokemonInfo.MaxNameLength}",
                };

                po.Add(entry);
            }

            return po;
        }

        public PokemonList Convert(Po source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            PokemonList list = new PokemonList();
            if (originalList != null) {
                list.Metadata = (byte[])originalList.Metadata.Clone();
            }

            for (int i = 0; i < source.Entries.Count; i++) {
                PokemonInfo pokemon = new PokemonInfo();
                if (originalList != null) {
                    pokemon.Metadata = (byte[])originalList.Pokemons[i].Metadata.Clone();
                }

                pokemon.Name = source.Entries[i].Text;
                list.Pokemons.Add(pokemon);
            }

            return list;
        }
    }
}
