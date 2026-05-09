using OceanFactory;

namespace OceanFactoryDesktop;

public class Program {

    public static void Main() {
        using var game = new OceanFactoryGame();
        game.Run();
    }

}

