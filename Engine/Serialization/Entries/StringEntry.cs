using System.IO;

namespace Engine.Serialization.Entries;

public class StringEntry : AbstractEntry {

    public string Data;

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
        return Data;
    }

    public override void RenderDebugEditor() {

    }

}
