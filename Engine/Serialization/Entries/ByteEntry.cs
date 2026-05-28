using ImGuiNET;
using System;
using System.IO;

namespace Engine.Serialization.Entries;

public class ByteEntry : AbstractEntry {

    public byte Data = 0;

    public ByteEntry() { }

    public ByteEntry(byte data) {
        Data = data;
    }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.BYTE;
    }

    public override void Read(BinaryReader reader) {
        Data = reader.ReadByte();
    }

    public override void Write(BinaryWriter writer) {
        writer.Write(Data);
    }

    public override string ToString() {
        return Data.ToString();
    }

#if DEBUG
    public override void RenderDebugEditor() {
        int d = Data;
        if (ImGui.InputInt("Value##byte", ref d)) {
            d = Math.Clamp(d, Byte.MinValue, Byte.MaxValue);
            Data = (byte)d;
        }
    }
#endif

}
