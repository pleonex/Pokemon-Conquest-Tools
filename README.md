# Pokemon Conquest translation tools

Tools to translate the game Pokemon Conquest.

**These tools have been tested against the european (EUR) version, but they
should work after minor changes for other regions.**

## Export files

### Prerequisites

Before running the script to export files, follow these steps to setup the
folders:

1. Install .NET Core SDK 3.1 (or latest).
2. Open a console and run the following command in the repository folder:
   `dotnet build`
3. Download the latest version of
   [Tinke](https://github.com/pleonex/tinke/releases/latest)
4. Create the folders `GameData/root` in the repository.
5. Open the game ROM with Tinke, select the top node `root`, click in the button
   `Extract` and select the new folder from step 4 `root`.
6. Rename the folder inner folder `root` to `rom` so the files are in
   `GameData/root/rom`
7. Move the folder `GameData/root/rom/ftc` to be `GameData/root/system` and
   delete every file except `arm9.bin` and `overlay9_*`
8. Download the
   [CUE decompressor tools](https://www.romhacking.net/utilities/826/).
9. Decompress the ARM9 and the overlay files with the tool `blz.exe`. For
   instance: `blz.exe -d GameData/root/system/*`

### Run script

To export the game files to editable formats, run the following scripts. The
extracted files will be in `GameData/extracted`. The optional arguments are:

- `--game`: path to the _root_ folder with the game files.
- `--output`: output directory to extract files.

#### Windows

From PowerShell console:

```powershell
.\build.ps1 -script exporter.cake
```

From cmd.exe console:

```batch
powershell -ExecutionPolicy ByPass -File build.ps1 -script exporter.cake
```

#### Linux

```bash
./build.sh --script exporter.cake
```

## Import files

TODO
