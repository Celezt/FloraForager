using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using MyBox;

public class UIStateVisibility : Singleton<UIStateVisibility>
{
    [SerializeField, ListDrawerSettings(Expanded = true)]
    private List<UIState> _States = new List<UIState>();

    private Dictionary<string, CanvasGroup> _StatesDictionary = new Dictionary<string, CanvasGroup>();

    private void Start()
    {
        _StatesDictionary = _States.ToDictionary(s => s.Key, s => s.Canvas);

        DialogueManager.GetByIndex(0).Started += (DialogueManager manager) => 
        {
            Show("dialogue");
            Hide("player_hud", "inventory");
        };
        DialogueManager.GetByIndex(0).Completed += (DialogueManager manager) =>
        {
            Show("player_hud", "inventory");
            Hide("dialogue");
        };
    }

    public void Show(params string[] showStates)
    {
        foreach (string state in showStates)
        {
            if (!_StatesDictionary.TryGetValue(state, out CanvasGroup value))
                continue;

            value.alpha = 1.0f;
            value.blocksRaycasts = true;
        }
    }
    public void Hide(params string[] hideStates)
    {
        foreach (string state in hideStates)
        {
            if (!_StatesDictionary.TryGetValue(state, out CanvasGroup value))
                continue;

            value.alpha = 0.0f;
            value.blocksRaycasts = false;
        }
    }
    public void HideAll()
    {
        foreach (KeyValuePair<string, CanvasGroup> pair in _StatesDictionary)
        {
            Hide(pair.Key);
        }
    }

    public void ShowAndHide(string showState, params string[] hideStates)
    {
        Hide(hideStates);
        Show(showState);
    }

    public void ShowAndHideAll(string showState)
    {
        HideAll();
        Show(showState);
    }

    [System.Serializable]
    private class UIState
    {
        [HorizontalGroup("Group")]
        [VerticalGroup("Group/Left"), LabelWidth(30)]
        public string Key;
        [VerticalGroup("Group/Right"), LabelWidth(50)]
        public CanvasGroup Canvas;
    }
}
