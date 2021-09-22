using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

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

        }

        public static void Load(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}

