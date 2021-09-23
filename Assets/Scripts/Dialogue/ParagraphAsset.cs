using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public struct ParagraphAsset
{
    public string ID { get; set; }
    public string Text { get; set; }
    public IList<string> Tag { get; set; }
    public IList<DialogueAsset> Action { get; set; }
}
