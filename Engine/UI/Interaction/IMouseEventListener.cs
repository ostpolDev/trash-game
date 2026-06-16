namespace Engine.UI.Interaction;

public interface IMouseEventListener {

    void MouseEnter(UIMouseEventArgs state);
    void MouseLeave(UIMouseEventArgs state);
    void MouseDown(UIMouseEventArgs state);
    void MouseUp(UIMouseEventArgs state);

}
