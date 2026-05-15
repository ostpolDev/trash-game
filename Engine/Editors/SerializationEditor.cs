using Engine.Interaction;
using Engine.Serialization;
using Engine.Serialization.Entries;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace Engine.Editors;

internal class SerializationEditor : EditorScene {

    public SerializableDictionary Data { get; private set; }

    private readonly InputManager inputManager;

    public SerializationEditor() : base("serialization") {
        BaseGame.Instance.SetResizable(true);
        inputManager = BaseGame.Instance.InputManager;
    }

    private SerializableDictionary AddTarget;
    private string[] POSSIBLE_ITEMS = [.. Enum.GetValues<DictionaryEntryType>().Select(m => m.ToString())];
    private int ToAddType = (int)DictionaryEntryType.INVALID;
    private int ToChangeType = (int)DictionaryEntryType.INVALID;
    private string ToAddKey = "";
    private string ToEditKey = "";

    private bool popupOpen = false;

    private AbstractEntry ToEditTarget;
    private AbstractEntry ToEditParent;

    public override void DrawScene() {

        ImGuiViewportPtr viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(viewport.WorkPos);
        ImGui.SetNextWindowSize(viewport.WorkSize);
        ImGui.SetNextWindowViewport(viewport.ID);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(5, 5));

        ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoTitleBar
                                     | ImGuiWindowFlags.NoCollapse
                                     | ImGuiWindowFlags.NoResize
                                     | ImGuiWindowFlags.NoMove
                                     | ImGuiWindowFlags.NoBringToFrontOnFocus
                                     | ImGuiWindowFlags.NoNavFocus;

        DrawTree(windowFlags);

        if (AddTarget != null)
            AddItemWindow();

        if (ToEditTarget != null)
            EditWindow();

        if (popupOpen) {
            ImGui.Begin("Confirm New", ImGuiWindowFlags.Modal | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);
                ImGui.Text("Are you sure? This will replace the existing Data.");
                if (ImGui.Button("Confirm")) {
                    Data = null;
                    popupOpen = false;
                    CreateNew();
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancel")) {
                    popupOpen = false;
                }
            ImGui.End();
        }
    }

    private void DrawTree(ImGuiWindowFlags flags) {
        ImGui.Begin("Tree View", flags);
        ImGui.PopStyleVar(3);

        if (Data == null) {
            if (ImGui.Button("New Tree")) {
                CreateNew();
            }
            ImGui.End();
            return;
        }

        TreeNode(Data, null);

        ImGui.End();
    }

    private void TreeNode(AbstractEntry entry, AbstractEntry parent, int i = 0) {
        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow;
        if (entry is not SerializableDictionary) {
            flags |= ImGuiTreeNodeFlags.Leaf;
        }

        if (ImGui.TreeNodeEx($"{entry.Key ?? "Root"}: {entry.GetEntryType()} - {entry}##{i}", flags)) {
            if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && ImGui.IsItemClicked(ImGuiMouseButton.Left)) {
                // EDIT
                ToEditTarget = entry;
                ToEditParent = parent;
                ToChangeType = (int)entry.GetEntryType();
                ToEditKey = entry.Key;
            }

            if (entry is SerializableDictionary serializableDictionary) {
                if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) {
                    // Right Click
                    AddTarget = serializableDictionary;
                }

                foreach (var item in serializableDictionary.GetValues()) {
                    TreeNode(item, entry, i + 1);
                }
            }

            ImGui.TreePop();
        }
    }

    private void AddItemWindow() {
        ImGui.Begin("Add Item", ImGuiWindowFlags.NoCollapse);

        ImGui.Combo("Item Type", ref ToAddType, POSSIBLE_ITEMS, POSSIBLE_ITEMS.Length);
        ImGui.InputText("Key", ref ToAddKey, 64);

        if (ImGui.Button("Add")) {
            DictionaryEntryType type = (DictionaryEntryType)ToAddType;
            if (type != DictionaryEntryType.END && type != DictionaryEntryType.INVALID && !string.IsNullOrEmpty(ToAddKey)) {
                AddTarget.Put(ToAddKey, AbstractEntry.GetEntryFromType(type));
                AddTarget = null;
                ToAddKey = "";
                ToAddType = (int)DictionaryEntryType.INVALID;
            }
        }
        ImGui.SameLine();
        if (ImGui.Button("Cancel")) {
            AddTarget = null;
        }

        ImGui.End();
    }

    private void EditWindow() {
        ImGui.Begin("Edit", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking);

        ImGui.SeparatorText($"{ToEditTarget.Key ?? "Root"} ({ToEditTarget.GetEntryType()})");
        ImGui.Spacing();

        ToEditTarget.RenderDebugEditor();

        ImGui.Spacing();

        if (ToEditParent is SerializableDictionary dict) {
            ImGui.SeparatorText("Other");
            ImGui.Spacing();

            if (ImGui.Button("Delete")) {
                dict.Delete(ToEditTarget.Key);
                ToEditTarget = null;
            }

            ImGui.Spacing();
            ImGui.SeparatorText("Type");

            ImGui.Combo("New Type", ref ToChangeType, POSSIBLE_ITEMS, POSSIBLE_ITEMS.Length);
            if (ImGui.Button("Change Type")) {
                string key = ToEditTarget.Key;
                ToEditTarget = AbstractEntry.GetEntryFromType((DictionaryEntryType)ToChangeType);
                dict.Put(key, ToEditTarget);
            }
        }

        if (ToEditKey != null) {
            ImGui.Spacing();
            ImGui.SeparatorText("Key");

            ImGui.InputText("New Key", ref ToEditKey, 64);
            if (ImGui.Button("Rename")) {
                ToEditTarget.SetKey(ToEditKey);
            }

            ImGui.Spacing();
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        if (ImGui.Button("Close")) {
            ToEditTarget = null;
        }

        ImGui.End();
    }


    protected override void DrawMenu() {
        if (ImGui.BeginMenu("File")) {
            if (ImGui.MenuItem("New", "Ctrl + N")) {
                CreateNew();
            }
            if (ImGui.MenuItem("Import", "Ctrl + I")) {

            }
            if (ImGui.MenuItem("Export", "Ctrl + E")) {

            }
            ImGui.EndMenu();
        }
    }

    private void CreateNew() {
        if (Data == null) {
            Data = new();
            return;
        }
        popupOpen = true;
    }


    public override void FixedUpdate() {
        
    }

    public override void LoadContent(ContentManager contentManager) {
        
    }

    public override void Update(GameTime gameTime, float delta) {
        if (inputManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.N) && inputManager.IsCtrlDown) {
            CreateNew();
        }
    }

    protected override void OnBeginLoad() {
        
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha) {

    }

}
