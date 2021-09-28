using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemObject : MonoBehaviour
{
    public GameObject prefabOverworld;
    public GameObject prefabInventorySlot;
    public int MaxAmount;
    public int ID;
    public int buy;
    public int sell;
    public List<string> types;
    [TextArea(15, 20)]
    public string description;
}
