using Engine.Serialization.Entries;
using Engine.Utility;
using Microsoft.Xna.Framework;
using System.IO;

namespace Engine.Serialization;

public static class SerializationExtensions {

    public static void WriteBinary(this DictionaryEntryType type, BinaryWriter writer, object data) {
        switch (type) {
            case DictionaryEntryType.BYTE:
                writer.Write((byte)data);
                break;
            case DictionaryEntryType.SHORT:
                writer.Write((short)data);
                break;
            case DictionaryEntryType.INT:
                writer.Write((int)data);
                break;
            case DictionaryEntryType.LONG:
                writer.Write((long)data);
                break;
            case DictionaryEntryType.FLOAT:
                writer.Write((float)data);
                break;
            case DictionaryEntryType.DOUBLE:
                writer.Write((double)data);
                break;
            case DictionaryEntryType.STRING:
                writer.Write((string)data);
                break;
            case DictionaryEntryType.COLOR:
                new ColorEntry((Color)data).Write(writer);
                break;
            case DictionaryEntryType.VECTOR2:
                new Vector2Entry((Vector2)data).Write(writer);
                break;
            case DictionaryEntryType.VECTOR3:
                new Vector3Entry((Vector3)data).Write(writer);
                break;
            case DictionaryEntryType.RECTANGLE:
                new RectEntry((Rectangle)data).Write(writer);
                break;
            case DictionaryEntryType.IDENTIFIER:
                new IdentifierEntry((Identifier)data).Write(writer);
                break;
            case DictionaryEntryType.BOOL:
                new BoolEntry((bool)data).Write(writer);
                break;
            default:
                throw new System.ArgumentException($"Cannot write binary value for entry type: {type}", nameof(type));
        }
    }

    public static object ReadBinary(this DictionaryEntryType type, BinaryReader reader) {
        switch (type) {
            case DictionaryEntryType.BYTE:
                return reader.ReadByte();
            case DictionaryEntryType.SHORT:
                return reader.ReadInt16();
            case DictionaryEntryType.INT:
                return reader.ReadInt32();
            case DictionaryEntryType.LONG:
                return reader.ReadInt64();
            case DictionaryEntryType.FLOAT:
                return reader.ReadSingle();
            case DictionaryEntryType.DOUBLE:
                return reader.ReadDouble();
            case DictionaryEntryType.STRING:
                return reader.ReadString();
            case DictionaryEntryType.COLOR:
                ColorEntry colorEntry = new();
                colorEntry.Read(reader);
                return colorEntry.Data;
            case DictionaryEntryType.VECTOR2:
                Vector2Entry vector2Entry = new();
                vector2Entry.Read(reader);
                return vector2Entry.Data;
            case DictionaryEntryType.VECTOR3:
                Vector3Entry vector3Entry = new();
                vector3Entry.Read(reader);
                return vector3Entry.Data;
            case DictionaryEntryType.RECTANGLE:
                RectEntry rectEntry = new();
                rectEntry.Read(reader);
                return rectEntry.Data;
            case DictionaryEntryType.IDENTIFIER:
                IdentifierEntry identifierEntry = new();
                identifierEntry.Read(reader);
                return identifierEntry.Data;
            case DictionaryEntryType.BOOL:
                BoolEntry boolEntry = new();
                boolEntry.Read(reader);
                return boolEntry.Data;
            default:
                throw new System.ArgumentException($"Cannot read binary value for entry type: {type}", nameof(type));
        }
    }

}
