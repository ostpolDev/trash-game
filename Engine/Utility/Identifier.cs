namespace Engine.Utility;

public struct Identifier(string id_namespace, string key) {

    public static readonly Identifier EMPTY = new("common", "empty");

    public string Namespace = id_namespace ?? throw new System.ArgumentNullException(nameof(id_namespace));
    public string Key = key ?? throw new System.ArgumentNullException(nameof(key));

    public readonly bool IsEmpty { get { return (Namespace == EMPTY.Namespace && Key == EMPTY.Key) || string.IsNullOrEmpty(Namespace) || string.IsNullOrEmpty(Key); } }

    public override readonly string ToString() {
        return $"{Namespace}:{Key}";
    }

}
