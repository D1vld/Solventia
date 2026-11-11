using System;
using System.Collections.Generic;
using System.IO;

public static class SolventiaVideoLocator
{
    public const string CompletoFile = "Ttour_Completo.mp4";
    public const string EspaciosFile = "Tour_Espacios.mp4";

    public static readonly string[] Names = { CompletoFile, EspaciosFile };

    public static IEnumerable<string> Directories(string folderPath)
    {
        return new[]
        {
            "/storage/emulated/0/Managed files",
            folderPath,
            "/storage/emulated/0/Movies/360Videos",
            "/storage/emulated/0/Movies"
        };
    }

    public static string Find(string fileName, IEnumerable<string> directories)
    {
        if (string.IsNullOrWhiteSpace(fileName) || directories == null) return null;

        foreach (var directory in directories)
        {
            if (string.IsNullOrWhiteSpace(directory)) continue;
            try
            {
                var path = Path.Combine(directory, fileName);
                if (File.Exists(path)) return path;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException) { }
        }

        return null;
    }

    public static string[] FindAll(IEnumerable<string> directories)
    {
        var result = new string[Names.Length];
        for (int i = 0; i < Names.Length; i++)
            result[i] = Find(Names[i], directories);
        return result;
    }

    public static int IndexOf(string fileName)
    {
        for (int i = 0; i < Names.Length; i++)
            if (Names[i] == fileName) return i;
        return -1;
    }

    public static string NameAt(int index)
    {
        if (index < 0 || index >= Names.Length) return null;
        return Names[index];
    }
}
