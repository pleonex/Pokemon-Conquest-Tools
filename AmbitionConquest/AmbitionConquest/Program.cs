//
//  Program.cs
//
//  Author:
//       Benito Palacios Sanchez <benito356@gmail.com>
//
//  Copyright (c) 2018 Benito Palacios Sanchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace AmbitionConquest
{
    using System;
    using System.Reflection;
    using Yarhl.FileSystem;
    using Yarhl.FileFormat;
    using Yarhl.Media.Text;
    using Fonts;
    using Text;

    static class MainClass
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
                        .Transform<Font2Binary, BinaryFormat, Font>()
                        .Transform<Font2Json, Font, BinaryFormat>()
                        .Stream.WriteTo(outputPath);
                    break;

                case "font_image":
                    NodeFactory.FromFile(inputPath)
                        .Transform<Font2Binary, BinaryFormat, Font>().Format
                        .ConvertWith<Font2Image, Font, System.Drawing.Image>()
                        .Save(outputPath);
                    break;

                case "waza_text":
                    NodeFactory.FromFile(inputPath)
                        .Transform<WazaPlainText2Binary, BinaryFormat, BinaryFormat>()
                        .Stream.WriteTo(outputPath);
                    break;

                case "waza_po":
                    NodeFactory.FromFile(inputPath)
                        .Transform<Waza2Binary, BinaryFormat, Po>()
                        .Transform<BinaryFormat>()
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
