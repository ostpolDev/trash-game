using Microsoft.Xna.Framework.Input;

namespace Engine.UI;

public interface IMouseEventListener {

    void MouseEnter(MouseState state);
    void MouseLeave(MouseState state);
    void MouseDown(MouseState state);
    void MouseUp(MouseState state);

}
