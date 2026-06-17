using Engine.Serialization;
using System.IO;
using System.Numerics;

namespace Engine.Utility.Math;

public static class BigIntegerSerializationExtensions {

    public static void Write(this BinaryWriter writer, BigInteger bigInteger) {
        byte[] bytes = bigInteger.ToByteArray();
        writer.Write(bytes.Length);
        writer.Write(bytes);
    }

    public static BigInteger ReadBigInteger(this BinaryReader reader) {
        int size = reader.ReadInt32();
        return new(reader.ReadBytes(size));
    }

    public static BigInteger GetBigInteger(this SerializableDictionary dictionary, string key) {
        byte[] bytes = dictionary.GetBytes(key);
        if (bytes == null) return default;

        return new(bytes);
    }

    public static void Put(this SerializableDictionary dictionary, string key, BigInteger bigInteger) {
        dictionary.Put(key, bigInteger.ToByteArray());
    }

}
