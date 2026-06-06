using Engine.UI.Components;
using FontStashSharp;
using ImGuiNET;
using System;
using System.Linq;
using System.Numerics;

namespace Engine.UI.Debugging;

#if DEBUG

internal class TextRenderer(TextUIComponent component) : BaseRenderer(component) {

    private static readonly string[] TextStyles = [.. Enum.GetValues<TextStyle>().Select(m => m.ToString())];
    private static readonly string[] TextAlignments = [.. Enum.GetValues<TextUIComponent.TextAlignment>().Select(m => m.ToString())];
    private static readonly string[] TextWraps = [.. Enum.GetValues<TextUIComponent.TextWrapMode>().Select(m => m.ToString())];

    private string Text = component.Text;
    private int FontSize = component.FontSize;
    private Vector4 Color = component.Color.ToVector4().ToNumerics();
    private Vector2 Scale = component.Scale.ToNumerics();

    private int TextStyle = (int)component.TextStyle;
    private int Alignment = (int)component.Alignment;
    private int WrapMode = (int)component.WrapMode;


    public override void Render() {
        base.Render();

        TextUIComponent component = (TextUIComponent)Component;
        
        if (ImGui.CollapsingHeader("Text")) {
            if (ImGui.InputTextMultiline("Text##input", ref Text, 2048, new())) {
                component.UpdateText(Text);
            }
            if (ImGui.Combo("Alignment", ref Alignment, TextAlignments, TextAlignments.Length)) {
                component.SetAlignment((TextUIComponent.TextAlignment)Alignment);
            }
            if (ImGui.InputInt("Font Size", ref FontSize)) {
                FontSize = Math.Max(0, FontSize);
                component.UpdateFontSize(FontSize);
            }
            if (ImGui.ColorPicker4("Color##text", ref Color)) {
                component.Color = new(Color);
            }
            ImGui.Spacing();
            if (ImGui.InputFloat2("Scale##text", ref Scale)) {
                component.Scale = new(Scale.X, Scale.Y);
            }
            if (ImGui.InputFloat("Character Spacing", ref component.CharacterSpacing)) {
                component.UpdateTextRendering();
            }
            if (ImGui.InputFloat("Line Spacing", ref component.LineSpacing)) {
                component.UpdateTextRendering();
            }
            if (ImGui.Combo("Style", ref TextStyle, TextStyles, TextStyles.Length)) {
                component.TextStyle = (TextStyle)TextStyle;
            }

            ImGui.Spacing();

            if (ImGui.Combo("Wrap", ref WrapMode, TextWraps, TextWraps.Length)) {
                component.SetWrapMode((TextUIComponent.TextWrapMode)WrapMode);
            }
            
            if (ImGui.Checkbox("RTL", ref component.RTL)) {
                component.UpdateTextRendering();
            }
        }
    }

}

#endif
