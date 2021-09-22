using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class JsonCreator
{
    [MenuItem("Assets/Create/Json", priority = 80)]
    static void Create()
    {
        string[] selectedGUIDs = Selection.assetGUIDs;

        if (selectedGUIDs.Length == 0)  // Nothing is selected.
            return;
        
        string path = AssetDatabase.GUIDToAssetPath(selectedGUIDs[0]);
        File.Create($"{path}/new_json.json");
    }
}
#endif