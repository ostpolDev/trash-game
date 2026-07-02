using Engine.Debugging;
using Engine.Events;
using Microsoft.Xna.Framework.Input;
using System;
using System.Drawing;

namespace Engine.Interaction;

public class InputManager {

    private readonly Logger Logger = Logger.Get("InputManager");

    public event EventHandler<InputMethodChangedEventArgs> OnInputMethodChanged;

    public KeyboardState CurrentKeyboardState {get; private set;}
    public KeyboardState OldKeyboardState {get; private set;}

    public MouseState CurrentMouseState {get; private set;}
    public MouseState OldMouseState {get; private set;}

    public GamePadState CurrentGamepadState { get; private set; }
    public GamePadState OldGamepadState {get; private set;}

    public Point MousePosition { get; private set; }

    public bool IsCtrlDown { get; private set; }
    public bool IsShiftDown { get; private set; }

    public InputMethod CurrentInputMethod { get; private set; } = InputMethod.CONTROLLER;
    public ControllerBrand CurrentControllerBrand { get; private set; } = ControllerBrand.NONE;


    /// <summary>
    /// Call before other components that rely on input get updated
    /// </summary>
    public void Update() {
        CurrentKeyboardState = Keyboard.GetState();
        CurrentMouseState = Mouse.GetState();
        CurrentGamepadState = GamePad.GetState(0);

        IsCtrlDown = CurrentKeyboardState.IsKeyDown(Keys.LeftControl);
        IsShiftDown = CurrentKeyboardState.IsKeyDown(Keys.LeftShift);

        if (CurrentInputMethod != InputMethod.CONTROLLER && CurrentGamepadState != OldGamepadState) {
            CurrentControllerBrand = GetControllerBrandFromName(GamePad.GetCapabilities(0).DisplayName);
            Logger.Info($"Changing to Gamepad input: {CurrentControllerBrand}");
            CurrentInputMethod = InputMethod.CONTROLLER;
            OnInputMethodChanged?.Invoke(this, new(CurrentInputMethod, CurrentControllerBrand));
        } else if (CurrentInputMethod != InputMethod.MOUSE_KEYBOARD && (CurrentMouseState != OldMouseState || CurrentKeyboardState != OldKeyboardState)) {
            Logger.Info("Changing to Mouse & Keyboard input");
            CurrentInputMethod = InputMethod.MOUSE_KEYBOARD;
            CurrentControllerBrand = ControllerBrand.NONE;
            OnInputMethodChanged?.Invoke(this, new(CurrentInputMethod, CurrentControllerBrand));
        }


        MousePosition = new(CurrentMouseState.X, CurrentMouseState.Y);
    }

    /// <summary>
    /// Call after all components relying on input got updated
    /// </summary>
    public void LateUpdate() {
        OldKeyboardState = CurrentKeyboardState;
        OldMouseState = CurrentMouseState;
        OldGamepadState = CurrentGamepadState;
    }

    #region Key & Mouse Helpers

    /// <summary>
    /// If the given key is down for this frame for the first time.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool IsKeyDown(Keys key) {
        return CurrentKeyboardState.IsKeyDown(key) && !OldKeyboardState.IsKeyDown(key);
    }

    /// <summary>
    /// If the given key is down
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool IsKey(Keys key) {
        return CurrentKeyboardState.IsKeyDown(key);
    }

    /// <summary>
    /// If the given key is up for the first time this frame
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool IsKeyUp(Keys key) {
        return !CurrentKeyboardState.IsKeyDown(key) && OldKeyboardState.IsKeyDown(key);
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

    /// <summary>
    /// If the right mouse button is down for the first time this frame.
    /// </summary>
    /// <returns></returns>
    public bool IsRightMouseDown() {
        return CurrentMouseState.RightButton == ButtonState.Pressed && OldMouseState.RightButton != ButtonState.Pressed;
    }

    /// <summary>
    /// If the right mouse button is up for the first time this frame.
    /// </summary>
    /// <returns></returns>
    public bool IsRightMouseUp() {
        return CurrentMouseState.RightButton == ButtonState.Released && OldMouseState.RightButton != ButtonState.Released;
    }

    /// <summary>
    /// If the middle mouse button is down for the first time this frame.
    /// </summary>
    /// <returns></returns>
    public bool IsMiddleMouseDown() {
        return CurrentMouseState.MiddleButton == ButtonState.Pressed && OldMouseState.MiddleButton != ButtonState.Pressed;
    }

    /// <summary>
    /// If the middle mouse button is up for the first time this frame.
    /// </summary>
    /// <returns></returns>
    public bool IsMiddleMouseUp() {
        return CurrentMouseState.MiddleButton == ButtonState.Released && OldMouseState.MiddleButton != ButtonState.Released;
    }

    /// <summary>
    /// If the x1 mouse button is down for the first time this frame.
    /// </summary>
    /// <returns></returns>
    public bool IsX1MouseDown() {
        return CurrentMouseState.XButton1 == ButtonState.Pressed && OldMouseState.XButton1 != ButtonState.Pressed;
    }

    /// <summary>
    /// If the x1 mouse button is up for the first time this frame.
    /// </summary>
    /// <returns></returns>
    public bool IsX1MouseUp() {
        return CurrentMouseState.XButton1 == ButtonState.Released && OldMouseState.XButton1 != ButtonState.Released;
    }

    /// <summary>
    /// If the x2 mouse button is down for the first time this frame.
    /// </summary>
    /// <returns></returns>
    public bool IsX2MouseDown() {
        return CurrentMouseState.XButton2 == ButtonState.Pressed && OldMouseState.XButton2 != ButtonState.Pressed;
    }

    /// <summary>
    /// If the x2 mouse button is up for the first time this frame.
    /// </summary>
    /// <returns></returns>
    public bool IsX2MouseUp() {
        return CurrentMouseState.XButton2 == ButtonState.Released && OldMouseState.XButton2 != ButtonState.Released;
    }

    #endregion

    #region Utility

    private static ControllerBrand GetControllerBrandFromName(string name) {
        if (string.IsNullOrEmpty(name)) return ControllerBrand.XBOX;

        if (name.Contains("Xbox", StringComparison.OrdinalIgnoreCase)) {
            return ControllerBrand.XBOX;
        }

        if (name.Contains("Nintendo", StringComparison.OrdinalIgnoreCase)) {
            return ControllerBrand.NINTENDO;
        }

        if (name.Contains("Steam", StringComparison.OrdinalIgnoreCase)) {
            return ControllerBrand.STEAM;
        }

        if (name.Contains("PS", StringComparison.OrdinalIgnoreCase) || name.Contains("PlayStation", StringComparison.OrdinalIgnoreCase)) {
            return ControllerBrand.PLAYSTATION;
        }

        return ControllerBrand.XBOX;
    }

    #endregion

}
