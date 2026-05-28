using System.IO;

namespace Engine.Serialization.Entries;

public abstract class AbstractEntry {

    public string Key { get; private set; }

    public abstract void Write(BinaryWriter writer);
    public abstract void Read(BinaryReader reader);
    public abstract DictionaryEntryType GetEntryType();

#if DEBUG
    public abstract void RenderDebugEditor();
#endif

    public AbstractEntry SetKey(string key) {
        Key = key;
        return this;
    }

    public static AbstractEntry GetEntryFromType(DictionaryEntryType type) {
        return type switch {
            DictionaryEntryType.END => new EndEntry(),
            DictionaryEntryType.BYTE => new ByteEntry(),
            DictionaryEntryType.SHORT => new ShortEntry(),
            DictionaryEntryType.INT => new IntEntry(),
            DictionaryEntryType.LONG => new LongEntry(),
            DictionaryEntryType.FLOAT => new FloatEntry(),
            DictionaryEntryType.DOUBLE => new DoubleEntry(),
            DictionaryEntryType.STRING => new StringEntry(),
            DictionaryEntryType.LIST => new ListEntry(),
            DictionaryEntryType.COLOR => new ColorEntry(),
            DictionaryEntryType.VECTOR2 => new Vector2Entry(),
            DictionaryEntryType.VECTOR3 => new Vector3Entry(),
            DictionaryEntryType.RECTANGLE => new RectEntry(),
            DictionaryEntryType.DICTIONARY => new SerializableDictionary(),
            DictionaryEntryType.INVALID => new InvalidEntry(),
            DictionaryEntryType.IDENTIFIER => new IdentifierEntry(),
            DictionaryEntryType.BOOL => new BoolEntry(),
            _ => null,
        };
    }

    public static AbstractEntry ReadEntry(BinaryReader reader) {
        DictionaryEntryType type = (DictionaryEntryType)reader.ReadByte();
        if (type == DictionaryEntryType.END) {
            return new EndEntry();
        } else if (type == DictionaryEntryType.INVALID) {
            return new InvalidEntry();
        }

        AbstractEntry entry = GetEntryFromType(type);
        entry.SetKey(reader.ReadString());
        entry.Read(reader);
        return entry;
    }

    public static void WriteEntry(AbstractEntry entry, BinaryWriter writer) {
        writer.Write((byte)entry.GetEntryType());
        if (entry.GetEntryType() != DictionaryEntryType.END && entry.GetEntryType() != DictionaryEntryType.INVALID) {
            writer.Write(entry.Key);
            entry.Write(writer);
        }
    }

}
