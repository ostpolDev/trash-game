using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Engine.UI.Debugging;

#if DEBUG

public class BaseRenderer(AbstractUIComponent component) : UIDebugRenderer(component) {

    private static readonly string[] AnchorPositions = [.. Enum.GetValues<UIAnchorPosition>().Select(m => m.ToString())];

    private readonly int[] Position = [component.LocalArea.X, component.LocalArea.Y];
    private readonly int[] Size = [component.LocalArea.Width, component.LocalArea.Height];
    private int AnchorPosition = (int)component.AnchorPosition;
    private string ReferenceID = component.ReferenceID ?? "";
    private int ZIndex = component.RelativeZIndex;

    private int MinWidth = (int)component.Constraints[0].X;
    private int MaxWidth = (int)component.Constraints[0].Y;
    private int MinHeight = (int)component.Constraints[1].X;
    private int MaxHeight = (int)component.Constraints[1].Y;

    private readonly int[] ScissorPosition = [component.ScissorRectangle.X, component.ScissorRectangle.Y];
    private readonly int[] ScissorSize = [component.ScissorRectangle.Width, component.ScissorRectangle.Height];
    private bool EnableScissor = component.EnableScissor;
    private int ScissorAnchorPosition = (int)component.ScissorAnchor;

    public override void Render() {
        if (ImGui.CollapsingHeader("Properties", ImGuiTreeNodeFlags.DefaultOpen)) {
            if (ImGui.InputText("ID", ref ReferenceID, 128)) {
                Component.SetReferenceID(ReferenceID);
            }
            ImGui.Text($"Screen: ({Component.ScreenArea.X} | {Component.ScreenArea.Y})");
            ImGui.Text($"Global Z: {Component.ZIndex}");
            ImGui.Text($"Children: {Component.ChildCount}");
            ImGui.Text($"Depth: {Component.Depth}");
            ImGui.Text($"Index: {Component.Index}");
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

        if (ImGui.CollapsingHeader("Responsive")) {
            ImGui.SeparatorText("Stretching");
            if (ImGui.Checkbox("Vertical##stretch", ref Component.Stretch[1])) {
                Component.RecalculateScreenPosition();
            }
            if (ImGui.Checkbox("Horizontal##stretch", ref Component.Stretch[0])) {
                Component.RecalculateScreenPosition();
            }
            ImGui.Spacing();
            if (ImGui.InputInt4("Padding", ref Component.Padding[0])) {
                Component.RecalculateScreenPosition();
            }

        }
        ImGui.Spacing();

        if (ImGui.CollapsingHeader("Constraints")) {
            if (ImGui.Checkbox("Min. Width", ref Component.ConstraintsEnabled[0])) {
                Component.RecalculateScreenPosition();
            }
            if (Component.ConstraintsEnabled[0]) {
                if (ImGui.InputInt("Amount##minwidth", ref MinWidth)) {
                    Component.Constraints[0].X = MinWidth;
                    Component.RecalculateScreenPosition();
                }
            }
            ImGui.Spacing();

            if (ImGui.Checkbox("Max. Width", ref Component.ConstraintsEnabled[1])) {
                Component.RecalculateScreenPosition();
            }
            if (Component.ConstraintsEnabled[1]) {
                if (ImGui.InputInt("Amount##maxwidth", ref MaxWidth)) {
                    Component.Constraints[0].Y = MaxWidth;
                    Component.RecalculateScreenPosition();
                }
            }
            ImGui.Spacing();

            if (ImGui.Checkbox("Min. Height", ref Component.ConstraintsEnabled[2])) {
                Component.RecalculateScreenPosition();
            }
            if (Component.ConstraintsEnabled[2]) {
                if (ImGui.InputInt("Amount##minheight", ref MinHeight)) {
                    Component.Constraints[1].X = MinHeight;
                    Component.RecalculateScreenPosition();
                }
            }
            ImGui.Spacing();

            if (ImGui.Checkbox("Max. Height", ref Component.ConstraintsEnabled[3])) {
                Component.RecalculateScreenPosition();
            }
            if (Component.ConstraintsEnabled[3]) {
                if (ImGui.InputInt("Amount##maxheight", ref MaxHeight)) {
                    Component.Constraints[1].Y = MaxHeight;
                    Component.RecalculateScreenPosition();
                }
            }
            ImGui.Spacing();
        }
        ImGui.Spacing();

        if (ImGui.CollapsingHeader("Masking")) {
            if (ImGui.Checkbox("Enable Scissor Mask", ref EnableScissor)) {
                Component.SetScissor(new Rectangle(ScissorPosition[0], ScissorPosition[1], ScissorSize[0], ScissorSize[1]), EnableScissor, (UIAnchorPosition)ScissorAnchorPosition);
            }
            if (EnableScissor) {
                if (ImGui.Combo("Anchor##Scissor", ref ScissorAnchorPosition, AnchorPositions, AnchorPositions.Length)) {
                    Component.SetScissor(new Rectangle(ScissorPosition[0], ScissorPosition[1], ScissorSize[0], ScissorSize[1]), EnableScissor, (UIAnchorPosition)ScissorAnchorPosition);
                }
                if (ImGui.InputInt2("Mask Position", ref ScissorPosition[0])) {
                    Component.SetScissor(new Rectangle(ScissorPosition[0], ScissorPosition[1], ScissorSize[0], ScissorSize[1]), EnableScissor, (UIAnchorPosition)ScissorAnchorPosition);
                }
                if (ImGui.InputInt2("Mask Size", ref ScissorSize[0])) {
                    Component.SetScissor(new Rectangle(ScissorPosition[0], ScissorPosition[1], ScissorSize[0], ScissorSize[1]), EnableScissor, (UIAnchorPosition)ScissorAnchorPosition);
                }
                if (ImGui.Button("Auto Mask")) {
                    ScissorPosition[0] = 0;
                    ScissorPosition[1] = 0;
                    ScissorSize[0] = Component.LocalArea.Width;
                    ScissorSize[1] = Component.LocalArea.Height;
                    Component.SetScissor(new Rectangle(ScissorPosition[0], ScissorPosition[1], ScissorSize[0], ScissorSize[1]), EnableScissor, (UIAnchorPosition)ScissorAnchorPosition);
                }
            }
        }
        ImGui.Spacing();
    }

}

#endif
