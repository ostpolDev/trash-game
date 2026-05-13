using ImGuiNET;

namespace Engine.UI.Debugging;

public class SimpleRenderer(SimpleUIComponent component) : BaseRenderer(component) {

    private readonly int[] UVPos = [component.SourceRectangle.X, component.SourceRectangle.Y];
    private readonly int[] UVSize = [component.SourceRectangle.Width, component.SourceRectangle.Height];

    public override void Render() {
        base.Render();
        SimpleUIComponent component = (SimpleUIComponent)Component;

        if (ImGui.CollapsingHeader("UV", ImGuiTreeNodeFlags.DefaultOpen)) {
            if (ImGui.InputInt2("Position##uv", ref UVPos[0])) {
                component.SourceRectangle.X = UVPos[0];
                component.SourceRectangle.Y = UVPos[1];
            }

            if (ImGui.InputInt2("Scale##uv", ref UVSize[0])) {
                component.SourceRectangle.Width = UVSize[0];
                component.SourceRectangle.Height = UVSize[1];
            }
        }
        ImGui.Spacing();
    }

}
