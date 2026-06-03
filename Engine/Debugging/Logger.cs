using Engine.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Engine.Debugging;

public class Logger {

    public enum Level { INFO, WARNING, ERROR, EXCEPTION };

    private static readonly Dictionary<string, Logger> loggerLookup = [];
    private static FileStream stream;
    private static int iteration = 0;
    private static readonly string logPath;

    public static readonly Logger Shared = Get("Generic");

    public static Logger Get(string name) {
        if (loggerLookup.TryGetValue(name, out Logger value))
            return value;

        Logger l = new(name);
        loggerLookup[name] = l;
        return l;
    }

    private static void WriteLine(string str) {
        if (str == null || str.Equals(string.Empty))
            return;

        System.Diagnostics.Debug.WriteLine(str);

        byte[] chars = Encoding.UTF8.GetBytes($"{str}\n");
        stream?.Write(chars);

        if (stream != null && stream.Length > 5e+6) {
            // roll over
            stream.Close();
            string latest = Path.Combine(logPath, $"latest.{iteration}.log");
            DateTime time = File.GetCreationTime(latest);
            string newPath = Path.Combine(logPath, $"{PathHelper.MakeFileSafe(time.ToString("yyyy-MM-dd HH:mm:ss"))}.{iteration}.log");
            if (File.Exists(newPath))
                newPath = Path.Combine(logPath, $"{PathHelper.MakeFileSafe(time.ToString("yyyy-MM-dd HH:mm:ss"))}.{iteration}.{Random.Shared.Next(9999)}.log");
            File.Move(latest, newPath);

            iteration++;
            string latestPath = Path.Combine(logPath, $"latest.{iteration}.log");
            stream = File.OpenWrite(latestPath);
        }
    }

    public static string GetCrashLogLocation() {
        return Path.Combine(logPath, $"fatal_crash.log");
    }

    public static string GetLogPath() {
        return logPath;
    }

    static Logger() {

        logPath = Path.Combine(PathHelper.GetAppDirectory(), "logs");
        if (!Directory.Exists(logPath))
            Directory.CreateDirectory(logPath);

        string latest = Path.Combine(logPath, $"latest.{iteration}.log");
        if (File.Exists(latest)) {
            DateTime time = File.GetCreationTime(latest);
            string newPath = Path.Combine(logPath, $"{PathHelper.MakeFileSafe(time.ToString("yyyy-MM-dd HH:mm:ss"))}.log");
            if (File.Exists(newPath))
                newPath = Path.Combine(logPath, $"{PathHelper.MakeFileSafe(time.ToString("yyyy-MM-dd HH:mm:ss"))}.{Random.Shared.Next(9999)}.log");
            File.Move(latest, newPath);
        }

        stream = File.OpenWrite(latest);
    }

    public static void Close() {
        stream?.Close();
    }

    public readonly string Name;

    private Logger(string name) {
        Name = name;
    }

    public Logger Child(string name) {
        return Get($"{Name}.{name}");
    }

    public void Info(object message) {
        WriteLine($"{DateTime.Now.ToLongTimeString()} [{Name}/INFO]: {message}");
    }

    public void Warning(object message) {
        WriteLine($"{DateTime.Now.ToLongTimeString()} [{Name}/WARN]: {message}");
    }

    public void Error(object message) {
        WriteLine($"{DateTime.Now.ToLongTimeString()} [{Name}/ERR]: {message}");
    }

    public void Exception(Exception ex) {
        WriteLine($"{DateTime.Now.ToLongTimeString()} [{Name}/EXCEPTION]: {ex.Message} ({ex.GetType().Name})");
        WriteLine(ex.StackTrace);
    }

    public void Exception(Exception ex, string message) {
        WriteLine($"{DateTime.Now.ToLongTimeString()} [{Name}/EXCEPTION]: {ex.Message} ({ex.GetType().Name})");
        WriteLine($"    {message}");
        WriteLine(ex.StackTrace);
    }


}
