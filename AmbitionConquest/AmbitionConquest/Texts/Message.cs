//
//  Message.cs
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
namespace AmbitionConquest.Texts
{
    using System.Text;

    public class Message
    {
        public Message()
        {
            Text = new StringBuilder();
        }

        public int ID { get; set; }

        public StringBuilder Text { get; set; }

        public bool MultiText { get; set; }

        public override string ToString()
        {
            return string.Format("[Message: ID={0}, Text={1}]", ID, Text);
        }
    }
}
