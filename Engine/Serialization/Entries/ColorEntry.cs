using ImGuiNET;
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

#if DEBUG
    public override void RenderDebugEditor() {
        System.Numerics.Vector4 cVec = Data.ToVector4().ToNumerics();
        if (ImGui.ColorPicker4("Color", ref cVec)) {
            Data = new(cVec.X, cVec.Y, cVec.Z, cVec.Z);
        }
    }
#endif

}
