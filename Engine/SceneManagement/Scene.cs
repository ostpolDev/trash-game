using Engine.Debugging;
using Microsoft.Xna.Framework.Content;
using System;

namespace Engine.SceneManagement;

public abstract class Scene(string name) : Component {

    protected readonly static Logger Logger = Logger.Get("Scenes");

    public event EventHandler<EventArgs> OnDoneLoading;
    public event EventHandler<EventArgs> OnUnload;

    public string Name { get; private set; } = name;

    public virtual void Unload() {
        OnUnload?.Invoke(this, new());
    }

    public virtual void Load() {
        OnBeginLoad();
        OnDoneLoading?.Invoke(this, new());
        Logger.Info($"Scene \"{Name}\" loaded");
    }

    protected abstract void OnBeginLoad();

    public abstract void LoadContent(ContentManager contentManager);

}
