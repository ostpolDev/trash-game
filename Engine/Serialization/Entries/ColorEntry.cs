using Microsoft.Xna.Framework;
using System.IO;

namespace Engine.Serialization.Entries;

public class ColorEntry : AbstractEntry {

    public Color Data;

    public ColorEntry() { }

    public ColorEntry(Color c) {
        Data = c;
    }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.COLOR;
    }

    public override void Read(BinaryReader reader) {
        Data = new(reader.ReadUInt32());
    }

    public override void Write(BinaryWriter writer) {
        writer.Write(Data.PackedValue);
    }

    public override string ToString() {
        return Data.ToString();
    }

}
