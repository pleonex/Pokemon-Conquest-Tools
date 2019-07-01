// Program.cs
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
namespace AmbitionConquest
{
    using System;
    using System.Reflection;
    using AmbitionConquest.Fonts;
    using AmbitionConquest.Texts;
    using Yarhl.FileSystem;
    using Yarhl.IO;
    using Yarhl.Media.Text;

    static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(
                "AmbitionConquest v{0} -- Editor for Pokémon Conquest ~~ by pleonex",
                Assembly.GetExecutingAssembly().GetName().Version);

            if (args.Length != 3) {
                Console.WriteLine("USAGE: [mono] AmbitionConquest.exe type in out");
                FailExit();
            }

            string type = args[0];
            string inputPath = args[1];
            string outputPath = args[2];

            Console.Write($"[{type}] {inputPath} --> ");

            switch (type) {
                case "font_json":
                    NodeFactory.FromFile(inputPath)
                        .TransformTo<Font>()
                        .TransformWith<Font2Json>()
                        .Stream.WriteTo(outputPath);
                    break;

                case "font_image":
                    NodeFactory.FromFile(inputPath)
                        .TransformTo<Font>()
                        .TransformTo<IImage>()
                        .GetFormatAs<IImage>().Image.Save(outputPath);
                    break;

                case "building":
                    NodeFactory.FromFile(inputPath)
                        .TransformWith<BlockText2Po, BlockTextFile>(BlockTextFile.Building)
                        .TransformWith<Po, BinaryFormat>(new Po2Binary())
                        .Stream.WriteTo(outputPath);
                    break;
                case "eventspeaker":
                    NodeFactory.FromFile(inputPath)
                        .TransformWith<BlockText2Po, BlockTextFile>(BlockTextFile.EventSpeaker)
                        .TransformWith<Po2Binary>()
                        .Stream.WriteTo(outputPath);
                    break;
                case "gimmick":
                    NodeFactory.FromFile(inputPath)
                        .TransformWith<BlockText2Po, BlockTextFile>(BlockTextFile.Gimmick)
                        .TransformWith<Po2Binary>()
                        .Stream.WriteTo(outputPath);
                    break;
                case "item":
                    NodeFactory.FromFile(inputPath)
                        .TransformWith<BlockText2Po, BlockTextFile>(BlockTextFile.Item)
                        .TransformWith<Po2Binary>()
                        .Stream.WriteTo(outputPath);
                    break;
                case "kuni":
                    NodeFactory.FromFile(inputPath)
                        .TransformWith<BlockText2Po, BlockTextFile>(BlockTextFile.Kuni)
                        .TransformWith<Po2Binary>()
                        .Stream.WriteTo(outputPath);
                    break;
                case "saihai":
                    NodeFactory.FromFile(inputPath)
                        .TransformWith<BlockText2Po, BlockTextFile>(BlockTextFile.Saihai)
                        .TransformWith<Po2Binary>()
                        .Stream.WriteTo(outputPath);
                    break;
                case "tokusei":
                    NodeFactory.FromFile(inputPath)
                        .TransformWith<BlockText2Po, BlockTextFile>(BlockTextFile.Tokusei)
                        .TransformWith<Po2Binary>()
                        .Stream.WriteTo(outputPath);
                    break;
                case "waza":
                    NodeFactory.FromFile(inputPath)
                        .TransformWith<BlockText2Po, BlockTextFile>(BlockTextFile.Waza)
                        .TransformWith<Po2Binary>()
                        .Stream.WriteTo(outputPath);
                    break;

                default:
                    Console.WriteLine("Unsupported type");
                    FailExit();
                    break;
            }

            Console.WriteLine("Done!");
        }

        static void FailExit()
        {
            Console.WriteLine("Press a key to quit...");
            Console.ReadLine();
            Environment.Exit(-1);
        }
    }
}
