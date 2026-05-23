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

#if DEBUG

internal class FilePickerWindow : EditorWindow {

    public string ResultPath { get; private set; }
    public TargetType PickerMode { get; private set; } = TargetType.FILE;
    public SelectionMode SelectMode { get; private set; } = SelectionMode.IMPORT;
    public bool WasCancelled { get; private set; } = false;

    private string ItemName = "";
    private string CurrentPath = "";

    private readonly List<PathItem> PathItems = [];

    private bool IsLoading = false;

    private static readonly PathPreset[] PRESET_PATHS;

    private readonly string FallbackDir = PathHelper.GetAppDirectory();
    private long ms = 0;
    private readonly Stopwatch sw = new();
    public string ctx = null;

    public FilePickerWindow(TargetType pickerMode, SelectionMode selectionMode) {
        PickerMode = pickerMode;
        SelectMode = selectionMode;
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
            SwitchPath(Directory.GetParent(CurrentPath)?.FullName);
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
                ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch, 0.8f);
                ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableHeadersRow();

                if (Directory.GetParent(CurrentPath) != null) {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Selectable("..", false, ImGuiSelectableFlags.SpanAllColumns);
                    if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left)) {
                        SwitchPath(Directory.GetParent(CurrentPath).FullName);
                    }
                    ImGui.TableNextColumn();
                    ImGui.Text("");
                }

                foreach (var item in PathItems) {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    if (ImGui.Selectable($"{(item.Type == TargetType.DIRECTORY ? "+ " : "")}{item.Name}", false, ImGuiSelectableFlags.SpanAllColumns)) {
                        if (item.Type == PickerMode) {
                            ItemName = item.Name;
                        }
                    }

                    if (item.Type == TargetType.DIRECTORY && ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left)) {
                        SwitchPath(item.Path);
                        break;
                    }

                    ImGui.TableNextColumn();
                    ImGui.Text(item.DisplayType ?? item.Type.ToString());
                    
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
            if (SelectMode == SelectionMode.EXPORT) {
                ItemName = PathHelper.MakeFileSafe(ItemName);
            }
            ResultPath = Path.Combine(CurrentPath, ItemName);
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
        if (string.IsNullOrEmpty(path)) return;

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
            TargetType mode = TargetType.FILE;
            if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory) {
                mode = TargetType.DIRECTORY;
            }

            string ext = Path.GetExtension(item);
            if (string.IsNullOrEmpty(ext)) {
                ext = null;
            }
            items.Add(new(Path.GetFileName(item), item, ext, mode));
        }
        PathItems.AddRange(items.OrderByDescending(p => p.Type).ThenBy(p => p.Name));

        IsLoading = false;
        sw.Stop();
        ms = sw.ElapsedMilliseconds;
    }

    public enum TargetType {
        FILE, DIRECTORY
    }

    public enum SelectionMode {
        IMPORT, EXPORT
    }

    static FilePickerWindow() {
        PRESET_PATHS = [
                new("App Data", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)),
                new("Documents", Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments)),
                new("Desktop", Environment.GetFolderPath(Environment.SpecialFolder.Desktop)),
                new("Favorites", Environment.GetFolderPath(Environment.SpecialFolder.Favorites)),
                new("Game Dir", PathHelper.GetAppDirectory())
        ];
    }

    private struct PathPreset(string name, string path) {
        public string Name = name ?? throw new ArgumentNullException(nameof(name));
        public string Path = path ?? throw new ArgumentNullException(nameof(path));
    }

    private struct PathItem(string name, string path, string displayType, TargetType type) : IComparable<PathItem> {
        public string Name = name ?? throw new ArgumentNullException(nameof(name));
        public string Path = path ?? throw new ArgumentNullException(nameof(path));
        public TargetType Type = type;
        public string DisplayType = displayType;

        public readonly int CompareTo(PathItem other) {
            return (int)other.Type - (int)Type;
        }
    }

}

#endif
