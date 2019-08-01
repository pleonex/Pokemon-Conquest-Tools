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
namespace AmbitionConquest.IntegrationTests.Texts
{
    using AmbitionConquest.Texts;
    using NUnit.Framework;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    [TestFixtureSource(typeof(TestData), nameof(TestData.PokemonList))]
    public class IdenticalPokemonListTests
    {
        string filePath;

        public IdenticalPokemonListTests(string filePath)
        {
            this.filePath = filePath;
        }

        [Test]
        public void TestConvertBinaryListTwoWays()
        {
            using (Node node = NodeFactory.FromFile(filePath)) {
                var originalBin = node.Stream;

                var textList = ConvertFormat
                    .With<Binary2PokemonList>(node.Format);
                var importedBin = (BinaryFormat)ConvertFormat
                    .With<Binary2PokemonList>(textList);

                bool comparaison = importedBin.Stream.Compare(originalBin);
                Assert.That(comparaison, Is.True);
            }
        }

        [Test]
        public void TestConvertPoTwoWaysWithoutOriginal()
        {
            using (Node node = NodeFactory.FromFile(filePath)) {
                var originalList = (PokemonList)ConvertFormat
                    .With<Binary2PokemonList>(node.Format);

                var po = ConvertFormat
                    .With<PokemonList2Po>(originalList);
                var importedList = (PokemonList)ConvertFormat
                    .With<PokemonList2Po>(po);

                Assert.That(importedList.Metadata, Is.Null);
                Assert.That(
                    importedList.Pokemons.Count,
                    Is.EqualTo(originalList.Pokemons.Count));
                for (int i = 0; i < importedList.Pokemons.Count; i++) {
                    Assert.That(importedList.Pokemons[i].Metadata, Is.Null);
                    Assert.That(
                        importedList.Pokemons[i].Name,
                        Is.EqualTo(originalList.Pokemons[i].Name));
                }
            }
        }

        [Test]
        public void TestConvertPoTwoWaysWithOriginal()
        {
            using (Node node = NodeFactory.FromFile(filePath)) {
                var originalList = (PokemonList)ConvertFormat
                    .With<Binary2PokemonList>(node.Format);

                var po = ConvertFormat
                    .With<PokemonList2Po>(originalList);
                var importedList = (PokemonList)ConvertFormat
                    .With<PokemonList2Po, PokemonList>(originalList, po);

                Assert.That(importedList.Metadata, Is.EqualTo(originalList.Metadata));
                Assert.That(
                    importedList.Pokemons.Count,
                    Is.EqualTo(originalList.Pokemons.Count));

                for (int i = 0; i < importedList.Pokemons.Count; i++) {
                    Assert.That(
                        importedList.Pokemons[i].Metadata,
                        Is.EqualTo(originalList.Pokemons[i].Metadata));
                    Assert.That(
                        importedList.Pokemons[i].Name,
                        Is.EqualTo(originalList.Pokemons[i].Name));
                }
            }
        }
    }
}
