using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueAsset
{
    public string Act { get; set; }
    public IList<ParagraphAsset> Dialogues { get; set; }
}
