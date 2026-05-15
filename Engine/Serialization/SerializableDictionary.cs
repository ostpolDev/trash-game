using Engine.Debugging;
using Engine.Serialization.Entries;
using Engine.Utility.Exceptions;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Engine.Serialization;

public class SerializableDictionary : AbstractEntry {

    public const string FILE_HEADER_STRING = "OSD-v1";

    private static readonly Logger Logger = Logger.Get("Serialization");

    private readonly Dictionary<string, AbstractEntry> Data = [];

    public int Count { get { return Data.Count; } }

    public override DictionaryEntryType GetEntryType() {
        return DictionaryEntryType.DICTIONARY;
    }

    public override void Read(BinaryReader reader) {
        Data.Clear();

        AbstractEntry entry = ReadEntry(reader);
        while (entry.GetEntryType() != DictionaryEntryType.END) {
            Data[entry.Key] = entry;
            entry = ReadEntry(reader);
        }
    }

    public override void Write(BinaryWriter writer) {
        foreach (var item in Data) {
            WriteEntry(item.Value, writer);
        }
        writer.Write((byte)DictionaryEntryType.END);
    }

    private static byte[] GetHeaderBytes() {
        return Encoding.UTF8.GetBytes(FILE_HEADER_STRING);
    }

    public static void WriteToFile(string filePath, SerializableDictionary dictionary) {
        using FileStream stream = File.Create(filePath);
        stream.Write(GetHeaderBytes());

        using GZipStream gZipStream = new(stream, CompressionMode.Compress);
        using BinaryWriter writer = new(gZipStream);
        dictionary.Write(writer);
    }

    public static SerializableDictionary ReadFromFile(string filePath) {
        if (!File.Exists(filePath)) {
            Logger.Error($"Cannot read file at: {filePath}");
            return null;
        }

        using FileStream stream = File.OpenRead(filePath);
        byte[] headerCheck = new byte[GetHeaderBytes().Length];
        stream.ReadExactly(headerCheck);

        string extractedData = Encoding.UTF8.GetString(headerCheck);
        if (extractedData != FILE_HEADER_STRING)
            throw new FileHeaderMissingException(FILE_HEADER_STRING);

        using GZipStream gZipStream = new(stream, CompressionMode.Decompress);
        using BinaryReader reader = new(gZipStream);

        SerializableDictionary dictionary = new();
        dictionary.Read(reader);
        return dictionary;
    }

    public bool ContainsKey(string key) {
        return Data.ContainsKey(key);
    }


    public AbstractEntry this[string key] {
        get {
            return Data[key];
        }
        set {
            Data[key] = value;
        }
    }

    public DictionaryEntryType GetTypeOf(string key) {
        return Data[key]?.GetEntryType() ?? DictionaryEntryType.INVALID;
    }

    public AbstractEntry[] GetValues() {
        return [.. Data.Values];
    }

    #region Put Helpers

    public void Put(string key, AbstractEntry entry) {
        Data[key] = entry.SetKey(key);
    }

    public void Put(string key, byte b) {
        Data[key] = new ByteEntry(b).SetKey(key);
    }

    public void Put(string key, short s) {
        Data[key] = new ShortEntry(s).SetKey(key);
    }

    public void Put(string key, int i) {
        Data[key] = new IntEntry(i).SetKey(key);
    }

    public void Put(string key, long l) {
        Data[key] = new LongEntry(l).SetKey(key);
    }

    public void Put(string key, float f) {
        Data[key] = new FloatEntry(f).SetKey(key);
    }

    public void Put(string key, double d) {
        Data[key] = new DoubleEntry(d).SetKey(key);
    }

    public void Put(string key, string s) {
        Data[key] = new StringEntry(s).SetKey(key);
    }

    public void Put(string key, SerializableDictionary dictionary) {
        Data[key] = dictionary.SetKey(key);
    }

    public void Put(string key, Color color) {
        Data[key] = new ColorEntry(color).SetKey(key);
    }

    public void Put(string key, Vector2 v) {
        Data[key] = new Vector2Entry(v).SetKey(key);
    }

    public void Put(string key, Vector3 v) {
        Data[key] = new Vector3Entry(v).SetKey(key);
    }

    public void Put(string key, bool b) {
        Data[key] = new ByteEntry((byte)(b ? 1 : 0)).SetKey(key);
    }

    public void Put(string key, Rectangle rect) {
        Data[key] = new RectEntry(rect).SetKey(key);
    }

    public void Delete(string key) {
        Data.Remove(key);
    }

    #endregion

    #region Get Helpers

    public byte GetByte(string key, byte def = 0) {
        return !ContainsKey(key) ? def : ((ByteEntry)Data[key]).Data;
    }

    public short GetShort(string key, short def = 0) {
        return !ContainsKey(key) ? def : ((ShortEntry)Data[key]).Data;
    }

    public int GetInt(string key, int def = 0) {
        return !ContainsKey(key) ? def : ((IntEntry)Data[key]).Data;
    }

    public long GetLong(string key, long def = 0) {
        return !ContainsKey(key) ? def : ((LongEntry)Data[key]).Data;
    }

    public float GetFloat(string key, float def = 0) {
        return !ContainsKey(key) ? def : ((FloatEntry)Data[key]).Data;
    }

    public double GetDouble(string key, double def = 0) {
        return !ContainsKey(key) ? def : ((DoubleEntry)Data[key]).Data;
    }

    public string GetString(string key, string def = "") {
        return !ContainsKey(key) ? def : ((StringEntry)Data[key]).Data;
    }

    public ListEntry GetList(string key) {
        return !ContainsKey(key) ? new() : (ListEntry)Data[key];
    }

    public Color GetColor(string key) {
        return !ContainsKey(key) ? default : ((ColorEntry)Data[key]).Data;
    }

    public Vector2 GetVector2(string key) {
        return !ContainsKey(key) ? default : ((Vector2Entry)Data[key]).Data;
    }

    public Vector3 GetVector3(string key) {
        return !ContainsKey(key) ? default : ((Vector3Entry)Data[key]).Data;
    }

    public SerializableDictionary GetSerializableDictionary(string key) {
        return !ContainsKey(key) ? new() : (SerializableDictionary)Data[key];
    }

    public bool GetBool(string key, bool def = false) {
        return !ContainsKey(key) ? def : ((ByteEntry)Data[key]).Data == 1;
    }

    public Rectangle GetRectangle(string key) {
        return !ContainsKey(key) ? default : ((RectEntry)Data[key]).Data;
    }

    #endregion

    public override string ToString() {
        return $"{Count} entries";
    }

    public override void RenderDebugEditor() {
        
    }

}
