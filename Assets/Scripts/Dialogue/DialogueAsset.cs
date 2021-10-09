using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public struct DialogueAsset
{
    public List<string> Tag;
    public string Act;
    [JsonProperty(Required = Required.Always)]
    public List<ParagraphAsset> Dialogue;
}
