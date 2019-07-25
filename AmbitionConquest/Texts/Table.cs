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
    using System.IO;
    using System.Reflection;
    using System.Text;
    using Yarhl.Media.Text;

    public class Table
    {
        const string TablePath = "AmbitionConquest.Resources.table.tbl";
        static readonly object InstanceLock = new object();

        static Table instance;

        readonly Replacer replacer;

        private Table()
        {
            replacer = new Replacer();
            FillTable();
        }

        public static Table Instance {
            get {
                if (instance == null) {
                    lock (InstanceLock) {
                        if (instance == null) {
                            instance = new Table();
                        }
                    }
                }

                return instance;
            }
        }

        public string Decode(string text)
        {
            return replacer.Transform(text, true);
        }

        public string Encode(string text)
        {
            return replacer.Transform(text, false);
        }

        void FillTable()
        {
            Encoding shiftjis = Encoding.GetEncoding("shift_jis");

            Stream stream = null;
            try {
                stream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(TablePath);
                var reader = new StreamReader(stream);

                while (!reader.EndOfStream) {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    if (line[0] == '#')
                        continue;

                    string[] fields = line.Split('=');
                    replacer.Add(fields[0], fields[1]);
                }
            } finally {
                stream?.Dispose();
            }
        }
    }
}
