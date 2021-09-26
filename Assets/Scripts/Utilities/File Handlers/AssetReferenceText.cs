using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.AddressableAssets
{
    [System.Serializable]
    public class AssetReferenceText : AssetReferenceT<TextAsset>
    {
        public AssetReferenceText(string guid) : base(guid) { }
    }
}
