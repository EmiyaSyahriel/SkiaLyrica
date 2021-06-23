# SkiaLyrica
My SkiaSharp-based Single Source Lyric Music Player Test

## Build
### Pre-requiste
- .NET 5.0 SDK & Runtime
- .NET scripting tool (`dotnet script`) to merge the DLL into one
- [BASS Library](http://www.un4seen.com/) for your target platform
- ILRepack

### Compilation
- VS2019 way : Open the solution via Visual Studio 2019 the Compile
- Terminal way : Open your OS's terminal, `cd` to the `.csproj` folder, then do `dotnet build`

### Libraries used
- OpenTK (for OpenGL and GLFW Windowing)
- SkiaSharp (for Drawing)
- ManagedBass (for Audio Player)
- ILRepack (to Repack DLLs into one)

### About the `UnityPlayer.dll` in this project
This project did not have any affiliation with Unity Engine at all, this DLL actually contains repackaged libraries
of this project when is built using `Project Solution (.sln)` instead of the `C# Project File (.csproj)`.

You can change this DLL name by modifying the `DLLMerge.cs` (the post-build C# script). 

Actually, Modification to this post build script is mandatory if you uses the `Project Solution`, as ILRepack 
binary files have different paths between version and platform.

## Usage
```Warning : This project is mostly contains hardcoded source. To modify most of the functionality, edit the code then recompile```

- Edit the code and the post-build script as you need it, then build it.
- Copy the `Lyrica_Data` directory to the same directory as the compiled binary, 
- Modify the content of the `Lyrica_Data` directory to your heart content.
- Run the compiled binary
  - on Windows : `Lyrica.exe`
  - on Linux : Terminal - `dotnet Lyrica.dll`
  
### My Default Keybinds
- `Spacebar` : Play/Pause
- `Left` : Seek to previous one second
- `Right` : Seek to the next one second
- `Shift + Left` : Seek to previous ten second
- `Shift + Right` : Seek to the next ten second

## License
CC-BY Public Domain
