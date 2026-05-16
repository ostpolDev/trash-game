using ImGuiNET;
using System.Numerics;

namespace Engine.UI.Debugging;

public class SimpleRenderer(SimpleUIComponent component) : BaseRenderer(component) {

    private readonly int[] UVPos = [component.Sprite.SourceRectangle.X, component.Sprite.SourceRectangle.Y];
    private readonly int[] UVSize = [component.Sprite.SourceRectangle.Width, component.Sprite.SourceRectangle.Height];

    private Vector4 Color = component.Color.ToVector4().ToNumerics();

    public override void Render() {
        base.Render();
        SimpleUIComponent component = (SimpleUIComponent)Component;

        if (ImGui.CollapsingHeader("Sprite", ImGuiTreeNodeFlags.DefaultOpen)) {

            ImGui.Spacing();
            if (ImGui.ColorPicker4("Color", ref Color)) {
                component.Color = new Microsoft.Xna.Framework.Color(new Microsoft.Xna.Framework.Vector4(Color.X, Color.Y, Color.Z, Color.W));
            }
            ImGui.Spacing();

            if (ImGui.InputInt2("Position##uv", ref UVPos[0])) {
                component.Sprite.MoveUV(UVPos[0], UVPos[1]);
            }

            if (ImGui.InputInt2("Scale##uv", ref UVSize[0])) {
                component.Sprite.MoveUV(UVPos[0], UVPos[1], UVSize[0], UVSize[1]);
            }
        }
        ImGui.Spacing();
    }

}
