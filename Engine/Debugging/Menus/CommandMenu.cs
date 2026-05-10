using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Debugging.Menus;

public class CommandMenu : DebugMenu {

    private string cheatInput = "";

    public CommandMenu() : base("command") {
        WindowFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDecoration;
    }

    protected override void OnDrawMenu() {
        if (ImGui.InputText("Command", ref cheatInput, 256, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.EscapeClearsAll)) {
            ProcessCheat(cheatInput);
            cheatInput = "";
        }

        ImGui.SameLine();
        if (ImGui.Button("Submit")) {
            ProcessCheat(cheatInput);
            cheatInput = "";
        }
    }

    private void ProcessCheat(string cheat) {
        System.Diagnostics.Debug.WriteLine(cheat);
    }

    protected override void PrepareWindow() {
        ImGui.SetNextWindowPos(new(0, 0), ImGuiCond.Appearing);
        ImGui.SetNextWindowSize(new(ImGui.GetWindowWidth(), 0), ImGuiCond.Always);
    }

}
