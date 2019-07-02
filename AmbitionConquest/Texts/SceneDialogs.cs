// SceneDialogs.cs
//
// Author:
//       Benito Palacios Sanchez <benito356@gmail.com>
//
// Copyright (c) 2019 Benito Palacios Sanchez
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Yarhl.FileFormat;

    public class SceneDialogs : IFormat
    {
        readonly IList<string> dialogs;

        public SceneDialogs()
        {
            dialogs = new List<string>();
            Dialogs = new ReadOnlyCollection<string>(dialogs);
        }

        public ReadOnlyCollection<string> Dialogs {
            get;
            private set;
        }

        public void Add(string dialog)
        {
            dialogs.Add(dialog);
        }
    }
}