using Engine.Utility;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Engine.Debugging;

public class CrashHandler {

    public static void Initialize() {
        TaskScheduler.UnobservedTaskException += (sender, e) => {
            LogCrashToFile(e.Exception);
        };

        AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
            LogCrashToFile(e.ExceptionObject as Exception);
        };
    }

    private static void LogCrashToFile(Exception exception) {
        if (exception == null) return;

        try {

            using StreamWriter streamWriter = new(Logger.GetCrashLogLocation(), true);
            streamWriter.WriteLine("===== Fatal Crash :( =====");
            streamWriter.WriteLine("The game has crashed unexpectedly. That is my bad (probably)");
            streamWriter.WriteLine("If you can, please submit this log file to the official game forum, or to contact@ostpol.dev so that I can hopefully get this fixed!");
            streamWriter.WriteLine("Sorry for the inconvenience");
            streamWriter.WriteLine("");
            streamWriter.WriteLine($"Caused by {exception.GetType().Name} - {exception.Message ?? "Unknown"}");
            streamWriter.WriteLine(exception.StackTrace ?? "No stack");
            streamWriter.WriteLine("");
            streamWriter.WriteLine("=== System information ===");
            streamWriter.WriteLine("This information is used to determine if the crash is related to your specific setup");
            streamWriter.WriteLine($"Crash date: {DateTime.Now:yyyy MMMM dd HH:mm:ss}");

            if (!HardwareSniffer.IsInitialized) {
                streamWriter.WriteLine("Unfortunately, no hardware information could be provided, as the collector has not yet been initialized.");
            } else {
                foreach (string item in HardwareSniffer.Information) {
                    streamWriter.WriteLine(item);
                }
            }

            streamWriter.WriteLine("");
            streamWriter.WriteLine("--------------------------");
            streamWriter.WriteLine("");

            streamWriter.Flush();



        } catch { }
    }

}
