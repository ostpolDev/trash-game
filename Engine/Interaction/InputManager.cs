using Microsoft.Xna.Framework.Input;
using System.Drawing;

namespace Engine.Interaction;

public class InputManager {

    private KeyboardState CurrentState;
    private KeyboardState OldState;

    private MouseState CurrentMouseState;
    private MouseState OldMouseState;

    public Point MousePosition { get; private set; }

    public bool IsCtrlDown { get; private set; }
    public bool IsShiftDown { get; private set; }

    /// <summary>
    /// Call before other components that rely on input get updated
    /// </summary>
    public void Update() {
        CurrentState = Keyboard.GetState();
        CurrentMouseState = Mouse.GetState();

        IsCtrlDown = CurrentState.IsKeyDown(Keys.LeftControl);
        IsShiftDown = CurrentState.IsKeyDown(Keys.LeftShift);

        MousePosition = new(CurrentMouseState.X, CurrentMouseState.Y);
    }

    /// <summary>
    /// Call after all components relying on input got updated
    /// </summary>
    public void LateUpdate() {
        OldState = CurrentState;
        OldMouseState = CurrentMouseState;
    }

    /// <summary>
    /// If the given key is down for this frame for the first time.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool IsKeyDown(Keys key) {
        return CurrentState.IsKeyDown(key) && !OldState.IsKeyDown(key);
    }

    /// <summary>
    /// If the given key is down
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool IsKey(Keys key) {
        return CurrentState.IsKeyDown(key);
    }

    /// <summary>
    /// If the given key is up for the first time this frame
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool IsKeyUp(Keys key) {
        return !CurrentState.IsKeyDown(key) && OldState.IsKeyDown(key);
    }

    /// <summary>
    /// If the left mouse button is down for the first time this frame.
    /// </summary>
    /// <returns></returns>
    public bool IsLeftMouseDown() {
        return CurrentMouseState.LeftButton == ButtonState.Pressed && OldMouseState.LeftButton != ButtonState.Pressed;
    }

    /// <summary>
    /// If the left mouse button is up for the first time this frame.
    /// </summary>
    /// <returns></returns>
    public bool IsLeftMouseUp() {
        return CurrentMouseState.LeftButton == ButtonState.Released && OldMouseState.LeftButton != ButtonState.Released;
    }


}
