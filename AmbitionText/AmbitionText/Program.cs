//
//  Program.cs
//
//  Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
//  Copyright (c) 2015 Benito Palacios Sánchez
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;

namespace AmbitionText
{
	public static class Program
	{
        static readonly string ProgramPath = Path.GetDirectoryName(
            Assembly.GetExecutingAssembly().CodeBase);

		public static void Main(string[] args)
		{
            Console.WriteLine("Pokemon Nobunaga Ambition && Pokemon Conquest");
            Console.WriteLine("Text extractor for MSG.DAT ~~ by pleoNeX\n");
			if ((args.Length != 4 && args.Length != 3) ||
                (args.Length == 4 && args[3] != "/nt")) {

				Help();
				return;
			}

            bool useTable = File.Exists(Path.Combine(ProgramPath, "table.tbl"));
			if (args[0] == "-e") {
				if (!Directory.Exists(args[1])) {
					Console.WriteLine("Input doesn't exist");
					return;
				}
				
				if (!Directory.Exists(args[2]))
					Directory.CreateDirectory(args[2]);
				if (args[2][args[2].Length - 1] != Path.DirectorySeparatorChar)
					args[2] += Path.DirectorySeparatorChar;
				
				foreach (string fileIn in Directory.GetFiles(args[1])) {
					if (!fileIn.ToLower().EndsWith(".bin"))
						continue;
					
					string fileOut = args[2] + Path.GetFileNameWithoutExtension(fileIn) + ".xml";
					Export(fileIn, fileOut, useTable);
				}
			} else if (args[0] == "-ef") {
				if (!File.Exists(args[1])) {
					Console.WriteLine("File in doesn't exist");
					return;
				}
				
				Export(args[1], args[2], useTable);
			} else if (args[0] == "-i") {
				if (!Directory.Exists(args[1])) {
					Console.WriteLine("Input doesn't exist");
					return;
				}
				
				if (!Directory.Exists(args[2]))
					Directory.CreateDirectory(args[2]);
				if (args[2][args[2].Length - 1] != Path.DirectorySeparatorChar)
					args[2] += Path.DirectorySeparatorChar;
				
				foreach (string fileIn in Directory.GetFiles(args[1])) {
					if (!fileIn.ToLower().EndsWith(".xml"))
						continue;
					
					string fileOut = args[2] + Path.GetFileNameWithoutExtension(fileIn) + ".bin";
					Import(fileIn, fileOut, useTable);
				}
			} else if (args[0] == "-if") {
				if (!File.Exists(args[1])) {
					Console.WriteLine("File in doesn't exist");
					return;
				}

				Import(args[1], args[2], useTable);
			}
			
			Console.WriteLine("\nDone!");
		}
		
		static void Help()
		{
			Console.WriteLine("Usage: AmbitionText Action In Out [/nt]");
			Console.WriteLine("Actions:");
			Console.WriteLine("\t-e:\tExtract BIN files from In folder to Out folder");
			Console.WriteLine("\t-ef:\tExtract In file to Out file");
			Console.WriteLine("\t-i:\tImport XML files from In folder to Out folder");
			Console.WriteLine("\t-if:\tImport In file to Out file");
            Console.WriteLine("Optional:");
            Console.WriteLine("\t/nt:\tNot use table.tbl. By default is enabled if that file exists.");
			Console.WriteLine("\nGood hacking!");
            Console.WriteLine("Press a key to quit.");
            Console.ReadKey(true);
		}
		
		private static void Export(string fileIn, string fileOut)
		{
		    Export(fileIn, fileOut, false);
		}
		
		private static void Export(string fileIn, string fileOut, bool useTable)
		{
			byte[] data = File.ReadAllBytes(fileIn);
			uint num_phrase = BitConverter.ToUInt32(data, 0);
			
			// Initialize XML file
			XmlDocument doc = new XmlDocument();
			doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
			XmlElement root = doc.CreateElement("Block");
			
			for (int i = 0; i < num_phrase; i++)
			{
				PNAReader at = new PNAReader(data, i, useTable);
				
				XmlElement elem = doc.CreateElement("Text");
				elem.SetAttribute("ID", i.ToString("x"));
				
				string text = at.Text;
				text = text.Replace("\n", "\n    ");
				if (text.Contains("\n"))
					text = "\n    " + text + "\n  ";
				elem.InnerText = text;
				
				root.AppendChild(elem);
			}

			doc.AppendChild(root);
			doc.Save(fileOut);
		}
		
		private static void Import(string fileIn, string fileOut)
		{
		    Import(fileIn, fileOut, false);
		}
		
		private static void Import(string fileIn, string fileOut, bool useTable)
		{
			if (File.Exists(fileOut))
				File.Delete(fileOut);
			
			BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));
			
			XmlDocument doc = new XmlDocument();
            try {
                doc.Load(fileIn);
            } catch (Exception e) {
                Console.WriteLine("Error opening XML, probably it contains syntax errors:");
                Console.WriteLine(e.Message);
                return;
            }

			XmlNode root = doc.ChildNodes[1];
			
			bw.Write(root.ChildNodes.Count);
			
			int offset = root.ChildNodes.Count * 4 + 4;
			List<byte> buffer = new List<byte>();
            for (int i = 0; i < root.ChildNodes.Count; i++) {
				bw.Write(offset);
				
				string text = root.ChildNodes[i].InnerText;
				if (text.Contains("\n")) {
					text = text.Remove(0, 5);
					text = text.Remove(text.Length - 3);
					text = text.Replace("\n    ", "\n");
				}
				
				PNAWriter pw = new PNAWriter(text, useTable);
				offset += pw.Data.Length;
				buffer.AddRange(pw.Data);
			}
			
			bw.Write(buffer.ToArray());
			bw.Flush();
			bw.Close();
		}
	}
}