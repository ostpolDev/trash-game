using Microsoft.Xna.Framework;
using System.IO;

namespace Engine.Serialization.Entries;

public class Vector2Entry : AbstractEntry {

    public Vector2 Data;

    public Vector2Entry() { }

    public Vector2Entry(Vector2 v) {
        Data = v;
    }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.VECTOR2;
    }

    public override void Read(BinaryReader reader) {
        Data = new() {
            X = reader.ReadSingle(),
            Y = reader.ReadSingle()
        };
    }

    public override void Write(BinaryWriter writer) {
        writer.Write(Data.X);
        writer.Write(Data.Y);
    }

    public override string ToString() {
        return Data.ToString();
    }

    public override void RenderDebugEditor() {

    }

}
