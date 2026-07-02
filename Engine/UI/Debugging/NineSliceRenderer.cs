using Engine.UI.Components;
using ImGuiNET;

namespace Engine.UI.Debugging;

#if DEBUG

public class NineSliceRenderer(NineSliceComponent component) : SimpleRenderer(component) {

    private readonly int[] SliceUVPos = [component.SliceRectangle.X, component.SliceRectangle.Y];
    private readonly int[] SliceUVSize = [component.SliceRectangle.Width, component.SliceRectangle.Height];

    private bool showInfoWindow = false;

    public override void Render() {
        base.Render();

        NineSliceComponent component = (NineSliceComponent)Component;

        if (showInfoWindow) {
            ImGui.Begin("Slice Info", ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.AlwaysAutoResize);

            ImGui.Text($"Screen position: {component.ScreenArea}");
            ImGui.Text($"Sprite UV: {component.Sprite.SourceRectangle}");
            ImGui.Spacing();

            ImGui.SeparatorText("Slices");
            ImGui.Spacing();

            if (ImGui.BeginTable("##sliceinfotable", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY, new(0, 200))) {
                ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Value", ImGuiTableColumnFlags.WidthStretch, 0.8f);
                ImGui.TableHeadersRow();

                for (int i = 0; i < component.Slices.Length; i++) {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text($"{(i >= 9 ? "S " : "")}{(UIAnchorPosition)(i % 9)}");
                    ImGui.TableNextColumn();
                    ImGui.Text(component.Slices[i].ToString());
                }

                ImGui.EndTable();
            }

            ImGui.Separator();

            if (ImGui.Button("Recalculate Slice##inforecalc")) {
                component.RecalculateSlice();
            }

            ImGui.SameLine();

            if (ImGui.Button("Close##info")) {
                showInfoWindow = false;
            }

            ImGui.End();
        }

        if (ImGui.CollapsingHeader("Nine Slice")) {
            if (ImGui.InputInt2("Position##slice", ref SliceUVPos[0])) {
                component.SetSliceArea(new Microsoft.Xna.Framework.Rectangle(SliceUVPos[0], SliceUVPos[1], SliceUVSize[0], SliceUVSize[1]));
            }
            if (ImGui.InputInt2("Size##slice", ref SliceUVSize[0])) {
                component.SetSliceArea(new Microsoft.Xna.Framework.Rectangle(SliceUVPos[0], SliceUVPos[1], SliceUVSize[0], SliceUVSize[1]));
            }
            if (ImGui.Button("Recalculate Slice")) {
                component.RecalculateSlice();
            }
            ImGui.SameLine();
            if (ImGui.Button("Info")) {
                showInfoWindow = !showInfoWindow;
            }
        }
        ImGui.Spacing();
    }

}

#endif
