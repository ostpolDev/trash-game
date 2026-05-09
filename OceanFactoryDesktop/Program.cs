using Engine.Debugging;
using Engine.Utility;
using OceanFactory;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OceanFactoryDesktop;

public class Program {

    public static void Main(string[] args) {

        // Catch un-caught errors

        TaskScheduler.UnobservedTaskException += (sender, e) => {
            LogToFile(e.Exception);
        };

        AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
            LogToFile(e.ExceptionObject as Exception);
        };

        // Check flags

        bool isDevelopment = args.Contains("--dev");

        using var game = new OceanFactoryGame(isDevelopment);
        game.Run();

    }

    static void LogToFile(Exception ex) {
        if (ex == null) return;

        try {

            using StreamWriter streamWriter = new(Logger.GetCrashLogLocation(), true);
            streamWriter.WriteLine("===== Fatal Crash :( =====");
            streamWriter.WriteLine("The game has crashed unexpectedly. That is my bad (probably)");
            streamWriter.WriteLine("If you can, please submit this log file to the official game forum, or to contact@ostpol.dev so that I can hopefully get this fixed!");
            streamWriter.WriteLine("Sorry for the inconvenience");
            streamWriter.WriteLine("");
            streamWriter.WriteLine($"Caused by {ex.GetType().Name} - {ex.Message ?? "Unknown"}");
            streamWriter.WriteLine(ex.StackTrace ?? "No stack");
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

