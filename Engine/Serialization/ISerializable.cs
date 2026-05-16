namespace Engine.Serialization;

public interface ISerializable {

    public void LoadData(SerializableDictionary dictionary);
    public void WriteData(SerializableDictionary dictionary);

}
