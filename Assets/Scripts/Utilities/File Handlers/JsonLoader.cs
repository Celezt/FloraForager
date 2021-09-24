using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace FF.Json
{
    public static class JsonLoader
    {
        public static IReadOnlyDictionary<string, string> Loaded => _loaded;
        public static IReadOnlyDictionary<string, string> Hooked => _hooked;

        private const string JSON_LOADER_EXCEPTION = "JsonLoaderException";

        private static Dictionary<string, string> _loaded = new Dictionary<string, string>();
        private static Dictionary<string, string> _hooked = new Dictionary<string, string>();

        public static bool Unload(string fileName) => _loaded.Remove(fileName);
        public static string GetContent(string fileName) => _loaded[fileName];
        public static void Clear()
        {
            _loaded.Clear();
            _hooked.Clear();
        }

        /// <summary>
        /// Unhook and unload JSON-file.
        /// </summary>
        /// <param name="fileName">Key.</param>
        /// <returns>If existed.</returns>
        public static bool Unhook(string fileName)
        {
            bool exist = _hooked.Remove(fileName);
            _loaded.Remove(fileName);

            return exist;
        }

        /// <summary>
        /// Hook up a JSON-file without loading it into memory.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <returns>If not already exist.</returns>
        public static bool Hook(string path)
        {
            if (!Path.HasExtension(path))
                path = path + ".json";

            if (Path.GetExtension(path) != ".json")
            {
                throw new JsonLoaderException($"{path} only support .json");
            }

            if (!File.Exists($"{Application.dataPath}/{path}"))
            {
                throw new JsonLoaderException($"Could not find {path}");
            }

            string fileName = Path.GetFileNameWithoutExtension(path);
            
            if (_hooked.ContainsKey(fileName))  // If already exist.
                return false;

            _hooked.Add(fileName, path);

            return true;
        }

        /// <summary>
        /// Load hooked JSON-file.
        /// </summary>
        /// <param name="fileName">Key.</param>
        /// <returns>If successful.</returns>
        public static bool Load(string fileName)
        {
            if (!_hooked.ContainsKey(fileName))
            {
                Debug.LogError($"{JSON_LOADER_EXCEPTION}: {fileName} has not been hooked");
                return false;
            }

            string path = _hooked[fileName];

            if (path == "")
            {
                Debug.LogError($"{JSON_LOADER_EXCEPTION}: {fileName} cannot load TextAsset type. TextAsset will always be loaded");
                return false;
            }

            _loaded.Add(fileName, File.ReadAllText($"{Application.dataPath}/{path}"));

            return true;
        }

        /// <summary>
        /// Hook and load a <c>TextAsset</c> JSON-file.
        /// </summary>
        /// <param name="asset">TextAsset.</param>
        /// <returns>If successful.</returns>
        public static bool HookAndLoad(TextAsset asset)
        {
            string fileName = asset.name;

            if (_loaded.ContainsKey(fileName))
            {
                Debug.LogError($"{JSON_LOADER_EXCEPTION}: {fileName} already loaded");
                return false;
            }

            _hooked.Add(fileName, "");
            _loaded.Add(fileName, asset.text);

            return true;
        }

        /// <summary>
        /// Hook and Load JSON-file.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <returns>If successful.</returns>
        public static bool HookAndLoad(string path)
        {
            if (!Path.HasExtension(path))
                path = path + ".json";

            if (!Hook(path))
                return false;

            string text = File.ReadAllText($"{Application.dataPath}/{path}");

            _loaded.Add(Path.GetFileNameWithoutExtension(path), text);

            return true;
        }

        private class JsonLoaderException : System.Exception
        {
            public JsonLoaderException() { }
            public JsonLoaderException(string message) : base(message) { }
            public JsonLoaderException(string message, System.Exception inner) : base(message, inner) { }
        }
    }
}

