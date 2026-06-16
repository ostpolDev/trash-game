using Engine.UI.Components;
using Microsoft.Xna.Framework.Input;

namespace Engine.UI.Interaction;

internal class MouseEventWrapper(AbstractUIComponent component, IMouseEventListener listener) {

    public readonly AbstractUIComponent Component = component ?? throw new System.ArgumentNullException(nameof(component));
    public readonly IMouseEventListener MouseEventListener = listener;

    public bool IsHovered { get; private set; } = false;
    public bool IsClicked { get; private set; } = false;

    public void Update(MouseState currentState, MouseState oldState) {
        if (Component.Contains(currentState.Position)) {
            if (!IsHovered) {
                IsHovered = true;
                MouseEventListener.MouseEnter(GetState(currentState));
            }

            if (!IsClicked && currentState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released) {
                IsClicked = true;
                MouseEventListener.MouseDown(GetState(currentState));
            } else if (IsClicked && currentState.LeftButton == ButtonState.Released) {
                IsClicked = false;
                MouseEventListener.MouseUp(GetState(currentState));
            }
        } else {
            if (IsHovered) {
                IsHovered = false;
                MouseEventListener.MouseLeave(GetState(currentState));
            }
            if (IsClicked) {
                IsClicked = false;
                MouseEventListener.MouseUp(GetState(currentState));
            }
        }
    }

    private UIMouseEventArgs GetState(MouseState state) {
        return new() { IsClicked = IsClicked, IsHovered = IsHovered, State = state };
    }

}
