using System.IO;

namespace Engine.Serialization.Entries;

public class InvalidEntry : AbstractEntry {

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.INVALID;
    }

    public override void Read(BinaryReader reader) { }

    public override void Write(BinaryWriter writer) { }

    public override string ToString() {
        return "INVALID";
    }

}
