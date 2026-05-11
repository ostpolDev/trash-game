using Engine.Debugging;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Editors;

public class UIEditor : EditorScene {

    public UIEditor() : base("ui-editor") {
        BaseGame.Instance?.SetResizable();
    }


    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha) {

    }

    public override void DrawScene() {
        ImGui.SetNextWindowPos(new(0, 0));
        ImGui.SetNextWindowSize(new(150, DebugMenuManager.WindowViewport.Height));
        ImGui.Begin("Components", ImGuiWindowFlags.NoMove);
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
