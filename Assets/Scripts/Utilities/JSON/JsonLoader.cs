using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace FF.Json
{
    public class JsonLoader
    {
        private static Dictionary<string, string> _loaded = new Dictionary<string, string>()
        {
            {"sd", $"{Application.dataPath}/Resources/Dialogues/Event1.json"}
        };

        public static void Load(TextAsset asset)
        {
            Debug.Log(asset.text);
        }

        public static void Load(string path)
        {
            string text = File.ReadAllText($"{Application.dataPath}/{path}");
            Debug.Log(text);
        }
    }
}

