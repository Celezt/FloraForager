using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AddressableAssets;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _content;
    [SerializeField] private TextMeshProUGUI _namecard;

    private void Start()
    {
        Addressables.LoadAssetAsync<TextAsset>("knock_knock_test_en").Completed += (handle) =>
        {
            Debug.Log(handle.Result.text);
            Addressables.Release(handle);
        };
    }

    public void LoadHooks()
    {
    }
}
