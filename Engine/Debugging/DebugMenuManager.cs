using Engine.Editors;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Engine.Debugging;

public class DebugMenuManager {

    private static readonly Logger Logger = Logger.Get("Debug");

    public static DebugMenuManager Singleton { get; private set; }

    private static Viewport Viewport;

    private static readonly Dictionary<string, Type> MenuLookup = [];
    private static readonly List<DebugMenu> EnabledMenus = [];

#if DEBUG
    private static readonly Dictionary<string, Type> EditorLookup = [];
    public static EditorScene ActiveEditorScene { get; private set; }
#endif

    private readonly ImGuiRenderer _imGuiRenderer;

    public static bool IsShowingWindows { get { return EnabledMenus.Count > 0; } }
    public static int ShownWindowCount { get { return EnabledMenus.Count; } }

    public static bool EnableWindowDrawing = true;

    public static Viewport WindowViewport { get; private set; }

    public DebugMenuManager(Game game) {
        Singleton = this;
        _imGuiRenderer = new(game);
        _imGuiRenderer.RebuildFontAtlas();
        Viewport = game.GraphicsDevice.Viewport;

        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        WindowViewport = game.GraphicsDevice.Viewport;
        if (game is BaseGame baseGame) {
            baseGame.OnWindowResize += BaseGame_OnWindowResize;
        }

#if DEBUG
        RegisterEditor("ui", typeof(UIEditor));
        RegisterEditor("serialization", typeof(SerializationEditor));
#endif
    }

    private void BaseGame_OnWindowResize(object sender, Events.WindowResizeEventArgs e) {
        WindowViewport = e.Viewport;
    }

    public static void RegisterMenu(string name, Type windowType) {
        if (!windowType.IsAssignableTo(typeof(DebugMenu))) {
            Logger.Error($"{windowType.Name} is not assignable to {nameof(DebugMenu)}");
            return;
        }
        MenuLookup[name] = windowType;
    }

    public static DebugMenu CreateMenu(string name) {
        if (!MenuLookup.TryGetValue(name, out Type windowType)) {
            return null;
        }

        DebugMenu menu = (DebugMenu)Activator.CreateInstance(windowType);
        menu.Show(Viewport);
        EnabledMenus.Add(menu);
        return menu;
    }

    public static void DisableMenu(DebugMenu menu) {
        EnabledMenus.Remove(menu);
    }

    public static void DisableAllWithType(string name) {
        EnabledMenus.RemoveAll(m => m.Title == name);
    }

    public static void DisableAll() {
        EnabledMenus.Clear();
    }

    public static DebugMenu GetOpenMenuByName(string name) {
        return EnabledMenus.Find(m => m.Title == name);
    }

    public static void OnResize(Viewport viewport) {
        Viewport = viewport;
        foreach (DebugMenu menu in EnabledMenus) {
            menu.Resize(viewport);
        }
    }

#if DEBUG
    public static void RegisterEditor(string name, Type type) {
        if (!type.IsAssignableTo(typeof(EditorScene))) {
            Logger.Error($"{type.Name} is not assignable to {nameof(EditorScene)}");
            return;
        }

        EditorLookup[name] = type;
    }

    public static void SetEditorScene(string name) {
        ActiveEditorScene?.Unload();
        ActiveEditorScene = null;

        if (string.IsNullOrEmpty(name)) return;

        if (!EditorLookup.TryGetValue(name, out Type type)) {
            Logger.Error($"Editor scene not found: {name}");
            return;
        }

        try {

            EditorScene scene = (EditorScene)Activator.CreateInstance(type);
            scene.Load();
            ActiveEditorScene = scene;

            BaseGame.Instance.SceneManager.SetScene(ActiveEditorScene);

        } catch (Exception e) {
            Logger.Exception(e);
        }
    }

    public static string[] ListEditorScenes() {
        return [.. EditorLookup.Keys];
    }

#endif

    public void Draw(GameTime gameTime) {
        if (!EnableWindowDrawing) return;

        _imGuiRenderer.BeforeLayout(gameTime);

#if DEBUG
        ActiveEditorScene?.Draw();
#endif

        foreach (DebugMenu menu in EnabledMenus)
            menu.Draw();

        _imGuiRenderer.AfterLayout();
    }

}

