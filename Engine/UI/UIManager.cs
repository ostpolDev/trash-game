namespace Engine.UI;

public class UIManager {

    public static UIManager Singleton { get; private set; }

    public UIManager() {
        Singleton = this;
    }

}
