using Microsoft.Xna.Framework.Input;

namespace Engine.UI.Interaction;

public struct UIMouseEventArgs {

    public bool IsHovered;
    public bool IsClicked;
    public MouseState State;

}
