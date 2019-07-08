// Copyright (c) 2019 pleonex
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
namespace AmbitionConquest.IntegrationTests
{
    using System;
    using System.IO;
    using System.Collections;
    using NUnit.Framework;
    using AmbitionConquest.Texts;

    public static class TestData
    {
        public static string ResourceDirectory {
            get {
                string envVar = Environment.GetEnvironmentVariable("YARHL_TEST_DIR");
                if (!string.IsNullOrEmpty(envVar))
                    return envVar;

                string programDir = AppDomain.CurrentDomain.BaseDirectory;
                return Path.Combine(
                    programDir,
                    // net472, Debug, bin, project name
                    "..", "..", "..", "..",
                    "GameData",
                    "root");
            }
        }

        public static string RomDataDirectory {
            get {
                return Path.Combine(ResourceDirectory, "rom", "data");
            }
        }

        public static IEnumerable TextListFiles {
            get {
                yield return new TestFixtureData(
                    Path.Combine(RomDataDirectory, "Building.dat"),
                    TextListKind.Building);
                yield return new TestFixtureData(
                    Path.Combine(RomDataDirectory, "EventSpeaker.dat"),
                    TextListKind.EventSpeaker);
                yield return new TestFixtureData(
                    Path.Combine(RomDataDirectory, "Gimmick.dat"),
                    TextListKind.Gimmick);
                yield return new TestFixtureData(
                    Path.Combine(RomDataDirectory, "Item.dat"),
                    TextListKind.Item);
                yield return new TestFixtureData(
                    Path.Combine(RomDataDirectory, "Kuni.dat"),
                    TextListKind.Kuni);
                yield return new TestFixtureData(
                    Path.Combine(RomDataDirectory, "Saihai.dat"),
                    TextListKind.Saihai);
                yield return new TestFixtureData(
                    Path.Combine(RomDataDirectory, "Tokusei.dat"),
                    TextListKind.Tokusei);
                yield return new TestFixtureData(
                    Path.Combine(RomDataDirectory, "Waza.dat"),
                    TextListKind.Waza);
            }
        }
    }
}
