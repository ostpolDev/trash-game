using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Serialization.Entries;

public class ListEntry : AbstractEntry {

    public List<object> Data = [];
    public DictionaryEntryType ListType { get; private set; } = DictionaryEntryType.INVALID;

    public int Count { get { return Data.Count; } }

    public ListEntry() { }

    public ListEntry(DictionaryEntryType type) {
        ListType = type;
    }

    public ListEntry(DictionaryEntryType type, IEnumerable<object> data) {
        ListType = type;
        Data = [.. data];
    }

    public ListEntry(DictionaryEntryType type, object[] data) {
        ListType = type;
        Data = [.. data];
    }

    public static ListEntry CreateFromData<T>(DictionaryEntryType type, T[] data) {
        return new(type, Array.ConvertAll<T, object>(data, x => x));
    }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.LIST;
    }

    public override void Read(BinaryReader reader) {
        ListType = (DictionaryEntryType)reader.ReadByte();
        uint length = reader.ReadUInt32();

        Data.Clear();
        Data.EnsureCapacity((int)length);

        for (int i = 0; i < length; i++) {
            Data.Add(ListType.ReadBinary(reader));
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

#if DEBUG
    public override void RenderDebugEditor() {

    }
#endif

}
