using UnityEngine.AddressableAssets;

[System.Serializable]
public struct DialoguePriority
{
    public float Priority;
    public AssetReferenceText Dialogue;
    public string[] Aliases;
}

[System.Serializable]
public struct DialogueElement
{
    public AssetReferenceText Dialogue;
    public string[] Aliases;
}