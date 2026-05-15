using ImGuiNET;
using System.IO;

namespace Engine.Serialization.Entries;

public class IntEntry : AbstractEntry {

    public int Data;

    public IntEntry() { }

    public IntEntry(int i) {
        Data = i;
    }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.INT;
    }

    public override void Read(BinaryReader reader) {
        Data = reader.ReadInt32();
    }

    public override void Write(BinaryWriter writer) {
        writer.Write(Data);
    }

    public override string ToString() {
        return Data.ToString();
    }

    public override void RenderDebugEditor() {
        ImGui.InputInt("Value##int", ref Data);
    }

}
