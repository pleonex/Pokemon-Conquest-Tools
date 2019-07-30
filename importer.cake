//  Copyright (c) 2019 Benito Palacios Sanchez
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
#addin nuget:?package=Yarhl&version=3.0.0-alpha06
#addin nuget:?package=Yarhl.Media&version=3.0.0-alpha06
#r "AmbitionConquest/bin/Debug/netstandard2.0/AmbitionConquest.dll"

using AmbitionConquest.Fonts;
using AmbitionConquest.Texts;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

string target = Argument("target", "Default");

public class BuildData
{
    public string OutputDirectory { get; set; }

    public string InputDirectory { get; set; }

    public string TextDirectory { get { return $"{InputDirectory}/texts"; } }

    public string MessagesDirectory { get { return $"{InputDirectory}/messages"; } }
}

Setup<BuildData>(setupContext => {
    return new BuildData {
        InputDirectory = Argument("input", "GameData/translated"),
        OutputDirectory = Argument("output", "GameData/output"),
    };
});

Task("Import-TextLists")
    .Does<BuildData>(data =>
{
    NodeFactory.FromFile($"{data.TextDirectory}/building.po")
        .TransformWith<Po2Binary>()
        .TransformWith<Binary2TextList, TextListKind>(TextListKind.Building)
        .Stream.WriteTo($"{data.OutputDirectory}/data/Building.dat");

    NodeFactory.FromFile($"{data.TextDirectory}/event_speaker.po")
        .TransformWith<Po2Binary>()
        .TransformWith<Binary2TextList, TextListKind>(TextListKind.EventSpeaker)
        .Stream.WriteTo($"{data.OutputDirectory}/data/EventSpeaker.dat");

    NodeFactory.FromFile($"{data.TextDirectory}/gimmick.po")
        .TransformWith<Po2Binary>()
        .TransformWith<Binary2TextList, TextListKind>(TextListKind.Gimmick)
        .Stream.WriteTo($"{data.OutputDirectory}/data/Gimmick.dat");

    NodeFactory.FromFile($"{data.TextDirectory}/item.po")
        .TransformWith<Po2Binary>()
        .TransformWith<Binary2TextList, TextListKind>(TextListKind.Item)
        .Stream.WriteTo($"{data.OutputDirectory}/data/Item.dat");

    NodeFactory.FromFile($"{data.TextDirectory}/kuni.po")
        .TransformWith<Po2Binary>()
        .TransformWith<Binary2TextList, TextListKind>(TextListKind.Kuni)
        .Stream.WriteTo($"{data.OutputDirectory}/data/Kuni.dat");

    NodeFactory.FromFile($"{data.TextDirectory}/saihai.po")
        .TransformWith<Po2Binary>()
        .TransformWith<Binary2TextList, TextListKind>(TextListKind.Saihai)
        .Stream.WriteTo($"{data.OutputDirectory}/data/Saihai.dat");

    NodeFactory.FromFile($"{data.TextDirectory}/tokusei.po")
        .TransformWith<Po2Binary>()
        .TransformWith<Binary2TextList, TextListKind>(TextListKind.Tokusei)
        .Stream.WriteTo($"{data.OutputDirectory}/data/Tokusei.dat");

    NodeFactory.FromFile($"{data.TextDirectory}/waza.po")
        .TransformWith<Po2Binary>()
        .TransformWith<Binary2TextList, TextListKind>(TextListKind.Waza)
        .Stream.WriteTo($"{data.OutputDirectory}/data/Waza.dat");
});

Task("Import")
    .IsDependentOn("Import-TextLists");

Task("Default")
    .IsDependentOn("Import");

Information($"AmbitionConquest v{typeof(Message).Assembly.GetName().Version}");
Information("Editor for Pokémon Conquest ~~ by pleonex");
RunTarget(target);
