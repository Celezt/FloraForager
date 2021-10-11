using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AddressableAssets;
using System.Text;
using MyBox;
using System.Reflection;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using IngameDebugConsole;

public class DialogueManager : MonoBehaviour
{
    private static Dictionary<int, DialogueManager> _dialogues = new Dictionary<int, DialogueManager>();
    private static bool _initializedTags;

    /// <summary>
    /// Callback when the current dialogue is completed or canceled.
    /// </summary>
    public event Action<DialogueManager> Completed = delegate { };
    /// <summary>
    /// Callback when the current dialogue has started a conversation.
    /// </summary>
    public event Action<DialogueManager> Started = delegate { };

    [SerializeField] private TextMeshProUGUI _content;
    [SerializeField] private TextMeshProUGUI _namecard;
    [SerializeField] private GameObject _dialogueUI;
    [SerializeField] private Transform _buttonParent;
    [SerializeField] private GameObject _buttonType;
    [SerializeField] private AssetLabelReference _aliasLabel;
    [SerializeField] private float _autoTextSpeed = 0.1f;
    [SerializeField, Min(0)] private int _playerIndex;

    private Stack<ParagraphAsset> _paragraphStack;
    private Queue<(string, RangeInt)> _richTagSpeedMultiplierQueue;
    private List<float> _speedMultiplierHierarchy;
    private List<GameObject> _actions;
    private Dictionary<string, string> _aliases = new Dictionary<string, string>();
    private Dictionary<string, Action<Taggable, string>> _tags = new Dictionary<string, Action<Taggable, string>>();

    private Coroutine _autoTypeCoroutine;

    private int _currentLayer;
    private bool _isAutoTextCompleted;

    /// <summary>
    /// Return Dialogue Manager based on the player index connected to it.
    /// </summary>
    /// <param name="playerIndex">Index.</param>
    /// <returns>Dialogue Manager.</returns>
    public static DialogueManager GetByIndex(int playerIndex) => _dialogues[playerIndex];

    public float GetAutoTextSpeedMultiplier(int layer) => _speedMultiplierHierarchy[layer];

    public float SetAutoTextSpeedMultiplier(int layer, float speed)
    {
        while (_speedMultiplierHierarchy.Count <= layer)
            _speedMultiplierHierarchy.Add(float.NaN);

        return _speedMultiplierHierarchy[layer] = speed;
    }

    public DialogueManager StartDialogue(string address, params string[] aliases)
    {
        Addressables.LoadAssetAsync<TextAsset>(address).Completed += (handle) =>
        {
            StartDialogue(handle, aliases);
        };

        return this;
    }

    public DialogueManager StartDialogue(AssetReferenceText assetReference,  params string[] aliases)
    {
        assetReference.LoadAssetAsync<TextAsset>().Completed += (handle) => 
        {
            StartDialogue(handle, aliases);
        };

        return this;
    }

    public void StartDialogue(AsyncOperationHandle<TextAsset> handle, params string[] aliases)
    {
        Started.Invoke(this);
        _dialogueUI.SetActive(true);

        _speedMultiplierHierarchy = new List<float>();
        _currentLayer = 0;

        if (aliases.NotNullOrEmpty())
        {
            for (int i = 0; i < aliases.Length; i++)
            {
                if (_aliases.ContainsKey($"actor_{i}"))
                    _aliases[$"actor_{i}"] = aliases[i];
                else
                    _aliases.Add($"actor_{i}", aliases[i]);
            }

        }

        DialogueAsset asset = JsonConvert.DeserializeObject<DialogueAsset>(handle.Result.text);
        DialogueUtility.Tag(this, _currentLayer, asset.Tag, _tags);
        _paragraphStack = new Stack<ParagraphAsset>(asset.Dialogue.Reverse<ParagraphAsset>());
        _currentLayer++;
        _isAutoTextCompleted = true;
        Next();

        Addressables.Release(handle);
    }

    public void CancelDialogue()
    {
        _dialogueUI.SetActive(false);

        if (_autoTypeCoroutine != null)
            StopCoroutine(_autoTypeCoroutine);

        Completed.Invoke(this);
    }

    public void Next()
    {
        void DestroyActions()
        {
            if (_actions.NotNullOrEmpty())
            {
                for (int i = 0; i < _actions.Count; i++)
                    Destroy(_actions[i]);

                _actions = null;
            }
        }

        void CreateActions(ParagraphAsset paragraph)
        {
            if (paragraph.Action.IsNullOrEmpty())
                return;

            _actions = new List<GameObject>();
            _currentLayer++;

            for (int i = 0; i < paragraph.Action.Count; i++)
            {
                int index = i;
                GameObject obj = Instantiate(_buttonType, _buttonParent);
                Button button = obj.GetComponentInChildren<Button>();
                TextMeshProUGUI textMesh = obj.GetComponentInChildren<TextMeshProUGUI>();

                StringBuilder act = new StringBuilder(paragraph.Action[i].Act);
                DialogueUtility.Alias(act, _aliases);

                textMesh.text = act.ToString();
                button.onClick.AddListener(() =>
                {
                    _paragraphStack = new Stack<ParagraphAsset>(paragraph.Action[index].Dialogue.Reverse<ParagraphAsset>());
                    DialogueUtility.Tag(this, _currentLayer, paragraph.Action[index].Tag, _tags);
                    _currentLayer++;
                    _isAutoTextCompleted = true;
                    Next();
                });

                _actions.Add(obj);
            }
        }

        if (!_isAutoTextCompleted)   // Skip auto text if not yet completed.
        {
            if (_autoTypeCoroutine != null)
                StopCoroutine(_autoTypeCoroutine);

            _isAutoTextCompleted = true;
            _content.maxVisibleCharacters = int.MaxValue;
            return;
        }

        if (_paragraphStack.IsNullOrEmpty())
        {
            if (_actions.IsNullOrEmpty())
                CancelDialogue();

            return;
        }

        DestroyActions();

        ParagraphAsset paragraph = _paragraphStack.Pop();
        StringBuilder content = new StringBuilder(paragraph.Text.ToString());

        DialogueUtility.Alias(content, _aliases);
        DialogueUtility.Tag(this, _currentLayer, paragraph.Tag, _tags);
        DialogueUtility.ExtractCustomRichTag(content, "speed", out _richTagSpeedMultiplierQueue);

        _content.text = content.ToString();

        if (_aliases.TryGetValue(paragraph.ID, out string alias))
            _namecard.text = alias;
        else
            _namecard.text = paragraph.ID;

        if (_autoTypeCoroutine != null)
            StopCoroutine(_autoTypeCoroutine);

        _autoTypeCoroutine = StartCoroutine(AutoType(paragraph));

        CreateActions(paragraph);
    }

    private void Awake()
    {
        DialogueUtility.InitializeAllTags(this, _currentLayer, _tags);
    }

    private void Start()
    {
        _dialogues.Add(_playerIndex, this);

        DebugLogConsole.AddCommandInstance("dialogue_cancel", "Cancel current dialogue", nameof(CancelDialogue), this);
        DebugLogConsole.AddCommandInstance("dialogue_start", "Start dialogue", nameof(StartDialogueConsole), this);
        DebugLogConsole.AddCommandInstance("dialogue_speed", "Sets auto text speed", nameof(SetAutoTextSpeedConsole), this);

        _dialogueUI.SetActive(false);

        if (!_initializedTags)  // Prevent loading multiple times
        {
            DialogueUtility.LoadAliases(_aliasLabel, _aliases);
            _initializedTags = true;
        }
    }

    private void OnDestroy()
    {
        _dialogues.Remove(_playerIndex);
    }

    private void StartDialogueConsole(string address, params string[] aliases) => StartDialogue(address, aliases);
    private void SetAutoTextSpeedConsole(float speed) => _autoTextSpeed = speed;

    private IEnumerator AutoType(ParagraphAsset paragraph)
    {
        RangeInt indexRange = new RangeInt();
        float richTagSpeedMultiplier = 1;
        _isAutoTextCompleted = false;

        IEnumerator Dequeue()
        {
            (string, RangeInt) customRichTag = _richTagSpeedMultiplierQueue.Dequeue();
            indexRange = customRichTag.Item2;
            if (!float.TryParse(customRichTag.Item1, out richTagSpeedMultiplier))
            {
                Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: {customRichTag.Item1} could not be parsed to float");
                yield break;
            }
        }

        if (_richTagSpeedMultiplierQueue.Count > 0)
            yield return Dequeue();

        _content.ForceMeshUpdate();
        int maxCount = _content.textInfo.characterCount + 1;
        for (int count = 0; count < maxCount; count++)
        {
            _content.maxVisibleCharacters = count;

            while (true)
            {
                if (count >= indexRange.start && count < indexRange.end)
                {
                    yield return new WaitForSeconds(_autoTextSpeed / richTagSpeedMultiplier);
                    break;
                }
                else if (_richTagSpeedMultiplierQueue.Count > 0)
                {
                    yield return Dequeue();
                }
                else
                {
                    float speedMultiplier = 0;
                    if (paragraph.Tag != null && paragraph.Tag.Contains("speed"))
                        speedMultiplier = _speedMultiplierHierarchy.Last();
                    else
                    {
                        int hierarchyCount = _speedMultiplierHierarchy.Count;
                        for (int i = 1; i < hierarchyCount; i++)
                        {
                            speedMultiplier = _speedMultiplierHierarchy[hierarchyCount - i];

                            if (speedMultiplier != float.NaN)
                                break;
                        }
                    }
                       
                    yield return new WaitForSeconds(_autoTextSpeed / speedMultiplier);
                    break;
                }
            }
        }

        _isAutoTextCompleted = true;
    }
}
