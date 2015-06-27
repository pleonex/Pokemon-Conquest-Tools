//
//  Form1.cs
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Keyboard
{
    public partial class Form1 : Form
    {
        bool stop;
        string ov;
        KeyManager km;

        public Form1()
        {
            InitializeComponent();
        }

        private void numKey_ValueChanged(object sender, EventArgs e)
        {
            stop = true;
            int code;

            try { txtKey.Text = km.Get_Key((int)numPage.Value, (int)numKey.Value, out code).ToString(); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                numKey.Enabled = false;
                numPage.Enabled = false;
                txtKey.Enabled = false;
                btnWrite.Enabled = false;
                return;
            }

            numKeyCode.Value = code;
            stop = false;

            picKeyboard.Image = km.Get_Image(Properties.Resources.keyboard_bg, (int)numPage.Value,
                Properties.Resources.keyboard_select2, (int)numKey.Value);

            numKey.Enabled = true;
            numPage.Enabled = true;
            txtKey.Enabled = true;
            btnWrite.Enabled = true;
        }
        private void txtKey_TextChanged(object sender, EventArgs e)
        {
            if (stop || txtKey.Text.Length != 1)
                return;

            km.Change_Key((int)numPage.Value, (int)numKey.Value, txtKey.Text[0]);

            numKey_ValueChanged(null, null);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.FileName = "overlay9_12";
            if (o.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            ov = o.FileName;
            km = new KeyManager(ov, (int)numBaseAddress.Value, (int)numRAMadd.Value);

            o.Dispose();
            o = null;

            numKey.Enabled = true;
            numPage.Enabled = true;
            txtKey.Enabled = true;
            btnWrite.Enabled = true;

            numKey_ValueChanged(null, null);
        }
        private void btnWrite_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.FileName = "new_overlay9_12";
            o.OverwritePrompt = true;
            if (o.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            km.Write(o.FileName);

            o.Dispose();
            o = null;
        }

        private void picKeyboard_MouseClick(object sender, MouseEventArgs e)
        {
            if (!numKey.Enabled)
                return;

            int key = km.Get_KeyFromPos(e.Location);
            if (key == -1)
                return;

            numKey.Value = key;
        }

        private void numOveralyadd_ValueChanged(object sender, EventArgs e)
        {
            if (ov == null || ov == "")
                return;

            km = new KeyManager(ov, (int)numBaseAddress.Value, (int)numRAMadd.Value);
            numKey_ValueChanged(null, null);
        }

    }
}
