using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public struct ParagraphAsset
{
    public string ID;
    public string Text;
    public List<string> Tag;
    public List<DialogueAsset> Action;
}
