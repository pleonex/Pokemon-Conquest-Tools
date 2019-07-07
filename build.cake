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
#addin nuget:?package=Yarhl&version=3.0.0-alpha04
#addin nuget:?package=Yarhl.Media&version=3.0.0-alpha04
#addin nuget:?package=System.Drawing.Common&version=4.6.0-preview6.19303.8
#addin nuget:?package=YamlDotNet&version=6.0.0
#r "AmbitionConquest/bin/Debug/netstandard2.0/AmbitionConquest.dll"

using AmbitionConquest.Fonts;
using AmbitionConquest.Texts;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

var target = Argument("target", "Default");

public class BuildData
{
    public string Game { get; set; }

    public string OutputDirectory { get; set; }

    public string InternalDirectory { get { return $"{OutputDirectory}/internal"; } }

    public string ImageDirectory { get { return $"{OutputDirectory}/images"; } }

    public string FontDirectory { get { return $"{OutputDirectory}/fonts"; } }

    public string TextDirectory { get { return $"{OutputDirectory}/texts"; } }

    public string MessagesDirectory { get { return $"{TextDirectory}/messages"; } }

    public Node Root { get; set; }

    public Node GetNode(string path)
    {
        return Navigator.SearchNode(Root, $"/root/rom/{path}");
    }
}

Setup<BuildData>(setupContext => {
    return new BuildData {
        Game = Argument("game", System.IO.Path.Combine("GameData", "root")),
        OutputDirectory = Argument("output", "GameData/extracted"),
    };
});

Task("Open-Game")
    .Does<BuildData>(data =>
{
    data.Root = NodeFactory.FromDirectory(data.Game, "*", "root", true);
});

Task("Export-Font")
    .IsDependentOn("Open-Game")
    .Does<BuildData>(data =>
{
    var font = Navigator.SearchNode(data.Root, "/root/system/overlay9_11")
        .TransformWith<Font2Binary>()
        .GetFormatAs<Font>();
    ((BinaryFormat)ConvertFormat.ConvertWith<Font2Yaml>(font))
        .Stream.WriteTo($"{data.FontDirectory}/font.yml");
    ((IImage)ConvertFormat.ConvertWith<Font2Image>(font))
        .Image.Save($"{data.FontDirectory}/font.png");
});

Task("Export-TextLists")
    .IsDependentOn("Open-Game")
    .Does<BuildData>(data =>
{
    data.GetNode("data/Building.dat")
        .TransformWith<BlockText2Po, BlockTextFile>(BlockTextFile.Building)
        .TransformWith<Po2Binary>()
        .Stream.WriteTo($"{data.TextDirectory}/building.pot");

    data.GetNode("data/EventSpeaker.dat")
        .TransformWith<BlockText2Po, BlockTextFile>(BlockTextFile.EventSpeaker)
        .TransformWith<Po2Binary>()
        .Stream.WriteTo($"{data.TextDirectory}/event_speaker.pot");

    data.GetNode("data/Gimmick.dat")
        .TransformWith<BlockText2Po, BlockTextFile>(BlockTextFile.Gimmick)
        .TransformWith<Po2Binary>()
        .Stream.WriteTo($"{data.TextDirectory}/gimmick.pot");

    data.GetNode("data/Item.dat")
        .TransformWith<BlockText2Po, BlockTextFile>(BlockTextFile.Item)
        .TransformWith<Po2Binary>()
        .Stream.WriteTo($"{data.TextDirectory}/item.pot");

    data.GetNode("data/Kuni.dat")
        .TransformWith<BlockText2Po, BlockTextFile>(BlockTextFile.Kuni)
        .TransformWith<Po2Binary>()
        .Stream.WriteTo($"{data.TextDirectory}/kuni.pot");

    data.GetNode("data/Saihai.dat")
        .TransformWith<BlockText2Po, BlockTextFile>(BlockTextFile.Saihai)
        .TransformWith<Po2Binary>()
        .Stream.WriteTo($"{data.TextDirectory}/saihai.pot");

    data.GetNode("data/Tokusei.dat")
        .TransformWith<BlockText2Po, BlockTextFile>(BlockTextFile.Tokusei)
        .TransformWith<Po2Binary>()
        .Stream.WriteTo($"{data.TextDirectory}/tokusei.pot");

    data.GetNode("data/Waza.dat")
        .TransformWith<BlockText2Po, BlockTextFile>(BlockTextFile.Waza)
        .TransformWith<Po2Binary>()
        .Stream.WriteTo($"{data.TextDirectory}/waza.pot");
});

Task("Export-Messages")
    .IsDependentOn("Open-Game")
    .Does<BuildData>(data =>
{
    Node msgNode = data.GetNode("data/MSG.DAT")
        .TransformWith<Binary2Blocks>();
    foreach (var block in msgNode.Children) {
        string name = System.IO.Path.GetFileNameWithoutExtension(block.Name);
        block.TransformWith<Binary2BlockMessages>()
            .TransformWith<BlockMessages2Po>()
            .TransformWith<Po2Binary>()
            .Stream.WriteTo($"{data.MessagesDirectory}/{name}.pot");
    }
});

Task("Export")
    .IsDependentOn("Export-Font")
    .IsDependentOn("Export-TextLists")
    .IsDependentOn("Export-Messages");

Task("Default");

Information($"AmbitionConquest v{typeof(Message).Assembly.GetName().Version}");
Information("Editor for Pokémon Conquest ~~ by pleonex");
RunTarget(target);
