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
namespace AmbitionConquest.IntegrationTests.Texts
{
    using AmbitionConquest.Texts;
    using NUnit.Framework;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    [TestFixtureSource(typeof(TestData), nameof(TestData.BlockMessage))]
    public class IdenticalMessagesTests
    {
        string filePath;

        public IdenticalMessagesTests(string filePath)
        {
            this.filePath = filePath;
        }

        [Test]
        public void TestEncryption()
        {
            using (Node node = NodeFactory.FromFile(filePath)) {
                var originalBin = node.Stream;

                var blocks = ConvertFormat
                    .ConvertWith<Binary2Blocks>(node.Format);
                var importedBin = (BinaryFormat)ConvertFormat
                    .ConvertWith<Binary2Blocks>(blocks);

                bool comparaison = importedBin.Stream.Compare(originalBin);
                Assert.That(comparaison, Is.True);
            }
        }
    }
}
