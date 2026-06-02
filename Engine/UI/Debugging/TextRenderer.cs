using Engine.UI.Components;
using FontStashSharp;
using ImGuiNET;
using System;
using System.Linq;
using System.Numerics;

namespace Engine.UI.Debugging;

internal class TextRenderer(TextUIComponent component) : BaseRenderer(component) {

    private static readonly string[] TextStyles = [.. Enum.GetValues<TextStyle>().Select(m => m.ToString())];

    private string Text = component.Text;
    private int FontSize = component.FontSize;
    private Vector4 Color = component.Color.ToVector4().ToNumerics();
    private Vector2 Scale = component.Scale.ToNumerics();

    private int TextStyle = (int)component.TextStyle;
    

    public override void Render() {
        base.Render();

        TextUIComponent component = (TextUIComponent)Component;
        
        if (ImGui.CollapsingHeader("Text")) {
            if (ImGui.InputTextMultiline("Text##input", ref Text, 2048, new())) {
                component.UpdateText(Text);
            }
            if (ImGui.InputInt("Font Size", ref FontSize)) {
                FontSize = Math.Min(0, FontSize);
                component.UpdateFontSize(FontSize);
            }
            if (ImGui.ColorPicker4("Color##text", ref Color)) {
                component.Color = new(Color);
            }
            ImGui.Spacing();
            if (ImGui.InputFloat2("Scale##text", ref Scale)) {
                component.Scale = new(Scale.X, Scale.Y);
            }
            ImGui.InputFloat("Character Spacing", ref component.CharacterSpacing);
            ImGui.InputFloat("Line Spacing", ref component.LineSpacing);
            if (ImGui.Combo("Style", ref TextStyle, TextStyles, TextStyles.Length)) {
                component.TextStyle = (TextStyle)TextStyle;
            }
        }
    }

}
