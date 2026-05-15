using Engine.Debugging;
using Engine.Utility;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Engine.Editors.Windows;

internal class FilePickerWindow : EditorWindow {

    public string ResultPath { get; private set; }
    public Mode PickerMode = Mode.FILE;
    public bool WasCancelled { get; private set; } = false;

    private string ItemName = "";
    private string CurrentPath = "";

    private readonly List<PathItem> PathItems = [];

    private bool IsLoading = false;

    private static readonly PathPreset[] PRESET_PATHS;

    private readonly string FallbackDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private long ms = 0;
    private readonly Stopwatch sw = new();

    public FilePickerWindow(Mode pickerMode) {
        PickerMode = pickerMode;
        SwitchPath(FallbackDir);
    }

    public override bool Draw() {
        ImGui.Begin($"Select a {PickerMode.ToString().ToLower()}", ImGuiWindowFlags.NoDocking);

        if (PRESET_PATHS != null && ImGui.CollapsingHeader("Presets")) {
            foreach (var item in PRESET_PATHS) {
                if (ImGui.Button(item.Name)) {
                    SwitchPath(item.Path);
                }
            }
        }

        if (ImGui.InputText("Path", ref CurrentPath, 300, ImGuiInputTextFlags.EnterReturnsTrue)) {
            SwitchPath(CurrentPath);
        }
        ImGui.SameLine();
        if (ImGui.Button("Go")) {
            SwitchPath(CurrentPath);
        }
        ImGui.SameLine();
        if (ImGui.ArrowButton("##Up", ImGuiDir.Up)) {
            SwitchPath(Directory.GetParent(CurrentPath).FullName);
        }
        ImGui.Text($"{PathItems.Count} Item{(PathItems.Count != 1 ? "s" : "")} ({ms} ms)");

        ImGui.Spacing();
        ImGui.Separator();

        if (IsLoading) {
            ImGui.Text("Loading...");
        } else {
            ImGuiTableFlags flags = ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY;

            Vector2 outerSize = new(0, 250);

            if (ImGui.BeginTable("##table", 2, flags, outerSize)) {
                ImGui.TableSetupColumn("Name");
                ImGui.TableSetupColumn("Type");
                ImGui.TableHeadersRow();

                foreach (var item in PathItems) {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    if (ImGui.Selectable($"{(item.Type == Mode.DIRECTORY ? "+ " : "")}{item.Name}", false, ImGuiSelectableFlags.SpanAllColumns)) {
                        if (item.Type == PickerMode) {
                            ItemName = item.Path;
                        }
                    }

                    if (item.Type == Mode.DIRECTORY && ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left)) {
                        SwitchPath(item.Path);
                        break;
                    }

                    ImGui.TableNextColumn();
                    ImGui.Text(item.Type.ToString());
                    
                }

                
                ImGui.EndTable();
            }

        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        ImGui.InputText("Name", ref ItemName, 300);
        if (ImGui.Button("Select")) {
            ImGui.End();
            ResultPath = ItemName;
            return true;
        }
        ImGui.SameLine();
        if (ImGui.Button("Cancel")) {
            ImGui.End();
            WasCancelled = true;
            return true;
        }

        ImGui.End();
        return false;
    }

    private void SwitchPath(string path) {
        if (IsLoading) return;
        sw.Reset();
        sw.Start();

        IsLoading = true;
        CurrentPath = path;
        PathItems.Clear();

        FileInfo fi = null;
        try {

            fi = new(CurrentPath);

        } catch (Exception ex) {
            Logger.Shared.Exception(ex);
            IsLoading = false;
            if (path != FallbackDir) {
                SwitchPath(FallbackDir);
            }
            return;
        }

        if (fi is null) {
            SwitchPath(FallbackDir);
            return;
        }

        FileAttributes attr = File.GetAttributes(CurrentPath);
        if ((attr & FileAttributes.Directory) != FileAttributes.Directory) {
            SwitchPath(FallbackDir);
            return;
        }

        List<PathItem> items = [];
        foreach (string item in Directory.EnumerateFileSystemEntries(CurrentPath)) {
            FileAttributes fileAttr = File.GetAttributes(item);
            Mode mode = Mode.FILE;
            if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory) {
                mode = Mode.DIRECTORY;
            }

            items.Add(new(Path.GetFileName(item), item, mode));
        }
        PathItems.AddRange(items.OrderByDescending(p => p.Type).ThenBy(p => p.Name));

        IsLoading = false;
        sw.Stop();
        ms = sw.ElapsedMilliseconds;
    }

    public enum Mode {
        FILE, DIRECTORY
    }

#if DEBUG
    static FilePickerWindow() {
        PRESET_PATHS = [
                new("App Data", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)),
                new("Documents", Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments)),
                new("Desktop", Environment.GetFolderPath(Environment.SpecialFolder.Desktop)),
                new("Favorites", Environment.GetFolderPath(Environment.SpecialFolder.Favorites)),
                new("Game Dir", PathHelper.GetAppDirectory())
        ];
    }
#endif

    private struct PathPreset(string name, string path) {
        public string Name = name ?? throw new ArgumentNullException(nameof(name));
        public string Path = path ?? throw new ArgumentNullException(nameof(path));
    }

    private struct PathItem(string name, string path, Mode type) : IComparable<PathItem> {
        public string Name = name ?? throw new ArgumentNullException(nameof(name));
        public string Path = path ?? throw new ArgumentNullException(nameof(path));
        public Mode Type = type;

        public readonly int CompareTo(PathItem other) {
            return (int)other.Type - (int)Type;
        }
    }

}
