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
    using Yarhl.Media.Text;

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
                    .With<Binary2Blocks>(node.Format);
                var importedBin = (BinaryFormat)ConvertFormat
                    .With<Binary2Blocks>(blocks);

                bool comparaison = importedBin.Stream.Compare(originalBin);
                Assert.That(comparaison, Is.True);
            }
        }

        [Test]
        public void TestConvertPoTwoWays()
        {
            using (Node node = NodeFactory.FromFile(filePath)) {
                node.TransformWith<Binary2Blocks>();

                foreach (var child in node.Children) {
                    child.TransformTo<BlockMessages>();
                    var expected = child.GetFormatAs<BlockMessages>();

                    child.TransformTo<Po>()
                        .TransformTo<BlockMessages>();
                    var actual = child.GetFormatAs<BlockMessages>();

                    Assert.That(actual.Messages.Count, Is.EqualTo(expected.Messages.Count));
                    for (int i = 0; i < actual.Messages.Count; i++) {
                        var actualMsg = actual.Messages[i];
                        var expectedMsg = expected.Messages[i];

                        Assert.That(actualMsg.IsEmpty, Is.EqualTo(expectedMsg.IsEmpty));
                        Assert.That(actualMsg.GroupId, Is.EqualTo(expectedMsg.GroupId));
                        Assert.That(actualMsg.ElementId, Is.EqualTo(expectedMsg.ElementId));
                        Assert.That(actualMsg.Text, Is.EqualTo(expectedMsg.Text));
                        Assert.That(actualMsg.BoxConfig, Is.EqualTo(expectedMsg.BoxConfig));
                        Assert.That(actualMsg.Context, Is.EqualTo(expectedMsg.Context));
                    }
                }
            }
        }

        [Test]
        public void TestConvertBlockMessageTwoWays()
        {
            using (Node node = NodeFactory.FromFile(filePath)) {
                node.TransformWith<Binary2Blocks>();

                foreach (var child in node.Children) {
                    var expected = child.Stream;

                    var messages = ConvertFormat.To<BlockMessages>(child.Format);
                    var actualBinary = (BinaryFormat)ConvertFormat
                        .To<BinaryFormat>(messages);

                    Assert.That(actualBinary.Stream.Compare(expected), Is.True, child.Name);
                }
            }
        }
    }
}
