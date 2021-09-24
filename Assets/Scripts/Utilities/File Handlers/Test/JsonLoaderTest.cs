using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MyBox;

#if UNITY_EDITOR
namespace FF.Json.Test
{
    public class JsonLoaderTest : MonoBehaviour
    {
        [SerializeField] private TextAsset _asset;
        [SerializeField] private string _path;

        [ButtonMethod]
        private void HookAndLoadTextAsset()
        {
            JsonLoader.HookAndLoad(_asset);
            Debug.Assert(JsonLoader.Hooked.ContainsKey(_asset.name));
        }

        [ButtonMethod]
        private void HookAndLoadFromPath()
        {
            JsonLoader.HookAndLoad(_path);
            Debug.Assert(JsonLoader.Hooked.ContainsKey(Path.GetFileNameWithoutExtension(_path)));
        }

        [ButtonMethod]
        private void Load()
        {
            JsonLoader.Load(Path.GetFileNameWithoutExtension(Path.GetFileName(_path)));
            Debug.Assert(JsonLoader.Loaded.ContainsKey(Path.GetFileNameWithoutExtension(Path.GetFileName(_path))));
        }

        [ButtonMethod]
        private void Unload()
        {
            JsonLoader.Unload(Path.GetFileNameWithoutExtension(_path));
            Debug.Assert(!JsonLoader.Loaded.ContainsKey(Path.GetFileNameWithoutExtension(_path)));
        }

        [ButtonMethod]
        private void Hook()
        {
            JsonLoader.Hook(_path);
            Debug.Assert(JsonLoader.Hooked.ContainsKey(Path.GetFileNameWithoutExtension(Path.GetFileName(_path))));
        }

        [ButtonMethod]
        private void UnHook()
        {
            JsonLoader.Unhook(Path.GetFileNameWithoutExtension(_path));
            Debug.Assert(!JsonLoader.Hooked.ContainsKey(Path.GetFileNameWithoutExtension(_path)));
        }

        [ButtonMethod]
        private void ReadCount()
        {
            Debug.Log(JsonLoader.Loaded.Count);
        }

        [ButtonMethod]
        private void Clear()
        {
            JsonLoader.Clear();
            Debug.Assert(JsonLoader.Loaded.Count == 0);
        }

        [ButtonMethod]
        private void ReadLoaded()
        {
            foreach (string value in JsonLoader.Loaded.Values)
                Debug.Log(value);
        }
    }
}
#endif