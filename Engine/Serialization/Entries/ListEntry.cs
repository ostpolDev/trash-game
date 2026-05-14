using System.Collections.Generic;
using System.IO;

namespace Engine.Serialization.Entries;

public class ListEntry : AbstractEntry {

    public List<AbstractEntry> Data = [];
    private DictionaryEntryType ListType = DictionaryEntryType.INVALID;

    public int Count { get { return Data.Count; } }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.LIST;
    }

    public override void Read(BinaryReader reader) {
        ListType = (DictionaryEntryType)reader.ReadByte();
        uint length = reader.ReadUInt32();

        for (int i = 0; i < length; i++) {
            AbstractEntry entry = GetEntryFromType(ListType) ?? throw new System.NullReferenceException();
            entry.Read(reader);
            Data.Add(entry);
        }
    }

    public override void Write(BinaryWriter writer) {
        ListType = Count > 0 ? Data[0].GetEntryType() : DictionaryEntryType.BYTE;

        writer.Write((byte)ListType);
        writer.Write((uint)Count);

        for (int i = 0; i < Count; i++) {
            Data[i].Write(writer);
        }

    }

    public AbstractEntry this[int i] {
        get {
            return Data[i];
        }
        set {
            Data[i] = value;
        }
    }

    public override string ToString() {
        return $"{ListType} x{Count}";
    }

    public override void RenderDebugEditor() {

    }

}
