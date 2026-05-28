using ImGuiNET;
using Microsoft.Xna.Framework;
using System.IO;

namespace Engine.Serialization.Entries;

public class Vector3Entry : AbstractEntry {

    public Vector3 Data;

    public Vector3Entry() { }

    public Vector3Entry(Vector3 v) {
        Data = v;
    }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.VECTOR3;
    }

    public override void Read(BinaryReader reader) {
        Data = new() {
            X = reader.ReadSingle(),
            Y = reader.ReadSingle(),
            Z = reader.ReadSingle()
        };
    }

    public override void Write(BinaryWriter writer) {
        writer.Write(Data.X);
        writer.Write(Data.Y);
        writer.Write(Data.Z);
    }

    public override string ToString() {
        return Data.ToString();
    }

#if DEBUG
    public override void RenderDebugEditor() {
        System.Numerics.Vector3 vec = Data.ToNumerics();
        if (ImGui.InputFloat3("Value##vec3", ref vec)) {
            Data.X = vec.X;
            Data.Y = vec.Y;
            Data.Z = vec.Z;
        }
    }
#endif

}
