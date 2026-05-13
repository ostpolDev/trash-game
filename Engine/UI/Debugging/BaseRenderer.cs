using ImGuiNET;
using System;
using System.Linq;

namespace Engine.UI.Debugging;

public class BaseRenderer(AbstractUIComponent component) : UIDebugRenderer(component) {

    private static readonly string[] AnchorPositions = [.. Enum.GetValues<UIAnchorPosition>().Select(m => m.ToString())];

    private readonly int[] Position = [component.LocalArea.X, component.LocalArea.Y];
    private readonly int[] Size = [component.LocalArea.Width, component.LocalArea.Height];
    private int AnchorPosition = (int)component.AnchorPosition;
    private string ReferenceID = component.ReferenceID ?? "";
    private int ZIndex = component.RelativeZIndex;

    public override void Render() {
        if (ImGui.CollapsingHeader("Properties", ImGuiTreeNodeFlags.DefaultOpen)) {
            if (ImGui.InputText("ID", ref ReferenceID, 128)) {
                Component.SetReferenceID(ReferenceID);
            }
        }
        ImGui.Spacing();

        if (ImGui.CollapsingHeader("Transform", ImGuiTreeNodeFlags.DefaultOpen)) {
            if (ImGui.Combo("Anchor", ref AnchorPosition, AnchorPositions, AnchorPositions.Length)) {
                Component.SetAnchorPosition((UIAnchorPosition)AnchorPosition);
            }
            if (ImGui.InputInt2("Position", ref Position[0])) {
                Component.SetPosition(Position[0], Position[1]);
            }
            ImGui.Text($"Screen: ({Component.ScreenArea.X} | {Component.ScreenArea.Y})");
            if (ImGui.InputInt2("Scale##basic", ref Size[0])) {
                Component.SetArea(Size[0], Size[1]);
            }
            if (ImGui.InputInt("Z-Index", ref ZIndex)) {
                Component.SetZIndex(ZIndex);
            }
            ImGui.Text($"Global Z: {Component.ZIndex}");
        }
        ImGui.Spacing();
    }

}
