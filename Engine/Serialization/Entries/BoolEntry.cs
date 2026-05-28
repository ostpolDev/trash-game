using ImGuiNET;
using System.IO;

namespace Engine.Serialization.Entries;

public class BoolEntry : AbstractEntry {

    public bool Data;

    public BoolEntry() { }

    public BoolEntry(bool data) {
        Data = data;
    }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.BOOL;
    }

    public override void Read(BinaryReader reader) {
        Data = reader.ReadByte() == 1;
    }

    public override void Write(BinaryWriter writer) {
        writer.Write((byte)(Data ? 1 : 0));
    }

    public override string ToString() {
        return Data.ToString();
    }

#if DEBUG
    public override void RenderDebugEditor() {
        ImGui.Checkbox("Value", ref Data);
    }
#endif

}
