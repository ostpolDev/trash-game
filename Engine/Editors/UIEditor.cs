using Engine.Debugging;
using Engine.UI;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.Editors;

public class UIEditor : EditorScene {

    private readonly UIManager manager;

    private bool isChildSelectionOpen;
    private AbstractUIComponent childSelectionTarget;

    private readonly Type[] UI_TYPES = [];

    public UIEditor() : base("ui-editor") {
        BaseGame.Instance?.SetResizable();
        manager = BaseGame.Instance.UIManager;
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

            foreach (Type type in UI_TYPES) {
                if (ImGui.Button(type.Name)) {
                    AddChild(type);
                    isChildSelectionOpen = false;
                }
            }


            ImGui.End();
        }
    }

    private void AddChild(Type type) {
        if (type == null || !type.IsAssignableTo(typeof(AbstractUIComponent))) {
            Logger.Error($"Child type \"{type.Name}\" is not assignable to {nameof(AbstractUIComponent)}");
            return;
        }
        try {
            AbstractUIComponent component = (AbstractUIComponent)Activator.CreateInstance(type);
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
