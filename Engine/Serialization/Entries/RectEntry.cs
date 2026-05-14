using Microsoft.Xna.Framework;
using System.IO;

namespace Engine.Serialization.Entries;

public class RectEntry : AbstractEntry {

    public Rectangle Data;

    public RectEntry() { }

    public RectEntry(Rectangle r) {
        Data = r;
    }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.RECTANGLE;
    }

    public override void Read(BinaryReader reader) {
        Data = new() {
            X = reader.ReadInt32(),
            Y = reader.ReadInt32(),
            Width = reader.ReadInt32(),
            Height = reader.ReadInt32()
        };
    }

    public override void Write(BinaryWriter writer) {
        writer.Write(Data.X);
        writer.Write(Data.Y);
        writer.Write(Data.Width);
        writer.Write(Data.Height);
    }

    public override string ToString() {
        return Data.ToString();
    }

}
