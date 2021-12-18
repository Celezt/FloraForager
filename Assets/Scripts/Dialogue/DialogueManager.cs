using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

    public IReadOnlyDictionary<string, string> Aliases => _aliases;

    [SerializeField] private TextMeshProUGUI _content;
    [SerializeField] private TextMeshProUGUI _namecard;
    [SerializeField] private GameObject _namecardUI;
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

    private bool _audible;
    private float _pitch;

    private Dictionary<string, GameObject> _actors;

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

    public void SetAudible(bool value)
    {
        _audible = value;
    }
    public void SetPitch(float pitch)
    {
        _pitch = pitch;
    }

    public void SetActors(Dictionary<string, GameObject> actors)
    {
        _actors = actors;
    }
    public IReadOnlyDictionary<string, GameObject> GetActors() => _actors;

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

        _speedMultiplierHierarchy = new List<float>() { 1 };    // Speed 1 at layer 0 by default.
        _currentLayer = 0;

        _audible = true;
        _pitch = 1.0f;

        _actors = new Dictionary<string, GameObject>();

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

        foreach (KeyValuePair<string, GameObject> item in _actors)
        {
            HumanoidAnimationBehaviour animationBehaviour;
            if ((animationBehaviour = item.Value.GetComponentInChildren<HumanoidAnimationBehaviour>()) != null)
                animationBehaviour.BlendCancelCustomMotion();
        }

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

                StringBuilder act = new StringBuilder(paragraph.Action[i].Act ?? "");
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
        StringBuilder content = new StringBuilder(paragraph.Text ?? "");

        DialogueUtility.Alias(content, _aliases);
        DialogueUtility.Tag(this, _currentLayer, paragraph.Tag, _tags);
        DialogueUtility.ExtractCustomRichTag(content, "speed", out _richTagSpeedMultiplierQueue);

        _content.text = content.ToString();

        if (string.IsNullOrWhiteSpace(paragraph.ID))     // Hide namecard if no id exist.
            _namecardUI.SetActive(false);
        else
        {
            _namecardUI.SetActive(true);

            if (_aliases.TryGetValue(paragraph.ID, out string alias))
                _namecard.text = alias;
            else
                _namecard.text = paragraph.ID;
        }

        if (_autoTypeCoroutine != null)
            StopCoroutine(_autoTypeCoroutine);

        _autoTypeCoroutine = StartCoroutine(AutoType(paragraph));

        CreateActions(paragraph);
    }

    private void Awake()
    {
        DialogueUtility.InitializeAllTags(this, _currentLayer, _tags);
        _dialogues.Add(_playerIndex, this);
    }

    private void Start()
    {
        DebugLogConsole.AddCommandInstance("dialogue.cancel", "Cancel current dialogue", nameof(CancelDialogue), this);
        DebugLogConsole.AddCommandInstance("dialogue.start", "Start dialogue", nameof(StartDialogueConsole), this);
        DebugLogConsole.AddCommandInstance("dialogue.speed", "Sets auto text speed", nameof(SetAutoTextSpeedConsole), this);

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
            if (!float.TryParse(customRichTag.Item1, NumberStyles.Float, CultureInfo.InvariantCulture, out richTagSpeedMultiplier))
            {
                Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: {customRichTag.Item1} could not be parsed to float");
                yield break;
            }
        }

        if (_richTagSpeedMultiplierQueue.Count > 0)
            yield return Dequeue();

        _content.ForceMeshUpdate();

        string parsedText = _content.GetParsedText().ToUpper();

        bool speedTagExist = false;
        if (paragraph.Tag != null)
        {
            for (int i = 0; i < paragraph.Tag.Count; i++)   // Look for any speed tag.
                if (Regex.Match(paragraph.Tag[0], @"^.*?(?=\{)").Value == "speed")
                    speedTagExist = true;
        }

        int startSearchSpeedTag = _currentLayer > _speedMultiplierHierarchy.Count - 1 ? _speedMultiplierHierarchy.Count - 1 : _speedMultiplierHierarchy.Count - 2; // Use the last speed if current is ahead of the hierarchy.

        int maxCount = _content.textInfo.characterCount + 1;
        for (int count = 0; count < maxCount; count++)
        {
            _content.maxVisibleCharacters = count;

            if (_audible && count > 0)
            {
                string letter = parsedText[count - 1].ToString();

                if (SoundPlayer.Instance.TryGetSound(letter, out SoundPlayer.Sound sound))
                    SoundPlayer.Instance.Play(letter, 0, _pitch - sound.Pitch);
            }

            while (true)
            {
                if (count >= indexRange.start && count < indexRange.end)    // Always prioritize rich tags.
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
                    float speedMultiplier = 1;

                    if (speedTagExist)  // If speed tag exist.
                        speedMultiplier = _speedMultiplierHierarchy[_currentLayer];
                    else
                    {
                        for (int i = startSearchSpeedTag; i >= 0; i--) // Search for existing speed tag.
                        {
                            speedMultiplier = _speedMultiplierHierarchy[i];

                            if (speedMultiplier != float.NaN)   // Stop searching if layer contains speed tag.
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
