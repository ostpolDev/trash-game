using Engine.Debugging;
using OceanFactory;
using System;
using System.Linq;

namespace OceanFactoryDesktop;

public class Program {

    public static void Main(string[] args) {

        CrashHandler.Initialize();

        bool isDevelopment = args.Contains("--dev");

        using var game = new OceanFactoryGame(isDevelopment);
        game.Run();

    }

}

