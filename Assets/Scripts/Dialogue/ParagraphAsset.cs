using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public struct ParagraphAsset
{
    [JsonProperty(Required = Required.Always)]
    public string ID;
    [JsonProperty(Required = Required.Always)]
    public string Text;
    public List<string> Tag;
    public List<DialogueAsset> Action;
}
