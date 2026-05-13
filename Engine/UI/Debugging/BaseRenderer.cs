using ImGuiNET;
using System;
using System.Linq;

namespace Engine.UI.Debugging;

public class BaseRenderer(AbstractUIComponent component) : UIDebugRenderer<AbstractUIComponent>(component) {

    private static readonly string[] AnchorPositions = [.. Enum.GetValues<UIAnchorPosition>().Select(m => m.ToString())];

    private readonly int[] Position = [component.LocalArea.X, component.LocalArea.Y];
    private readonly int[] Size = [component.LocalArea.Width, component.LocalArea.Height];
    private int AnchorPosition = (int)component.AnchorPosition;
    private string ReferenceID = component.ReferenceID ?? "";

    public override void Render() {
        if (ImGui.CollapsingHeader("AbstractUIComponent", ImGuiTreeNodeFlags.DefaultOpen)) {
            if (ImGui.InputText("ID", ref ReferenceID, 128)) {
                Component.SetReferenceID(ReferenceID);
            }
            if (ImGui.Combo("Anchor", ref AnchorPosition, AnchorPositions, AnchorPositions.Length)) {
                Component.SetAnchorPosition((UIAnchorPosition)AnchorPosition);
            }
            if (ImGui.InputInt2("Position", ref Position[0])) {
                Component.SetPosition(Position[0], Position[1]);
            }
            if (ImGui.InputInt2("Area", ref Size[0])) {
                Component.SetArea(Size[0], Size[1]);
            }
        }
    }

}
