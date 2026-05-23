using Engine.Debugging;
using Engine.Editors.Windows;
using Engine.Serialization;
using Engine.Sprites;
using Engine.UI;
using Engine.Utility;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Editors;

#if DEBUG

internal class UIEditor : EditorScene {

    private readonly UIManager manager;

    private bool isChildSelectionOpen;
    private AbstractUIComponent childSelectionTarget;
    private AbstractUIComponent selectedComponent;

    public Spritesheet UI_TEXTURE { get; private set; }

    private FilePickerWindow FilePickerWindow;
    private ConfirmationWindow ConfirmationWindow;

    private readonly Dictionary<string, Func<UIEditor, AbstractUIComponent>> UI_REGISTRY = new() {
        { "Simple", (scene) => {
            // TODO: Fix Sprite loading
            return new SimpleUIComponent(scene.UI_TEXTURE.CreateSprite(0, 0, 64, 64, Utility.Identifier.EMPTY), 0, 0, 64, 64);
        } }
    };

    private readonly string[] UI_TYPE_KEYS;

    public UIEditor() : base("ui-editor") {
        BaseGame.Instance.SetResizable();
        manager = BaseGame.Instance.UIManager;
        UI_TEXTURE = new Spritesheet("UI/panel");
        UI_TYPE_KEYS = [.. UI_REGISTRY.Keys];
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha) {

    }

    public override void DrawScene() {
        ImGui.DockSpaceOverViewport(0, ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode);

        DrawComponentsWindow();
        
        if (isChildSelectionOpen) {
            DrawChildSelectionWindow();
        }

        DrawInspectorWindow();

        if (FilePickerWindow != null && FilePickerWindow.Draw()) {
            HandleFilePicker();
            FilePickerWindow = null;
        }

        ConfirmationWindow?.Draw();

    }

    private void DrawComponentsWindow() {
        ImGui.SetNextWindowPos(new(0, 0), ImGuiCond.Once);
        ImGui.SetNextWindowSize(new(150, DebugMenuManager.WindowViewport.Height), ImGuiCond.Once);
        ImGui.Begin("Components");

        if (ImGui.TreeNodeEx("Root", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick)) {
            if (ImGui.IsItemClicked()) {
                selectedComponent = null;
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
        if (ImGui.TreeNodeEx($"{name}##{component.UID}", ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick)) {
            if (ImGui.IsItemClicked()) {
                selectedComponent = component;
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

    private void DrawInspectorWindow() {
        ImGui.SetNextWindowPos(new(DebugMenuManager.WindowViewport.Width - 250, 0), ImGuiCond.Once);
        ImGui.SetNextWindowSize(new(250, DebugMenuManager.WindowViewport.Height), ImGuiCond.Once);
        ImGui.Begin("Inspector");

        if (selectedComponent != null) {
            ImGui.Text(selectedComponent.GetType().Name);
            ImGui.Text(selectedComponent.UID);
            selectedComponent.DrawDebugMenu();
            ImGui.Separator();
            if (ImGui.Button("Add Child")) {
                childSelectionTarget = selectedComponent;
                isChildSelectionOpen = true;
            }
            if (ImGui.Button("Delete Component")) {
                manager.RemoveComponent(selectedComponent);
                selectedComponent = null;
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

    private void AddChild(string key) {
        if (!UI_REGISTRY.TryGetValue(key, out Func<UIEditor, AbstractUIComponent> value)) {
            Logger.Error($"Could not create UI component of type {key}. Key not found");
            return;
        }

        try {
            AbstractUIComponent component = value.Invoke(this);
            childSelectionTarget?.AddChild(component);
            manager.AddComponent(component);
        } catch (Exception e) { Logger.Exception(e); }
    }

    private void HandleFilePicker() {
        if (FilePickerWindow.SelectMode == FilePickerWindow.SelectionMode.EXPORT) {
            string filePath = FilePickerWindow.ResultPath;
            string ext = Path.GetExtension(filePath);

            if (ext != PathHelper.COMPRESSED_DATA_FILE_EXTENSION && ext != PathHelper.DATA_FILE_EXTENSION) {
                if (File.Exists(filePath)) {
                    ConfirmationWindow = new((_) => { ConfirmationWindow = null; }, "Cannot override this file as it is not in the correct format", "Error", ConfirmationWindow.Buttons.OK);
                    return;
                }

                filePath = Path.ChangeExtension(filePath, PathHelper.COMPRESSED_DATA_FILE_EXTENSION);
            }
            SerializableDictionary dict = new();
            BaseGame.Instance.UIManager.WriteData(dict);
            SerializableDictionary.WriteToFile(filePath, dict);

            ConfirmationWindow = new((_) => { ConfirmationWindow = null; }, $"Successfully saved to file:\n{filePath}", "Success");
        }
    }

    protected override void DrawMenu() {
        if (ImGui.BeginMenu("File")) {
            if (ImGui.MenuItem("Import", "Ctrl + I")) {
                FilePickerWindow = new(FilePickerWindow.TargetType.FILE, FilePickerWindow.SelectionMode.IMPORT);
            }
            if (ImGui.MenuItem("Export", "Ctrl + E")) {
                FilePickerWindow = new(FilePickerWindow.TargetType.FILE, FilePickerWindow.SelectionMode.EXPORT);
            }

            ImGui.EndMenu();
        }

        if (ImGui.BeginMenu("Resources")) {
            if (ImGui.MenuItem("Load Internal", "Ctrl + L")) {

            }
            if (ImGui.MenuItem("Load External", "Ctrl + O")) {

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

            if (ImGui.MenuItem("Delete all")) {
                ConfirmationWindow = new((action) => {
                    if (action == ConfirmationWindow.Buttons.OK) {
                        BaseGame.Instance.UIManager.ClearAll();
                    }
                }, "This will delete all existing components and cannot be undone!", "Are you sure?", ConfirmationWindow.Buttons.OK | ConfirmationWindow.Buttons.CANCEL);
            }

            ImGui.EndMenu();
        }
    }

    public override void FixedUpdate() {

    }

    public override void LoadContent(ContentManager contentManager) {

    }

    public override void Update(GameTime gameTime, float delta) {

    }

    protected override void OnBeginLoad() {
        
    }

}

#endif
