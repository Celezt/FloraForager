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
    }
}
#endif