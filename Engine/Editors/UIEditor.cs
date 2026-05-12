using Engine.Debugging;
using Engine.Sprites;
using Engine.UI;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Engine.Editors;

public class UIEditor : EditorScene {

    private readonly UIManager manager;

    private bool isChildSelectionOpen;
    private AbstractUIComponent childSelectionTarget;

    public Spritesheet UI_TEXTURE { get; private set; }

    private readonly Dictionary<string, Func<UIEditor, AbstractUIComponent>> UI_REGISTRY = new() {
        { "Simple", (scene) => {
            return new SimpleUIComponent(scene.UI_TEXTURE, new Rectangle(0, 0, 64, 64), 0, 0, 64, 64);
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
        ImGui.SetNextWindowPos(new(0, 0));
        ImGui.SetNextWindowSize(new(150, DebugMenuManager.WindowViewport.Height), ImGuiCond.Appearing);
        ImGui.Begin("Components", ImGuiWindowFlags.NoMove);

        if (ImGui.TreeNodeEx("Root", ImGuiTreeNodeFlags.DefaultOpen)) {
            foreach (AbstractUIComponent component in manager.Components) {
                if (ImGui.TreeNodeEx(component.GetType().Name)) {
                    ImGui.TreePop();
                }
            }

            if (ImGui.BeginPopupContextItem()) {
                ImGui.Text("Root");
                if (ImGui.Button("Add Child")) {
                    childSelectionTarget = null;
                    isChildSelectionOpen = true;
                }
                ImGui.EndPopup();
            }

            ImGui.TreePop();
        }

        ImGui.End();

        if (isChildSelectionOpen) {
            ImGui.SetNextWindowSize(new(200, 100), ImGuiCond.Appearing);
            ImGui.Begin("Child Selection");
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

    public override void FixedUpdate() {

    }

    public override void LoadContent(ContentManager contentManager) {

    }

    public override void Update(GameTime gameTime, float delta) {

    }

    protected override void OnBeginLoad() {
        
    }

}
