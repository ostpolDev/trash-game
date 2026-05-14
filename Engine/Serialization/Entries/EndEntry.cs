using System.IO;

namespace Engine.Serialization.Entries;

public class EndEntry : AbstractEntry {
    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.END;
    }

    public override void Read(BinaryReader reader) { }

    public override void Write(BinaryWriter writer) { }

    public override string ToString() {
        return "END";
    }

}
