using ImGuiNET;
using System;

namespace Engine.Editors.Windows;

#if DEBUG

internal class ConfirmationWindow : EditorWindow {

    public delegate void ConfirmationWindowCallback(Buttons button);

    private readonly ConfirmationWindowCallback confirmationWindowCallback;
    public readonly Buttons confirmationWindowButtons;

    private readonly string Title;
    private readonly string Message;

    public ConfirmationWindow(ConfirmationWindowCallback callback, string message, string title = null, Buttons buttons = Buttons.OK) {
        confirmationWindowCallback = callback ?? throw new ArgumentNullException(nameof(callback));
        confirmationWindowButtons = buttons;

        Title = title ?? "Info";
        Message = message;
    }

    public override bool Draw() {
        ImGui.Begin(Title, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking);

        if (Message != null)
            ImGui.Text(Message);

        ImGui.Spacing();

        if (confirmationWindowButtons.HasFlag(Buttons.OK)) {
            if (ImGui.Button("Ok##confirmWindowOk")) {
                confirmationWindowCallback?.Invoke(Buttons.OK);
                ImGui.SameLine();
            }
        }
        if (confirmationWindowButtons.HasFlag(Buttons.CANCEL)) {
            if (ImGui.Button("Cancel##confirmWindowCancel")) {
                confirmationWindowCallback?.Invoke(Buttons.CANCEL);
                ImGui.SameLine();
            }
        }
        if (confirmationWindowButtons.HasFlag(Buttons.ACCEPT)) {
            if (ImGui.Button("Accept##confirmWindowAccept")) {
                confirmationWindowCallback?.Invoke(Buttons.ACCEPT);
                ImGui.SameLine();
            }
        }
        if (confirmationWindowButtons.HasFlag(Buttons.ABORT)) {
            if (ImGui.Button("Abort##confirmWindowAbort")) {
                confirmationWindowCallback?.Invoke(Buttons.ABORT);
                ImGui.SameLine();
            }
        }
        if (confirmationWindowButtons.HasFlag(Buttons.YES)) {
            if (ImGui.Button("Yes##confirmWindowYes")) {
                confirmationWindowCallback?.Invoke(Buttons.YES);
                ImGui.SameLine();
            }
        }
        if (confirmationWindowButtons.HasFlag(Buttons.NO)) {
            if (ImGui.Button("No##confirmWindowNo")) {
                confirmationWindowCallback?.Invoke(Buttons.NO);
                ImGui.SameLine();
            }
        }

        ImGui.Text("");

        ImGui.End();
        return false;
    }

    [Flags]
    public enum Buttons {
        OK = 2,
        CANCEL = 4,
        ACCEPT = 8,
        ABORT = 16,
        YES = 32,
        NO = 64
    }

}

#endif
