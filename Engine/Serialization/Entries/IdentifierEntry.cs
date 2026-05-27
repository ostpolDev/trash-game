using Engine.Utility;
using ImGuiNET;
using System.IO;

namespace Engine.Serialization.Entries;

public class IdentifierEntry : AbstractEntry {

    public Identifier Data = new("", "");

    public IdentifierEntry() { }

    public IdentifierEntry(Identifier data) {
        Data = data;
    }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.IDENTIFIER;
    }

    public override void Read(BinaryReader reader) {
        Data = new(reader.ReadString(), reader.ReadString());
    }

    public override void Write(BinaryWriter writer) {
        writer.Write(Data.Namespace);
        writer.Write(Data.Key);
    }

    public override void RenderDebugEditor() {
        ImGui.InputText("Namespace", ref Data.Namespace, 128);
        ImGui.InputText("Path", ref Data.Key, 128);
    }

    public override string ToString() {
        return Data.ToString();
    }

}
