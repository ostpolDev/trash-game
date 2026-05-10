using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Editors;

public class UIEditor : EditorScene {

    public UIEditor() : base("ui-editor") { }


    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha) {
    }

    public override void DrawScene() {
        ImGui.Begin("Test");
        ImGui.Text("HELLO WORLD");
        ImGui.End();
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
