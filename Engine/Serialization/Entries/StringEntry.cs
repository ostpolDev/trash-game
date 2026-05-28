using ImGuiNET;
using System.IO;

namespace Engine.Serialization.Entries;

public class StringEntry : AbstractEntry {

    public string Data = "";

    public StringEntry() { }

    public StringEntry(string s) {
        Data = s;
    }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.STRING;
    }

    public override void Read(BinaryReader reader) {
        Data = reader.ReadString();
    }

    public override void Write(BinaryWriter writer) {
        writer.Write(Data);
    }

    public override string ToString() {
        return Data.ReplaceLineEndings(" ");
    }

#if DEBUG
    public override void RenderDebugEditor() {
        ImGui.InputTextMultiline("Value##text", ref Data, 2048, new());
        ImGui.Text($"{Data.Length} / 2048");
    }
#endif

}
