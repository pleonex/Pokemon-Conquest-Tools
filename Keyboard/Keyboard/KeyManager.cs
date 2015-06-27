//
//  KeyManager.cs
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
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Keyboard
{
    public class KeyManager
    {
        byte[] data;

        int base_offset = 0x02259B78;
        int ram_offset =  0x02233C20;

        const int X_0 = 7;
        const int Y_0 = 58;
        const int XY_INCREMENT = 19;
        const int MAX_X = 13 * 19 + X_0;

        string table_path = System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + "table.tbl";
        int[,] table;   // Game code - Unicode code

        public KeyManager(string overlay, int base_offset, int ram_offset)
        {
            this.data = File.ReadAllBytes(overlay);
            this.base_offset = base_offset;
            this.ram_offset = ram_offset;

            table = new int[0, 2];
            if (File.Exists(table_path))
                Read_Table();
        }

        public void Write(string fileOut)
        {
            if (File.Exists(fileOut))
                File.Delete(fileOut);
            File.WriteAllBytes(fileOut, data);
        }

        private void Read_Table()
        {
            string[] lines = File.ReadAllLines(table_path);
            table = new int[lines.Length, 2];

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length != 6)
                    continue;

                table[i, 0] = Convert.ToInt32(lines[i].Substring(0, 4), 16);
                char c = lines[i][5];
                table[i, 1] = Decode_Unicode(c);
            }
        }

        private int Get_Offset(int page, int key)
        {
            int r0 = page + (page << 6);
            int offset = base_offset + (r0 << 2);
            offset += key << 2;
            offset -= ram_offset;

            if (offset < 0 || offset + 4 >= data.Length)
                throw new FormatException("Invalid offset!\nCheck the address of the overlay.");

            int char_offset = BitConverter.ToInt32(data, offset);
            char_offset -= ram_offset;

            if (char_offset < 0 || char_offset + 4 >= data.Length)
                throw new FormatException("Invalid offset!\nCheck the address of the overlay.");

            return char_offset;
        }

        public int Get_KeyFromPos(Point pt)
        {
            int x = X_0;
            int y = Y_0;

            for (int i = 0; i <= 0x40; i++)
            {
                if ((pt.X >= x && pt.X < x + XY_INCREMENT) &&
                    (pt.Y >= y && pt.Y < y + XY_INCREMENT))
                    return i;

                x += XY_INCREMENT;
                if (x + XY_INCREMENT > MAX_X)
                {
                    x = X_0;
                    y += XY_INCREMENT;
                }
            }

            return -1;
        }

        public char Get_Key(int page, int key)
        {
            int code;
            return Get_Key(page, key, out code);
        }
        public char Get_Key(int page, int key, out int code)
        {
            code = 0;

            if (page > 2 || key > 0x40 || page < 0 || key < 0)
                return '?';

            int char_offset = Get_Offset(page, key);
            code = BitConverter.ToInt32(data, char_offset);

            return Decode(code);
        }

        public void Change_Key(int page, int key, char c)
        {
            int char_offset = Get_Offset(page, key);
            int v = Encode(c);
            byte[] b = BitConverter.GetBytes(v);

            for (int i = 0; i < b.Length; i++)
                data[char_offset + i] = b[i];
        }

        public Image Get_Image(Image bg, int page)
        {
            Bitmap img = new Bitmap(bg.Width, bg.Height);
            Graphics graphic = Graphics.FromImage(img);
            graphic.DrawImageUnscaled(bg, 0, 0);

            int x = X_0;
            int y = Y_0;

            for (int i = 0; i <= 0x40; i++)
            {
                graphic.DrawString(
                    Get_Key(page, i).ToString(),
                    SystemFonts.DefaultFont,
                    Brushes.Black,
                    x, y);

                // Update coordinate
                x += XY_INCREMENT;
                if (x + XY_INCREMENT > MAX_X)
                {
                    x = X_0;
                    y += XY_INCREMENT;
                }
            }

            return img;
        }
        public Image Get_Image(Image bg, int page, Image select, int key)
        {
            Bitmap img = new Bitmap(bg.Width, bg.Height);
            Graphics graphic = Graphics.FromImage(img);
            graphic.DrawImageUnscaled(bg, 0, 0);

            int x = X_0;
            int y = Y_0;

            for (int i = 0; i <= 0x40; i++)
            {
                graphic.DrawString(
                    Get_Key(page, i).ToString(),
                    SystemFonts.DefaultFont,
                    Brushes.Black,
                    x, y);

                if (i == key && select != null && select is Image)
                {
                    ((Bitmap)select).MakeTransparent();
                    graphic.DrawImageUnscaled(select, x - 3, y - 3);
                }

                // Update coordinate
                x += XY_INCREMENT;
                if (x + XY_INCREMENT > MAX_X)
                {
                    x = X_0;
                    y += XY_INCREMENT;
                }
            }

            return img;
        }

        private int Decode_Unicode(char c)
        {
            byte[] b = Encoding.Unicode.GetBytes(c.ToString());
            return BitConverter.ToInt16(b, 0);
        }
        private char Decode(int v)
        {
            for (int i = 0; i < table.Length / 2; i++)
            {
                if (v == table[i, 0])
                    return Encoding.Unicode.GetChars(BitConverter.GetBytes(table[i, 1]))[0];
            }

            return Encoding.GetEncoding(932).GetChars(BitConverter.GetBytes(v))[0];
        }
        private int Encode(char c)
        {
            int v = Decode_Unicode(c);
            for (int i = 0; i < table.Length / 2; i++)
            {
                if (v == table[i, 1])
                    return table[i, 0];
            }

            return BitConverter.ToInt32(Encoding.GetEncoding(932).GetBytes(c.ToString()),0);
        }
    }
}
