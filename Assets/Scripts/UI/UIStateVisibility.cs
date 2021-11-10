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

    private Dictionary<string, GameObject> _StatesDictionary = new Dictionary<string, GameObject>();

    private void Start()
    {
        _StatesDictionary = _States.ToDictionary(s => s.Key, s => s.GameObject);

        _States.Clear();
        _States = null;

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
            if (!_StatesDictionary.TryGetValue(state, out GameObject value))
                continue;

            value.SetActive(true);
        }
    }
    public void Hide(params string[] hideStates)
    {
        foreach (string state in hideStates)
        {
            if (!_StatesDictionary.TryGetValue(state, out GameObject value))
                continue;

            value.SetActive(false);
        }
    }
    public void HideAll()
    {
        foreach (KeyValuePair<string, GameObject> item in _StatesDictionary)
        {
            Hide(item.Key);
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

    public string[] GetShownStates()
    {
        List<string> states = new List<string>();
        foreach (KeyValuePair<string, GameObject> item in _StatesDictionary)
        {
            if (!_StatesDictionary.TryGetValue(item.Key, out GameObject value))
                continue;

            if (value.activeSelf)
                states.Add(item.Key);
        }
        return states.ToArray();
    }
    public string[] GetHiddenStates()
    {
        List<string> states = new List<string>();
        foreach (KeyValuePair<string, GameObject> item in _StatesDictionary)
        {
            if (!_StatesDictionary.TryGetValue(item.Key, out GameObject value))
                continue;

            if (!value.activeSelf)
                states.Add(item.Key);
        }
        return states.ToArray();
    }

    public GameObject Get(string key)
    {
        if (!_StatesDictionary.ContainsKey(key))
            return null;

        return _StatesDictionary[key];
    }

    [System.Serializable]
    private class UIState
    {
        [HorizontalGroup("Group")]
        [VerticalGroup("Group/Left"), LabelWidth(30)]
        public string Key;
        [VerticalGroup("Group/Right"), LabelWidth(80)]
        public GameObject GameObject;
    }
}
