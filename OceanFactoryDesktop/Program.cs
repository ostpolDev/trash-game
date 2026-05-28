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

        CrashHandler.Initialize();

        bool isDevelopment = args.Contains("--dev");

        using var game = new OceanFactoryGame(isDevelopment);
        game.Run();

    }

}

