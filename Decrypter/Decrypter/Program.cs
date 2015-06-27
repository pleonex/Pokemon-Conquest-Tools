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
using System.IO;

namespace Decrypt
{
	class Program
	{
		const int NUM_BLOCK = 0x2C;					// 0x21 for Pokemon Conquest
		const string KEY = "MsgLinker Ver1.00";
		const uint MULT1 = 0xF0F0F0F1;
		const int MULT2 = 0x11;
		
		public static void Main(string[] args)
		{
			Console.WriteLine("Pokemon - Nobunaga Ambition\nDecrypter for MSG.DAT by pleoNeX");

			if (args.Length < 3)
			{
				Help();
				return;
			}

			string fileIn = args[1];
			string fileOut = args[2];

			if (args[0] == "-df")
			{
				if (!File.Exists(fileIn))
				{
					Console.WriteLine("Input file not found.");
					return;
				}
				if (args.Length != 4)
				{
					Console.WriteLine("You have to specified the number of block to decrypt.");
					return;
				}
				
				Console.WriteLine("Decrypting block {0}", args[3]);
				int ind = Convert.ToInt32(args[3]);
				
				Byte[] block = Get_Block(fileIn, ind);
				block = Encryption(block);
				
				if (File.Exists(fileOut))
					File.Delete(fileOut);
				File.WriteAllBytes(fileOut, block);
			}
			else if (args[0] == "-d")
			{
				if (!File.Exists(fileIn))
				{
					Console.WriteLine("Input file not found.");
					return;
				}

				if (!Directory.Exists(fileOut))
					fileOut = Directory.CreateDirectory(fileOut).FullName;
				
				if (fileOut[fileOut.Length - 1] != Path.DirectorySeparatorChar)
					fileOut += Path.DirectorySeparatorChar;
				
				Console.WriteLine("Reading folder: {0}\n", fileOut);
				for (int i = 0; i < NUM_BLOCK; i++)
				{
					Console.WriteLine("Decrypting block {0}", i.ToString());
					
					Byte[] block = Get_Block(fileIn, i);
					block = Encryption(block);
					
					if (File.Exists(fileOut + "block" + i.ToString() + ".bin"))
						File.Delete(fileOut + "block" + i.ToString() + ".bin");
					File.WriteAllBytes(fileOut + "block" + i.ToString() + ".bin", block);
				}
			}
			else if (args[0] == "-e")
			{
				if (!Directory.Exists(fileIn))
				{
					Console.WriteLine("Input directory not found.");
					return;
				}
				
				string[] files = Directory.GetFiles(fileIn);
				byte[][] blocks = new byte[NUM_BLOCK][];
				for (int i = 0; i < NUM_BLOCK; i++)
				{
					string currFile = Array.Find(files, name => Path.GetFileNameWithoutExtension(name) == "block" + i.ToString());
					blocks[i] = File.ReadAllBytes(currFile);
					blocks[i] = Encryption(blocks[i]);
				}
				
				if (File.Exists(fileOut))
					File.Delete(fileOut);
				Write_File(blocks, fileOut);
			}
			else if (args[0] == "-ef")
			{
				if (!File.Exists(fileIn))
				{
					Console.WriteLine("Input file not found.");
					return;
				}
				if (args.Length != 5)
				{
					Console.WriteLine("You have to specified the number of block to encrypt and the original MSG.DAT file.");
					return;
				}
				
				string original = args[4];
				int ind = Convert.ToInt32(args[3]);
				
				byte[][] blocks = new byte[NUM_BLOCK][];
				for (int i = 0; i < NUM_BLOCK; i++)
				{
					if (i != ind)
						blocks[i] = Get_Block(original, i);
					else
					{
						blocks[i] = File.ReadAllBytes(fileIn);
						blocks[i] = Encryption(blocks[i]);
					}
				}
				
				if (File.Exists(fileOut))
					File.Delete(fileOut);
				Write_File(blocks, fileOut);
			}
			
			
			Console.Write("Done!");
		}
		public static void Help()
		{
			Console.WriteLine("\nUsage: Decrypt.exe -[Action] In Out <NumberOfBlock> <Original>");
			Console.WriteLine("\nAction:");
			Console.WriteLine("\t-d\tDecrypt the input file, In, to the folder Out");
			Console.WriteLine("\t-df\tDecrypt one block from the input file to the file Out");
			Console.WriteLine("\t-e\tEncrypt the input folder, In, to the file Out");
			Console.WriteLine("\t-ef\tEncrypt one block from the input file to the file Out");
			Console.WriteLine("\n<NumberOfBlock> is only required for -df and -ef");
			Console.WriteLine("<Original> is only required for -ef");
			Console.Write("\nGood hacking!");
		}

		public static void Write_File(Byte[][] data, string fileOut)
		{
			BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));
			
			// Write offset table
			uint offset = (uint)data.Length * 8;
			if (offset % 0x800 != 0)  // Padding
				offset += (0x800 - (offset % 0x800));

			for (int i = 0; i < data.Length; i++)
			{
				bw.Write(offset);
				bw.Write(data[i].Length);
				
				offset += (uint)data[i].Length;
				if (offset % 0x800 != 0)  // Padding
					offset += (0x800 - (offset % 0x800));
			}
			
			// Padding
			byte[] rem = new byte[0];
			if (bw.BaseStream.Position % 0x800 != 0)
				rem = new byte[0x800 - (bw.BaseStream.Position % 0x800)];
			bw.Write(rem);
			bw.Flush();
			
			// Write blocks
			for (int i = 0; i < data.Length; i++)
			{
				bw.Write(data[i]);
				
				// Padding
				rem = new byte[0];
				if (bw.BaseStream.Position % 0x800 != 0)
					rem = new byte[0x800 - (bw.BaseStream.Position % 0x800)];
				bw.Write(rem);
				bw.Flush();
			}
			
			bw.Flush();
			bw.Close();
		}
		public static Byte[] Get_Block(string file, int i)
		{
			BinaryReader br = new BinaryReader(File.OpenRead(file));
			br.BaseStream.Position = i * 8;
			
			uint offset = br.ReadUInt32();
			uint size = br.ReadUInt32();
			
			br.BaseStream.Position = offset;
			byte[] data = br.ReadBytes((int)size);
			br.Close();
			
			return data;
		}
		public static Byte[] Encryption(byte[] data)
		{
			int size = data.Length;
			int pos = 0;
			
			while (pos < size)
			{
				long mult = pos * MULT1;
				int key_offset = (int)(mult >> 32) >> 4;
				mult = key_offset * MULT2;
				key_offset = (int)mult >> 32;
				key_offset = pos - key_offset;
				
				byte value = data[pos];
				byte keyv = (byte)KEY[key_offset];
				value = (byte)(value ^ keyv);
				data[pos] = value;
				
				pos++;
			}
			
			return data;
		}
	}
}