using System.IO;
using System;
using System.Text;
using System.Diagnostics;

string initDir = Args[0];

Console.WriteLine("Repacking as UnityPlayer Started");
Console.WriteLine("Creating Temporary Directory");

var tmpDir = Directory.Exists(".mergetmp") ? new DirectoryInfo(".mergetmp") : Directory.CreateDirectory(".mergetmp");

var userprofile = Environment.GetEnvironmentVariable("USERPROFILE");
var ilRepackExe = $"{userprofile}\\.nuget\\packages\\ilrepack\\2.0.18\\tools\\ILRepack.exe";
var dlls = Directory.GetFiles(initDir, "*.dll");
StringBuilder sb = new StringBuilder();
sb.Append($"/out:{initDir}\\UnityPlayer.dll");

Console.WriteLine("Querying DLLs...");

foreach(var dll in dlls)
{
    string dllFileName = Path.GetFileName(dll);
    if(dllFileName != "Lyrica.dll" && dllFileName != "UnityPlayer.dll")
    {
        var tmpDllName = $"{tmpDir.FullName}\\{dllFileName}";
        File.Move(dll, tmpDllName);
        sb.Append(" ").Append(tmpDllName);
    }
}

Console.WriteLine("Appending DLLs...");
var proc = Process.Start(ilRepackExe, sb.ToString());
proc.WaitForExit();
Console.WriteLine("Deleting Initial DLLs...");
tmpDir.Delete(true);

Console.WriteLine("Repacking as UnityPlayer finished");