using Engine.Debugging;
using Engine.Editors.Windows;
using Engine.Serialization;
using Engine.Sprites;
using Engine.UI;
using Engine.UI.Components;
using Engine.Utility;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Editors;

#if DEBUG

internal class UIEditor : EditorScene {

    private readonly UIManager manager;

    private bool isChildSelectionOpen;
    private AbstractUIComponent childSelectionTarget;

    public Spritesheet UI_TEXTURE { get; private set; }

    private FilePickerWindow FilePickerWindow;

    private bool isFontStatsOpen = false;
    private bool isUIInfoOpen = false;
    private bool allowClickInteraction = true;

    private float uiScale = 0f;

    private readonly Dictionary<string, Func<UIEditor, AbstractUIComponent>> UI_REGISTRY = new() {
        { "Simple", (scene) => {
            return new SimpleUIComponent(scene.UI_TEXTURE.Get(Identifier.EMPTY), 0, 0, 64, 64);
        } },
        { "Text", (_) => {
            return new TextUIComponent(0, 0, "Text");
        } },
        { "Empty", (_) => {
            return new EmptyUIComponent(0, 0);
        } },
        { "NineSlice", (scene) => {
            return new NineSliceComponent(scene.UI_TEXTURE.Get(Identifier.EMPTY), 0, 0, 64, 64);
        } }
    };

    private readonly string[] UI_TYPE_KEYS;

    public UIEditor() : base("ui-editor") {
        BaseGame.Instance.SetResizable();
        manager = BaseGame.Instance.UIManager;
        UI_TEXTURE = new Spritesheet("UI/panel");
        UI_TYPE_KEYS = [.. UI_REGISTRY.Keys];
        uiScale = manager.UIScale;

#if DEBUG
        UI_TEXTURE.With(Identifier.EMPTY, 0, 0, 64, 64);

        BaseGame.Instance?.SpriteManager.RegisterSpritesheet(UI_TEXTURE);
#endif
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha) {

    }

    public override void DrawScene() {
        ImGui.DockSpaceOverViewport(0, ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode);

        DrawComponentsWindow();
        
        if (isChildSelectionOpen) {
            DrawChildSelectionWindow();
        }

        if (isFontStatsOpen) {
            DrawFontStats();
        }

        if (isUIInfoOpen) {
            DrawUIInfo();
        }

        DrawInspectorWindow();

        if (FilePickerWindow != null && FilePickerWindow.Draw()) {
            if (!FilePickerWindow.WasCancelled)
                HandleFilePicker();
            FilePickerWindow = null;
        }

    }

    private void DrawComponentsWindow() {
        ImGui.SetNextWindowPos(new(0, 0), ImGuiCond.Once);
        ImGui.SetNextWindowSize(new(150, DebugMenuManager.WindowViewport.Height), ImGuiCond.Once);
        ImGui.Begin("Components");

        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.DefaultOpen;
        if (manager.ChildCount == 0)
            flags |= ImGuiTreeNodeFlags.Leaf;

        if (ImGui.TreeNodeEx("Root", flags)) {
            if (ImGui.IsItemClicked()) {
                manager.SelectedObject = null;
            }

            for (int i = 0; i < manager.ChildCount; i++) {
                AbstractUIComponent component = manager.Components[i];
                if (component.Parent != null) continue;

                DrawComponentTree(component, i);
            }

            ImGui.TreePop();
        }

        ImGui.End();
    }

    private void DrawComponentTree(AbstractUIComponent component, int i) {
        string name = string.IsNullOrEmpty(component.ReferenceID) ? component.GetType().Name : component.ReferenceID;

        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow;
        if (component.ChildCount == 0)
            flags |= ImGuiTreeNodeFlags.Leaf;

        if (ImGui.TreeNodeEx($"{name}##{component.UID}", flags)) {
            if (ImGui.IsItemClicked()) {
                manager.SelectedObject = component;
            }

            for (int j = 0; j < component.ChildCount; j++) {
                DrawComponentTree(component.Children[j], j);
            }

            ImGui.TreePop();
        }
    }

    private void DrawChildSelectionWindow() {
        ImGui.SetNextWindowSize(new(200, 100), ImGuiCond.Appearing);
        ImGui.Begin("Child Selection", ImGuiWindowFlags.MenuBar);

        if (ImGui.Button("Cancel")) {
            isChildSelectionOpen = false;
            childSelectionTarget = null;
        }
        foreach (string key in UI_TYPE_KEYS) {
            if (ImGui.Button(key)) {
                AddChild(key);
                isChildSelectionOpen = false;
            }
        }


        ImGui.End();
    }

    private void DrawFontStats() {
        ImGui.Begin("Font System Stats", ImGuiWindowFlags.NoDocking);
        ImGui.Text($"Initialized: {FontManager.IsInitialized}");
        if (!FontManager.IsInitialized) {
            ImGui.End();
            return;
        }

        ImGui.Text($"Font Atlases: {FontManager.FontSystem.Atlases.Count}");
        ImGui.Text($"Current Atlas: {FontManager.FontSystem.CurrentAtlas?.Texture.Name ?? "--"}");

        ImGui.Text($"Texture: {FontManager.FontSystem.TextureWidth} x {FontManager.FontSystem.TextureHeight}");
        ImGui.Text($"Kernel: {FontManager.FontSystem.KernelWidth} x {FontManager.FontSystem.KernelHeight}");

        ImGui.Text($"Use text shaping: {FontManager.FontSystem.UseTextShaping}");

        ImGui.Spacing();
        if (ImGui.Button("Close##font")) {
            isFontStatsOpen = false;
        }

        ImGui.End();
    }

    private void DrawUIInfo() {
        ImGui.Begin("UI Information", ImGuiWindowFlags.NoDocking);
        ImGui.Text($"Elements: {manager.ChildCount}");
        ImGui.Text($"Scale: {manager.UIScale} ({manager.UIScaleFactor})");
        ImGui.Text($"Matrix: {manager.UIScaleMatrix}");
        ImGui.Spacing();
        ImGui.SeparatorText("Settings");
        ImGui.Spacing();

        if (ImGui.SliderFloat("UI Scale", ref uiScale, 0.01f, 10f)) {
            manager.SetUIScale(uiScale);
        }
        if (ImGui.InputFloat("##uiScale", ref uiScale)) {
            manager.SetUIScale(uiScale);
        }

        if (ImGui.Button("Recalculate Matrix")) {
            manager.UpdateScaleMatrix();
        }

        if (ImGui.Button("Close##uisettings")) {
            isUIInfoOpen = false;
        }

        ImGui.End();
    }

    private void DrawInspectorWindow() {
        ImGui.SetNextWindowPos(new(DebugMenuManager.WindowViewport.Width - 250, 0), ImGuiCond.Once);
        ImGui.SetNextWindowSize(new(250, DebugMenuManager.WindowViewport.Height), ImGuiCond.Once);
        ImGui.Begin("Inspector");

        if (manager.SelectedObject != null) {
            ImGui.Text(manager.SelectedObject.GetType().Name);
            ImGui.Text(manager.SelectedObject.UID);
            manager.SelectedObject.DrawDebugMenu();
            ImGui.Separator();
            if (ImGui.Button("Add Child")) {
                childSelectionTarget = manager.SelectedObject;
                isChildSelectionOpen = true;
            }
            if (ImGui.Button("Delete Component")) {
                manager.RemoveComponent(manager.SelectedObject);
                manager.SelectedObject = null;
            }
        } else {
            ImGui.Text("Root Node");
            if (ImGui.Button("Add Child")) {
                childSelectionTarget = null;
                isChildSelectionOpen = true;
            }
        }

        ImGui.End();
    }

    private AbstractUIComponent AddChild(string key) {
        if (!UI_REGISTRY.TryGetValue(key, out Func<UIEditor, AbstractUIComponent> value)) {
            Logger.Error($"Could not create UI component of type {key}. Key not found");
            return null;
        }

        try {
            AbstractUIComponent component = value.Invoke(this);
            childSelectionTarget?.AddChild(component);
            manager.AddComponent(component);
            return component;
        } catch (Exception e) { Logger.Exception(e); return null; }
    }

    private void HandleFilePicker() {
        if (FilePickerWindow.ctx == "font") {
            HandleFontFilePicker();
            return;
        }

        if (FilePickerWindow.ctx == "ui") {
            HandleUIFilePicker();
            return;
        }
        
    }

    private void HandleUIFilePicker() {
        if (FilePickerWindow.SelectMode == FilePickerWindow.SelectionMode.EXPORT) {
            string filePath = FilePickerWindow.ResultPath;
            string ext = Path.GetExtension(filePath);

            if (ext != PathHelper.COMPRESSED_DATA_FILE_EXTENSION && ext != PathHelper.DATA_FILE_EXTENSION) {
                if (File.Exists(filePath)) {
                    ShowConfirmationWindow("Cannot override this file as it is not in the correct format", "Error", ConfirmationWindow.Buttons.OK);
                    return;
                }

                filePath = PathHelper.EnsureValidSaveFileExtension(filePath);
            }
            SerializableDictionary dict = new();
            BaseGame.Instance.UIManager.WriteData(dict);
            SerializableDictionary.WriteToFile(filePath, dict);

            ShowConfirmationWindow($"Successfully saved to file:\n{filePath}", "Success");
        } else if (FilePickerWindow.SelectMode == FilePickerWindow.SelectionMode.IMPORT) {
            string filePath = FilePickerWindow.ResultPath;
            string ext = Path.GetExtension(filePath);

            if (ext != PathHelper.COMPRESSED_DATA_FILE_EXTENSION && ext != PathHelper.DATA_FILE_EXTENSION) {
                ShowConfirmationWindow($"File extension is invalid:\n{filePath}", "Error");
                return;
            }

            SerializableDictionary dict = SerializableDictionary.ReadFromFile(filePath);
            BaseGame.Instance.UIManager.ReadData(dict);
        }
    }

    private void HandleFontFilePicker() {
        if (FilePickerWindow.SelectMode == FilePickerWindow.SelectionMode.IMPORT) {
            string filePath = FilePickerWindow.ResultPath;
            if (!File.Exists(filePath)) {
                ShowConfirmationWindow($"File not found:\n{filePath}", "Error");
                return;
            }

            try {
                FontManager.RegisterFont(filePath);
                ShowConfirmationWindow($"Successfully loaded font: {Path.GetFileName(filePath)}", "Success");
            } catch (Exception e) {
                Logger.Exception(e);
                ShowConfirmationWindow($"Failed to load font: {e.Message ?? "Unknown"}\n{e.StackTrace ?? "--"}", "Error");
            }
        }
    }

    protected override void DrawMenu() {
        if (ImGui.BeginMenu("File")) {
            if (ImGui.MenuItem("Import", "Ctrl + I")) {
                FilePickerWindow = new(FilePickerWindow.TargetType.FILE, FilePickerWindow.SelectionMode.IMPORT) { 
                    ctx = "ui"
                };
            }
            if (ImGui.MenuItem("Export", "Ctrl + E")) {
                FilePickerWindow = new(FilePickerWindow.TargetType.FILE, FilePickerWindow.SelectionMode.EXPORT) {
                    ctx = "ui"
                };
            }

            ImGui.EndMenu();
        }

        if (ImGui.BeginMenu("Resources")) {
            if (ImGui.MenuItem("Load Internal", "Ctrl + L")) {

            }
            if (ImGui.MenuItem("Load External", "Ctrl + O")) {

            }
            if (ImGui.BeginMenu("Font Manager")) {
                if (ImGui.MenuItem("Load Font")) {
                    FilePickerWindow = new(FilePickerWindow.TargetType.FILE, FilePickerWindow.SelectionMode.IMPORT) {
                        ctx = "font"
                    };
                }
                if (ImGui.MenuItem("Show Stats")) {
                    isFontStatsOpen = !isFontStatsOpen;
                }
                ImGui.EndMenu();
            }
            ImGui.Separator();
            if (ImGui.MenuItem("Pack resources", "Ctrl + P")) {

            }

            ImGui.EndMenu();
        }

        if (ImGui.BeginMenu("Edit")) {
            if (ImGui.MenuItem("Find", "Ctrl + F")) {

            }

            ImGui.Separator();

            if (ImGui.BeginMenu("UI Manager")) {
                if (ImGui.MenuItem("Recalculate Depth")) {
                    manager.RecalculateAllDepth();
                }
                if (ImGui.MenuItem("Sort components")) {
                    manager.SortComponentZ();
                }
                if (ImGui.MenuItem("Settings")) {
                    isUIInfoOpen = !isUIInfoOpen;
                }
                ImGui.Separator();
                ImGui.Checkbox("Draw Scissor Mask", ref manager.Debug_DrawScissorTest);
                ImGui.Spacing();
                ImGui.Checkbox("Draw UI Bounds", ref manager.Debug_DrawBounds);
                ImGui.Spacing();
                ImGui.Checkbox("Draw Local UI Bounds", ref manager.Debug_DrawLocalBounds);
                ImGui.Spacing();
                ImGui.Checkbox("Disable Mouse Events", ref manager.Debug_DisableMouseEventListeners);
                ImGui.Spacing();
                ImGui.Checkbox("Draw Gizmos", ref manager.Debug_DrawGizmos);
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Generate")) {
                if (ImGui.MenuItem("Create 3x3")) {
                    for (int i = 0; i < 9; i++) {
                        AbstractUIComponent component = AddChild("Simple");
                        component.SetAnchorPosition((UIAnchorPosition)i);
                        component.SetReferenceID($"{component.AnchorPosition} - Simple");
                    }
                }
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Interaction")) {
                ImGui.Checkbox("Allow clicking elements", ref allowClickInteraction);
                ImGui.Spacing();
                ImGui.EndMenu();
            }

            ImGui.Separator();

            if (ImGui.MenuItem("Delete all")) {
                ShowConfirmationWindow((action) => {
                    if (action == ConfirmationWindow.Buttons.OK) {
                        BaseGame.Instance.UIManager.ClearAll();
                    }
                }, "This will delete all existing components and cannot be undone!", "Are you sure?", ConfirmationWindow.Buttons.OK | ConfirmationWindow.Buttons.CANCEL);
            }

            ImGui.EndMenu();
        }
    
        if (ImGui.BeginMenu("View")) {
            ImGui.Checkbox("Highlight selected", ref manager.Debug_DrawSelection);

            ImGui.EndMenu();
        }
    }

    public override void FixedUpdate() {

    }

    public override void LoadContent(ContentManager contentManager) {

    }

    public override void Update(GameTime gameTime, float delta) {
        if (allowClickInteraction && !ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow) && BaseGame.Instance.InputManager.IsLeftMouseDown()) {
            MouseState state = Mouse.GetState();
            manager.SelectedObject = manager.GetComponentAtPosition(state.X, state.Y);
        }
    }

    protected override void OnBeginLoad() {
        
    }

}

#endif
