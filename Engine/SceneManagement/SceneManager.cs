using Engine.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Engine.SceneManagement;

public class SceneManager(BaseGame game) : Component {

    private static readonly Logger Logger = Logger.Get("SceneManagement");
    private readonly Dictionary<string, Type> sceneTypeLookup = [];

    public Scene ActiveScene { get; private set; }

    private readonly BaseGame Game = game;

    public void RegisterScene(string name, Type scene) {
        if (!scene.IsAssignableTo(typeof(Scene))) {
            Logger.Error($"{scene.Name} is not assignable to {nameof(Scene)}");
            return;
        }

        sceneTypeLookup[name] = scene;
    }

    public void UnRegisterScene(string name) {
        sceneTypeLookup.Remove(name);
    }

    public T LoadScene<T>(string name, bool preserveUI = false) where T : Scene {
        if (!sceneTypeLookup.TryGetValue(name, out Type type)) {
            Logger.Error($"Scene with key \"{name}\" is not registered!");
            return default;
        }

        ActiveScene?.Unload();
        if (!preserveUI) {
            BaseGame.Instance?.UIManager?.ClearAll();
        }
        T scene = (T) Activator.CreateInstance(type);
        ActiveScene = scene;
        ActiveScene.LoadContent(Game.Content);
        return scene;
    }

    public async Task<T> LoadSceneAsyncWithTask<T>(string name, Task loadingScreenTask, bool preserveUI = false) where T : Scene {
        if (!sceneTypeLookup.TryGetValue(name, out Type type)) {
            Logger.Error($"Scene with key \"{name}\" is not registered!");
            return default;
        }

        ActiveScene?.Unload();
        if (!preserveUI) {
            BaseGame.Instance?.UIManager?.ClearAll();
        }
        await loadingScreenTask;
        T scene = (T)Activator.CreateInstance(type);
        ActiveScene = scene;
        ActiveScene.LoadContent(Game.Content);
        return scene;
    }

    public void SetScene(Scene scene, bool preserveUI = false) {
        ActiveScene?.Unload();
        if (!preserveUI) {
            BaseGame.Instance?.UIManager?.ClearAll();
        }
        ActiveScene = scene;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha) {
        ActiveScene?.Draw(gameTime, spriteBatch, alpha);
    }

    public override void FixedUpdate() {
        ActiveScene?.FixedUpdate();
    }

    public override void Update(GameTime gameTime, float delta) {
        ActiveScene?.Update(gameTime, delta);
    }
}

