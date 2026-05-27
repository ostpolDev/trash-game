using System.Collections.Generic;
using System.IO;

namespace Engine.Serialization.Entries;

public class ListEntry : AbstractEntry {

    public List<object> Data = [];
    private DictionaryEntryType ListType = DictionaryEntryType.INVALID;

    public int Count { get { return Data.Count; } }

    public ListEntry() { }

    public ListEntry(DictionaryEntryType type, IEnumerable<object> data) {
        ListType = type;
        Data = [.. data];
    }

    public ListEntry(DictionaryEntryType type, params object[] data) {
        ListType = type;
        Data = [.. data];
    }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.LIST;
    }

    public override void Read(BinaryReader reader) {
        ListType = (DictionaryEntryType)reader.ReadByte();
        uint length = reader.ReadUInt32();

        for (int i = 0; i < length; i++) {
            Data[i] = ListType.ReadBinary(reader);
        }
    }

    public override void Write(BinaryWriter writer) {
        writer.Write((byte)ListType);
        writer.Write((uint)Count);

        for (int i = 0; i < Count; i++) {
            ListType.WriteBinary(writer, Data[i]);
        }

    }

    public object this[int i] {
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
