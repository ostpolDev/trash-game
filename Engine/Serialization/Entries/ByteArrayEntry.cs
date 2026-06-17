using ImGuiNET;
using System;
using System.IO;

namespace Engine.Serialization.Entries;

public class ByteArrayEntry : AbstractEntry {

    public byte[] Data;

    public ByteArrayEntry() { }

    public ByteArrayEntry(byte[] data) { Data = data; }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.BYTE_ARRAY;
    }

    public override void Read(BinaryReader reader) {
        int size = reader.ReadInt32();
        if (size == 0) {
            Data = [];
        }
        Data = reader.ReadBytes(size);
    }

    public override void Write(BinaryWriter writer) {
        if (Data == null) {
            writer.Write(0);
            return;
        }

        writer.Write(Data.Length);
        writer.Write(Data);
    }

    public override string ToString() {
        return $"{Data?.Length.ToString() ?? "Empty"} byte array";
    }

#if DEBUG

    private int newsize = 0;

    public override void RenderDebugEditor() {
        ImGui.Text($"{Data?.Length ?? 0} entries");
        if (Data == null) {
            if (ImGui.InputInt("New Size", ref newsize)) {
                newsize = Math.Max(0, newsize);
            }
            if (ImGui.Button("Create##bytearray")) {
                Data = new byte[Math.Max(newsize, 0)];
            }
        } else {
            if (ImGui.BeginListBox("Items")) {
                foreach (byte item in Data) {
                    ImGui.Text($"{item}");
                }
                ImGui.EndListBox();
            }

        }
    }

#endif

}
