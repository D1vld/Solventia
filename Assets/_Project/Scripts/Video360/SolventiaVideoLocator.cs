using System;
using System.Collections.Generic;
using System.IO;

public static class SolventiaVideoLocator
{
    private static readonly string[] Names = { "Ttour_Completo.mp4", "Tour_Espacios.mp4" };

    public static string[] Find(IEnumerable<string> directories)
    {
        var result = new string[Names.Length];
        foreach (var directory in directories)
        {
            if (string.IsNullOrWhiteSpace(directory)) continue;
            for (int i = 0; i < Names.Length; i++)
            {
                if (result[i] != null) continue;
                try
                {
                    var path = Path.Combine(directory, Names[i]);
                    if (File.Exists(path)) result[i] = path;
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException) { }
            }
        }
        return result;
    }
}
