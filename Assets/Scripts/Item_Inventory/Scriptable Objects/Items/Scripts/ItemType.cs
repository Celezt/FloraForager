using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemType
{
    public string ID { get; set; }
    public string Type { get; set; }
    public string Desciprition { get; set; }
    public int MaxAmount { get; set; }
    public string[] Tag { get; set; }
    public int BasePrice { get; set; }
}
