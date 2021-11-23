using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Sirenix.OdinInspector;
using MyBox;

public class Intro : MonoBehaviour
{
    [SerializeField, Scene]
    private string _SceneToLoad;
    [SerializeField]
    private float _FadeInTime = 3.0f;
    [SerializeField]
    private float _FadeOutTime = 3.0f;
    [SerializeField]
    private AssetReferenceText _Text;
    [SerializeField, ListDrawerSettings(Expanded = true)]
    private string[] _Aliases;

    private void Start()
    {
        FadeScreen.Instance.StartFadeOut(_FadeOutTime);
        FadeScreen.Instance.OnEndFade += Invoke;
    }

    private void Invoke()
    {
        if (string.IsNullOrEmpty(_Text.AssetGUID))
        {
            LoadLevel();
            return;
        }    

        DialogueManager.GetByIndex(0).StartDialogue(_Text, _Aliases).Completed += CompleteAction;

        void CompleteAction(DialogueManager manager)
        {
            FadeScreen.Instance.StartFadeIn(_FadeInTime);
            FadeScreen.Instance.OnEndFade += LoadLevel;

            manager.Completed -= CompleteAction;
        };

        FadeScreen.Instance.OnEndFade -= Invoke;
    }

    private void LoadLevel()
    {
        LoadScene.Instance.LoadSceneByName(_SceneToLoad);
        FadeScreen.Instance.OnEndFade -= LoadLevel;
    }
}
