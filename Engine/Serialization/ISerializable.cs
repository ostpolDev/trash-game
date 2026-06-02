namespace Engine.Serialization;

public interface ISerializable {

    public void ReadData(SerializableDictionary dictionary);
    public void WriteData(SerializableDictionary dictionary);

}
