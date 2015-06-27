//
//  Form1.Designer.cs
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
namespace Keyboard
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.numBaseAddress = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numRAMadd = new System.Windows.Forms.NumericUpDown();
            this.btnWrite = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.numPage = new System.Windows.Forms.NumericUpDown();
            this.numKey = new System.Windows.Forms.NumericUpDown();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numKeyCode = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.picKeyboard = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.numBaseAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRAMadd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeyCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picKeyboard)).BeginInit();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Base address:";
            //
            // numBaseAddress
            //
            this.numBaseAddress.Hexadecimal = true;
            this.numBaseAddress.InterceptArrowKeys = false;
            this.numBaseAddress.Location = new System.Drawing.Point(92, 12);
            this.numBaseAddress.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numBaseAddress.Name = "numBaseAddress";
            this.numBaseAddress.Size = new System.Drawing.Size(80, 20);
            this.numBaseAddress.TabIndex = 2;
            this.numBaseAddress.Value = new decimal(new int[] {
            36019064,
            0,
            0,
            0});
            this.numBaseAddress.ValueChanged += new System.EventHandler(this.numOveralyadd_ValueChanged);
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(199, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "RAM address:";
            //
            // numRAMadd
            //
            this.numRAMadd.Hexadecimal = true;
            this.numRAMadd.InterceptArrowKeys = false;
            this.numRAMadd.Location = new System.Drawing.Point(279, 12);
            this.numRAMadd.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numRAMadd.Name = "numRAMadd";
            this.numRAMadd.Size = new System.Drawing.Size(80, 20);
            this.numRAMadd.TabIndex = 4;
            this.numRAMadd.Value = new decimal(new int[] {
            35863584,
            0,
            0,
            0});
            this.numRAMadd.ValueChanged += new System.EventHandler(this.numOveralyadd_ValueChanged);
            //
            // btnWrite
            //
            this.btnWrite.Enabled = false;
            this.btnWrite.Location = new System.Drawing.Point(355, 207);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(75, 23);
            this.btnWrite.TabIndex = 5;
            this.btnWrite.Text = "Write";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            //
            // btnOpen
            //
            this.btnOpen.Location = new System.Drawing.Point(274, 207);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 6;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            //
            // numPage
            //
            this.numPage.Enabled = false;
            this.numPage.Location = new System.Drawing.Point(319, 55);
            this.numPage.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numPage.Name = "numPage";
            this.numPage.Size = new System.Drawing.Size(60, 20);
            this.numPage.TabIndex = 7;
            this.numPage.ValueChanged += new System.EventHandler(this.numKey_ValueChanged);
            //
            // numKey
            //
            this.numKey.Enabled = false;
            this.numKey.Hexadecimal = true;
            this.numKey.Location = new System.Drawing.Point(319, 81);
            this.numKey.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numKey.Name = "numKey";
            this.numKey.Size = new System.Drawing.Size(60, 20);
            this.numKey.TabIndex = 8;
            this.numKey.ValueChanged += new System.EventHandler(this.numKey_ValueChanged);
            //
            // txtKey
            //
            this.txtKey.Enabled = false;
            this.txtKey.Location = new System.Drawing.Point(317, 107);
            this.txtKey.MaxLength = 1;
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(62, 20);
            this.txtKey.TabIndex = 9;
            this.txtKey.TextChanged += new System.EventHandler(this.txtKey_TextChanged);
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(276, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Page:";
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(276, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Key:";
            //
            // label5
            //
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(276, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Char:";
            //
            // numKeyCode
            //
            this.numKeyCode.Enabled = false;
            this.numKeyCode.Hexadecimal = true;
            this.numKeyCode.Location = new System.Drawing.Point(319, 134);
            this.numKeyCode.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numKeyCode.Name = "numKeyCode";
            this.numKeyCode.ReadOnly = true;
            this.numKeyCode.Size = new System.Drawing.Size(60, 20);
            this.numKeyCode.TabIndex = 13;
            //
            // label6
            //
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(276, 136);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Code:";
            //
            // picKeyboard
            //
            this.picKeyboard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picKeyboard.Cursor = System.Windows.Forms.Cursors.Default;
            this.picKeyboard.Image = global::Keyboard.Properties.Resources.keyboard_bg;
            this.picKeyboard.InitialImage = null;
            this.picKeyboard.Location = new System.Drawing.Point(12, 38);
            this.picKeyboard.Name = "picKeyboard";
            this.picKeyboard.Size = new System.Drawing.Size(256, 192);
            this.picKeyboard.TabIndex = 0;
            this.picKeyboard.TabStop = false;
            this.picKeyboard.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picKeyboard_MouseClick);
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(441, 237);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numKeyCode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtKey);
            this.Controls.Add(this.numKey);
            this.Controls.Add(this.numPage);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnWrite);
            this.Controls.Add(this.numRAMadd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numBaseAddress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.picKeyboard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Keyboard changer - Pokemon Conquest    ## by pleonex ##";
            ((System.ComponentModel.ISupportInitialize)(this.numBaseAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRAMadd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeyCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picKeyboard)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picKeyboard;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numBaseAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numRAMadd;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.NumericUpDown numPage;
        private System.Windows.Forms.NumericUpDown numKey;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numKeyCode;
        private System.Windows.Forms.Label label6;
    }
}
