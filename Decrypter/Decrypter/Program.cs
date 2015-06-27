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
    public class Program
    {
        private const int NumBlocks = 0x2C;                // 0x21 for Pokemon Conquest

        public static void Main(string[] args)
        {
            Console.WriteLine("Pokemon - Nobunaga Ambition");
            Console.WriteLine("Decrypter for MSG.DAT by pleoNeX");

            if (args.Length < 3) {
                Help();
                return;
            }

            string fileIn  = args[1];
            string fileOut = args[2];

            if (args[0] == "-df") {
                if (!File.Exists(fileIn)) {
                    Console.WriteLine("Input file not found.");
                    return;
                }

                if (args.Length != 4) {
                    Console.WriteLine("Missing block number to decrypt.");
                    return;
                }

                int blockNumber = Convert.ToInt32(args[3]);
                Console.WriteLine("Decrypting block {0}", args[3]);

                byte[] block = GetBlock(fileIn, blockNumber);
                Encryption(block);

                File.WriteAllBytes(fileOut, block);
            } else if (args[0] == "-d") {
                if (!File.Exists(fileIn)) {
                    Console.WriteLine("Input file not found.");
                    return;
                }

                if (!Directory.Exists(fileOut))
                    fileOut = Directory.CreateDirectory(fileOut).FullName;

                if (fileOut[fileOut.Length - 1] != Path.DirectorySeparatorChar)
                    fileOut += Path.DirectorySeparatorChar;

                Console.WriteLine("Reading folder: {0}\n", fileOut);
                for (int i = 0; i < NumBlocks; i++) {
                    Console.WriteLine("Decrypting block {0}", i.ToString());

                    Byte[] block = GetBlock(fileIn, i);
                    Encryption(block);

                    if (File.Exists(fileOut + "block" + i.ToString() + ".bin"))
                        File.Delete(fileOut + "block" + i.ToString() + ".bin");
                    File.WriteAllBytes(fileOut + "block" + i.ToString() + ".bin", block);
                }
            } else if (args[0] == "-e") {
                if (!Directory.Exists(fileIn)) {
                    Console.WriteLine("Input directory not found.");
                    return;
                }

                string[] files = Directory.GetFiles(fileIn);
                byte[][] blocks = new byte[NumBlocks][];
                for (int i = 0; i < NumBlocks; i++) {
                    string currFile = Array.Find(
                        files,
                        name => Path.GetFileNameWithoutExtension(name) == "block" + i.ToString());
                    blocks[i] = File.ReadAllBytes(currFile);
                    Encryption(blocks[i]);
                }
                    
                WriteFile(blocks, fileOut);
            } else if (args[0] == "-ef") {
                if (!File.Exists(fileIn)) {
                    Console.WriteLine("Input file not found.");
                    return;
                }

                if (args.Length != 5) {
                    Console.WriteLine("Missing block number and path to MSG.DAT file.");
                    return;
                }

                string original = args[4];
                int ind = Convert.ToInt32(args[3]);

                byte[][] blocks = new byte[NumBlocks][];
                for (int i = 0; i < NumBlocks; i++) {
                    if (i != ind) {
                        blocks[i] = GetBlock(original, i);
                    } else {
                        blocks[i] = File.ReadAllBytes(fileIn);
                        Encryption(blocks[i]);
                    }
                }
                    
                WriteFile(blocks, fileOut);
            }


            Console.Write("Done!");
        }

        private static void Help()
        {
            Console.WriteLine("\nUsage: Decrypt [Action] In Out <NumberOfBlock> <Original>");
            Console.WriteLine("\nAction:");
            Console.WriteLine("\t-d\tDecrypt the input file, In, to the folder Out");
            Console.WriteLine("\t-df\tDecrypt one block from the input file to the file Out");
            Console.WriteLine("\t-e\tEncrypt the input folder, In, to the file Out");
            Console.WriteLine("\t-ef\tEncrypt one block from the input file to the file Out");
            Console.WriteLine("\n<NumberOfBlock> is only required for -df and -ef");
            Console.WriteLine("<Original> is only required for -ef");
            Console.Write("\nGood hacking!");
        }

        private static void WriteFile(byte[][] data, string fileOut)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            // Write offset table
            uint offset = (uint)data.Length * 8;
            if (offset % 0x800 != 0)  // Padding
                offset += (0x800 - (offset % 0x800));

            for (int i = 0; i < data.Length; i++) {
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
            for (int i = 0; i < data.Length; i++) {
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

        private static byte[] GetBlock(string file, int i) {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            br.BaseStream.Position = i * 8;

            uint offset = br.ReadUInt32();
            uint size   = br.ReadUInt32();

            br.BaseStream.Position = offset;
            byte[] data = br.ReadBytes((int)size);
            br.Close();

            return data;
        }

        private static void Encryption(byte[] data)
        {
            const string Key = "MsgLinker Ver1.00";
            for (int i = 0; i < data.Length; i++)
                data[i] ^= (byte)Key[i % Key.Length];
        }
    }
}
