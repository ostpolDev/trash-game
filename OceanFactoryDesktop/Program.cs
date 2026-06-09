using Engine.Debugging;
using OceanFactory;

namespace OceanFactoryDesktop;

public class Program {

    public static void Main(string[] args) {

        CrashHandler.Initialize();

        using var game = new OceanFactoryGame(args);
        game.Run();

    }

}

