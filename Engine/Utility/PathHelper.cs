using System;
using System.IO;

namespace Engine.Utility;

public class PathHelper {

    public const string DATA_FILE_EXTENSION = ".save";
    public const string COMPRESSED_DATA_FILE_EXTENSION = ".csave";

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

    public static string GetContentPath() {
        return Path.Combine(Directory.GetCurrentDirectory(), "Content");
    }

    public static string GetCurrentDirectory() {
        return Directory.GetCurrentDirectory();
    }

    public static string MakeFileSafe(string str, char replacement = '-') {
        foreach (char c in Path.GetInvalidFileNameChars())
            str = str.Replace(c, replacement);
        return str;
    }

    public static string MakeDirSafe(string str, char replacement = '-') {
        foreach (char c in Path.GetInvalidPathChars())
            str = str.Replace(c, replacement);
        return str;
    }

    public static string EnsureValidSaveFileExtension(string filename, bool compress = true) {
        string ext = Path.GetExtension(filename);
        if (ext != DATA_FILE_EXTENSION && ext != COMPRESSED_DATA_FILE_EXTENSION) {
            if (compress) {
                filename = Path.ChangeExtension(filename, COMPRESSED_DATA_FILE_EXTENSION);
            } else {
                filename = Path.ChangeExtension(filename, DATA_FILE_EXTENSION);
            }
        }
        return filename;
    }

}
