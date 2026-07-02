using Engine.UI.Components;
using ImGuiNET;

namespace Engine.UI.Debugging;

#if DEBUG

public class NineSliceRenderer(NineSliceComponent component) : SimpleRenderer(component) {

    private readonly int[] SliceUVPos = [component.SliceRectangle.X, component.SliceRectangle.Y];
    private readonly int[] SliceUVSize = [component.SliceRectangle.Width, component.SliceRectangle.Height];

    public override void Render() {
        base.Render();

        NineSliceComponent component = (NineSliceComponent)Component;
        if (ImGui.CollapsingHeader("Nine Slice")) {
            if (ImGui.InputInt2("Position##slice", ref SliceUVPos[0])) {
                component.SetSliceArea(new Microsoft.Xna.Framework.Rectangle(SliceUVPos[0], SliceUVPos[1], SliceUVSize[0], SliceUVSize[1]));
            }
            if (ImGui.InputInt2("Size##slice", ref SliceUVSize[0])) {
                component.SetSliceArea(new Microsoft.Xna.Framework.Rectangle(SliceUVPos[0], SliceUVPos[1], SliceUVSize[0], SliceUVSize[1]));
            }
        }
        ImGui.Spacing();
    }

}

#endif
