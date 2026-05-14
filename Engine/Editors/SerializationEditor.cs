using Engine.Serialization;
using Engine.Serialization.Entries;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Editors;

internal class SerializationEditor : EditorScene {

    public SerializableDictionary Data { get; private set; }

    private uint dockspaceId;

    public SerializationEditor() : base("serialization") {
        BaseGame.Instance.SetResizable(true);
    }

    public override void DrawScene() {
        dockspaceId = ImGui.GetID("MyWindowDockSpace");
        //ImGui.DockSpaceOverViewport(0, ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode);

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
    }

    private void DrawTree(ImGuiWindowFlags flags) {
        ImGui.Begin("Tree View", flags);
        ImGui.PopStyleVar(3);

        //dockspaceId = ImGui.GetID("MyDockingSpace");
        //ImGui.DockSpace(dockspaceId, new System.Numerics.Vector2(0, 0), ImGuiDockNodeFlags.None);

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
        if (ImGui.TreeNodeEx($"{entry.Key ?? "Root"}: {entry.GetEntryType()} - {entry}##{i}", ImGuiTreeNodeFlags.OpenOnArrow)) {
            if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && ImGui.IsItemClicked(ImGuiMouseButton.Left)) {
                // EDIT
            }

            if (entry is SerializableDictionary serializableDictionary) {
                foreach (var item in serializableDictionary.GetValues()) {
                    TreeNode(item, i + 1);
                }
            }
            ImGui.TreePop();
        }
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
