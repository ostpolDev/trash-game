using System.IO;

namespace Engine.Serialization.Entries;

public class ByteEntry : AbstractEntry {

    public byte Data = 0;

    public ByteEntry() { }

    public ByteEntry(byte data) {
        Data = data;
    }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.BYTE;
    }

    public override void Read(BinaryReader reader) {
        Data = reader.ReadByte();
    }

    public override void Write(BinaryWriter writer) {
        writer.Write(Data);
    }

    public override string ToString() {
        return Data.ToString();
    }

    public override void RenderDebugEditor() {
        
    }

}
