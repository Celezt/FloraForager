using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace FF.Json
{
    public class JsonCreator
    {
        [MenuItem("Assets/Create/Json", priority = 80)]
        static void Create()
        {
            string[] selectedGUIDs = Selection.assetGUIDs;

            if (selectedGUIDs.Length == 0)  // Nothing is selected.
                return;

            string path = AssetDatabase.GUIDToAssetPath(selectedGUIDs[0]);


            if (File.Exists($"{path}/new_json.json"))
            {
                int index = 1;
                while (true)
                {
                    if (!File.Exists($"{path}/new_json_{index}.json"))
                    {
                        File.WriteAllText($"{path}/new_json_{index}.json", "{\n\n}");
                        break;
                    }
                    else
                    {
                        index++;
                    }
                }
            }
            else
            {
                File.WriteAllText($"{path}/new_json.json", "{\n\n}");
            }

            AssetDatabase.Refresh();
        }
    }
}
#endif