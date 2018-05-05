﻿//
//  AssemblyInfo.cs
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
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Addins;

[assembly: AssemblyTitle("AmbitionConquest")]
[assembly: AssemblyDescription("Tool to export Pokemon Conquest files")]
[assembly: AssemblyCompany("GradienWords")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("2018 (c) Benito Palacios Sanchez (pleonex)")]
[assembly: AssemblyVersion("0.1.0")]
[assembly: AssemblyFileVersion("0.1.0")]
[assembly: CLSCompliant(true)]
[assembly: ComVisible(true)]


#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#elif RELEASE
[assembly: AssemblyConfiguration("Release")]
#endif

// Mono.Addins
[assembly: Addin("AmbitionConquest", "0.1")]
[assembly: AddinDependency("Yarhl", "1.0")]
