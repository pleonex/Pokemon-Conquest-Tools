// Message.cs
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
namespace AmbitionConquest.Texts
{
    public class Message
    {
        public int GroupId { get; set; }

        public int ElementId { get; set; }

        public string Text { get; set; }

        public string Context { get; set; }

        public string BoxConfig { get; set; }

        public bool IsEmpty {
            get {
                return string.IsNullOrEmpty(Text)
                    && string.IsNullOrEmpty(Context)
                    && string.IsNullOrEmpty(BoxConfig);
            }
        }
    }
}
