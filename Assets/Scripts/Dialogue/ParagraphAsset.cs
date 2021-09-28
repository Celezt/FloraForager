using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public struct ParagraphAsset
{
    [JsonProperty(Required = Required.Always)]
    public string ID { get; set; }
    [JsonProperty(Required = Required.Always)]
    public string Text { get; set; }
    public List<string> Tag { get; set; }
    public List<DialogueAsset> Action { get; set; }
}
