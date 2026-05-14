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

    public SerializationEditor() : base("serialization") {
        BaseGame.Instance.SetResizable(true);
    }

    private SerializableDictionary AddTarget;
    private string[] POSSIBLE_ITEMS = [.. Enum.GetValues<DictionaryEntryType>().Select(m => m.ToString())];
    private int ToAddType = (int)DictionaryEntryType.INVALID;
    private string ToAddKey = "";

    private AbstractEntry ToEditTarget;

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
    }

    private void DrawTree(ImGuiWindowFlags flags) {
        ImGui.Begin("Tree View", flags);
        ImGui.PopStyleVar(3);

        if (Data == null) {
            if (ImGui.Button("New Tree")) {
                Data = new();
            }
            ImGui.End();
            return;
        }

        TreeNode(Data);

        ImGui.End();
    }

    private void TreeNode(AbstractEntry entry, int i = 0) {
        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow;
        if (entry is not SerializableDictionary) {
            flags |= ImGuiTreeNodeFlags.Leaf;
        }

        if (ImGui.TreeNodeEx($"{entry.Key ?? "Root"}: {entry.GetEntryType()} - {entry}##{i}", flags)) {
            if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && ImGui.IsItemClicked(ImGuiMouseButton.Left)) {
                // EDIT
                ToEditTarget = entry;
            }

            if (entry is SerializableDictionary serializableDictionary) {
                if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) {
                    // Right Click
                    AddTarget = serializableDictionary;
                }

                foreach (var item in serializableDictionary.GetValues()) {
                    TreeNode(item, i + 1);
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
        ImGui.Begin("Edit", ImGuiWindowFlags.NoCollapse);

        ImGui.SeparatorText(ToEditTarget.Key);
        ImGui.Spacing();

        ToEditTarget.RenderDebugEditor();

        ImGui.Spacing();
        ImGui.SeparatorText("Other");
        ImGui.Spacing();

        if (ImGui.Button("Delete")) {

        }

        ImGui.End();
    }


    protected override void DrawMenu() {
        if (ImGui.BeginMenu("File")) {
            if (ImGui.MenuItem("New")) {
                Data = new();
            }
            if (ImGui.MenuItem("Import")) {

            }
            if (ImGui.MenuItem("Export")) {

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

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha) {

    }

}
