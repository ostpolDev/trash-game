namespace Engine.UI.Debugging;

#if DEBUG

public abstract class UIDebugRenderer(AbstractUIComponent component) {

    public AbstractUIComponent Component = component;

    public abstract void Render();

}

#endif
