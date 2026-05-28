using ImGuiNET;
using System;
using System.IO;

namespace Engine.Serialization.Entries;

public class LongEntry : AbstractEntry {

    public long Data;

    public LongEntry() { }

    public LongEntry(long l) {
        Data = l;
    }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.LONG;
    }

    public override void Read(BinaryReader reader) {
        Data = reader.ReadInt64();
    }

    public override void Write(BinaryWriter writer) {
        writer.Write(Data);
    }

    public override string ToString() {
        return Data.ToString();
    }

#if DEBUG
    public override void RenderDebugEditor() {
        double val = Convert.ToDouble(Data);
        if (ImGui.InputDouble("Value##long", ref val)) {
            val = Math.Clamp(val, double.MinValue, double.MaxValue);
            Data = Convert.ToInt64(val);
        }
    }
#endif

}
