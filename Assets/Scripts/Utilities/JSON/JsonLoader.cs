using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace FF.Json
{
    public class JsonLoader
    {
        public static IReadOnlyDictionary<string, string> Loaded => _loaded;

        private const string PATH_EXCEPTION = "PathException";

        private static Dictionary<string, string> _loaded = new Dictionary<string, string>();

        public static void Unload(string fileName) => _loaded.Remove(fileName);
        public static void Clear() => _loaded.Clear();
        public static int Count() => _loaded.Count;
        public static bool Contains(string fileName) => _loaded.ContainsKey(fileName);
        public static string Get(string fileName) => _loaded[fileName];

        public static void Load(TextAsset asset)
        {
            _loaded.Add(asset.name, asset.text);
        }

        public static void Load(string path)
        {
            if (!Path.HasExtension(path))
                path = path + ".json";

            if (Path.GetExtension(path) != ".json")
            {
                Debug.LogError($"{PATH_EXCEPTION}:{path} only support .json");
                return;
            }

            string text = File.ReadAllText($"{Application.dataPath}/{path}");

            _loaded.Add(Path.GetFileName(path), text);
        }

        public static void LoadResource(string path)
        {
            if (Path.HasExtension(path))
            {
                Debug.LogError($"{PATH_EXCEPTION}:{path} resource path not allowed to have any extensions");
                return;
            }

            TextAsset asset = Resources.Load<TextAsset>(path);

            if (asset == null)
            {
                Debug.LogError($"{PATH_EXCEPTION}: {path} was not found");
                return;
            }

            Load(asset);
        }
    }
}

