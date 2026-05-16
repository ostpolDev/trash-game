namespace Engine.Utility;

public struct Identifier(string id_namespace, string key) {

    public string Namespace = id_namespace ?? throw new System.ArgumentNullException(nameof(id_namespace));
    public string Key = key ?? throw new System.ArgumentNullException(nameof(key));

    public readonly bool IsEmpty { get { return Namespace == null || Key == null; } }

    public override readonly string ToString() {
        return $"{Namespace}:{Key}";
    }

}
