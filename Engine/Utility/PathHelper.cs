using System;
using System.IO;

namespace Engine.Utility;

public class PathHelper {

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

    public static string MakeFileSafe(string str, char replacement = '-') {
        foreach (char c in Path.GetInvalidFileNameChars())
            str = str.Replace(c, replacement);
        return str;
    }

}
