using System.IO;

namespace Engine.Serialization.Entries;

public class DoubleEntry : AbstractEntry {

    public double Data;

    public DoubleEntry() { }

    public DoubleEntry(double d) {
        Data = d;
    }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.DOUBLE;
    }

    public override void Read(BinaryReader reader) {
        Data = reader.ReadDouble();
    }

    public override void Write(BinaryWriter writer) {
        writer.Write(Data);
    }

    public override string ToString() {
        return Data.ToString();
    }

}
