using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class ItemDatabase 
{
    static private Dictionary<string, ItemType> database = new Dictionary<string, ItemType>();
    static public IReadOnlyDictionary<string, ItemType> Database => database;
}
