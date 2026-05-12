using Microsoft.Xna.Framework.Content;
using System;

namespace Engine.Events;

public class LoadContentEventArgs(ContentManager contentManager) : EventArgs {

    public ContentManager ContentManager = contentManager;
}
