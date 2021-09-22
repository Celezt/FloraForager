using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

#if UNITY_EDITOR
namespace FF.Json.Test
{
    public class JsonDebug : MonoBehaviour
    {
        [SerializeField] private TextAsset _asset;
        [SerializeField] private string _path;
        [SerializeField] private string _resourceFileName;

        [ButtonMethod]
        private void LoadTextAsset()
        {
            JsonLoader.Load(_asset);
        }

        [ButtonMethod]
        private void LoadFromPath()
        {
            JsonLoader.Load(_path);
        }

        [ButtonMethod]
        private void LoadResource()
        {
            JsonLoader.LoadResource(_resourceFileName);
        }

        [ButtonMethod]
        private void ReadCount()
        {
            Debug.Log(JsonLoader.Loaded.Count);
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