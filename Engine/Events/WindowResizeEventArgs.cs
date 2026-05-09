using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.Events;

public class WindowResizeEventArgs : EventArgs {

    public Viewport Viewport;

    public WindowResizeEventArgs(Viewport viewport) {
        Viewport = viewport;
    }

}
