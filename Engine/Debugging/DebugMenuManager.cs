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

    private readonly ImGuiRenderer _imGuiRenderer;

    public static bool IsShowingWindows { get { return EnabledMenus.Count > 0; } }
    public static int ShownWindowCount { get { return EnabledMenus.Count; } }

    public DebugMenuManager(Game game) {
        Singleton = this;
        _imGuiRenderer = new(game);
        _imGuiRenderer.RebuildFontAtlas();
        Viewport = game.GraphicsDevice.Viewport;
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

    public static void OnResize(Viewport viewport) {
        Viewport = viewport;
        foreach (DebugMenu menu in EnabledMenus) {
            menu.Resize(viewport);
        }
    }

    public void Draw(GameTime gameTime) {
        _imGuiRenderer.BeforeLayout(gameTime);

        foreach (DebugMenu menu in EnabledMenus)
            menu.Draw();

        _imGuiRenderer.AfterLayout();
    }

}

