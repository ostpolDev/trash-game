using System;
using System.IO;

namespace Engine.Utility;

public class PathHelper {

    public const string DATA_FILE_EXTENSION = ".dat";
    public const string COMPRESSED_DATA_FILE_EXTENSION = ".gz";

    public static string GetAuthorDirectory() {
        string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Configuration.DEVELOPER_NAME);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        return dir;
    }

    public static string GetAppDirectory() {
        string dir = Path.Combine(GetAuthorDirectory(), Configuration.APP_NAME);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        return dir;
    }

    public static string GetRuntimeContentDirectory() {
        string dir = Path.Combine(GetAppDirectory(), "RuntimeContent");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        return dir;
    }

    public static string GetModsDirectory() {
        string dir = Path.Combine(GetAppDirectory(), "Mods");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        return dir;
    }

    public static string MakeFileSafe(string str, char replacement = '-') {
        foreach (char c in Path.GetInvalidFileNameChars())
            str = str.Replace(c, replacement);
        return str;
    }

}
