using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public struct DialogueAsset
{
    public IList<string> Tag { get; set; }
    public string Act { get; set; }
    [JsonProperty(Required = Required.Always)]
    public IList<ParagraphAsset> Dialogue { get; set; }
}
