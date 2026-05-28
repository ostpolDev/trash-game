using ImGuiNET;
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

#if DEBUG
    public override void RenderDebugEditor() {
        int[] Pos = [Data.X, Data.Y];
        int[] Size = [Data.Width, Data.Height];
        if (ImGui.InputInt2("Position##rect", ref Pos[0])) {
            Data.X = Pos[0];
            Data.Y = Pos[1];
        }
        if (ImGui.InputInt2("Size##rect", ref Size[0])) {
            Data.Width = Size[0];
            Data.Height = Size[1];
        }
    }
#endif

}
