using System.IO;

namespace Engine.Serialization.Entries;

public class ShortEntry : AbstractEntry {

    public short Data;

    public ShortEntry() { }

    public ShortEntry(short s) {
        Data = s;
    }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.SHORT;
    }

    public override void Read(BinaryReader reader) {
        Data = reader.ReadInt16();
    }

    public override void Write(BinaryWriter writer) {
        writer.Write(Data);
    }

    public override string ToString() {
        return Data.ToString();
    }

}
