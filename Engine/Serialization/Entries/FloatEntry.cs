using ImGuiNET;
using System.IO;

namespace Engine.Serialization.Entries;

public class FloatEntry : AbstractEntry {

    public float Data;

    public FloatEntry() { }

    public FloatEntry(float f) {
        Data = f;
    }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.FLOAT;
    }

    public override void Read(BinaryReader reader) {
        Data = reader.ReadSingle();
    }

    public override void Write(BinaryWriter writer) {
        writer.Write(Data);
    }

    public override string ToString() {
        return Data.ToString();
    }

    public override void RenderDebugEditor() {
        ImGui.InputFloat("Value##float", ref Data);
    }

}
