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

using static DialogueUtility;

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
    /// <summary>
    /// Base text read speed.
    /// </summary>
    public float AutoTextSpeed => _autoTextSpeed;

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
    private List<GameObject> _actions;
    private Dictionary<string, string> _aliases = new Dictionary<string, string>();
    private Dictionary<string, ITag> _tags = new Dictionary<string, ITag>();
    private Dictionary<string, IHierarchyTag> _hierarchyTags = new Dictionary<string, IHierarchyTag>();
    private Dictionary<string, RichTagData> _richTags = new Dictionary<string, RichTagData>();
    private Dictionary<string, GameObject> _actors;

    private Coroutine _autoTypeCoroutine;

    private int _currentLayer;
    private int _currentTextIndex;
    private int _currentTextMaxLength;
    private float _pitch;
    private bool _isAutoTextCompleted;
    private bool _audible;

    private Node _rootNode;
    private Node _currentNode;

    public struct RichTagData
    {
        public IRichTag Execution;
        public Queue<(string, RangeInt)> Sequences;
    }

    public struct CurrentRichTagData
    {
        public IRichTag Execution;
        public (string, RangeInt) Sequence;
        public bool HasExecuted;
    }

    /// <summary>
    /// Return Dialogue Manager based on the player index connected to it.
    /// </summary>
    /// <param name="playerIndex">Index.</param>
    /// <returns>Dialogue Manager.</returns>
    public static DialogueManager GetByIndex(int playerIndex) => _dialogues[playerIndex];

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

        _currentLayer = 0;

        _audible = true;
        _pitch = 1.0f;

        _tags.ForEach(x => x.Value.OnActive(Taggable.CreatePackage(this, _currentLayer)));
        _richTags.ForEach(x => x.Value.Execution.OnActive(Taggable.CreatePackage(this, _currentLayer)));

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

        // Create root node.
        _rootNode = new Node(null, _currentLayer, DeserializeTags(asset.Tag, _tags));
        _rootNode.NodeBehaviour.ForEach(x => x.Type.EnterTag(Taggable.CreatePackage(this, _rootNode.Layer), x.Parameter));
        _currentNode = _rootNode;

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
                    _currentLayer++;

                    _paragraphStack = new Stack<ParagraphAsset>(paragraph.Action[index].Dialogue.Reverse<ParagraphAsset>());

                    _currentNode.NodeBehaviour.ForEach(x => x.Type.ExitTag(Taggable.CreatePackage(this, _currentNode.Layer), x.Parameter));

                    // Create node.
                    Node node = new Node(_rootNode, _currentLayer, DeserializeTags(paragraph.Tag, _tags));
                    node.NodeBehaviour.ForEach(x => x.Type.EnterTag(Taggable.CreatePackage(this, node.Layer), x.Parameter));
                    _currentNode = node;

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
            if (_actions.IsNullOrEmpty())   // Cancel dialogue.
            {
                _currentNode.NodeBehaviour.ForEach(x => x.Type.ExitTag(Taggable.CreatePackage(this, _currentNode.Layer), x.Parameter));

                CancelDialogue();
            }

            return;
        }

        _currentNode.NodeBehaviour.ForEach(x => x.Type.ExitTag(Taggable.CreatePackage(this, _currentNode.Layer), x.Parameter));

        DestroyActions();

        ParagraphAsset paragraph = _paragraphStack.Pop();
        StringBuilder content = new StringBuilder(paragraph.Text ?? "");

        DialogueUtility.Alias(content, _aliases);

        // Create node.
        Node node = new Node(_rootNode, _currentLayer, DeserializeTags(paragraph.Tag, _tags));
        node.NodeBehaviour.ForEach(x => x.Type.EnterTag(Taggable.CreatePackage(this, node.Layer), x.Parameter));
        _currentNode = node;

        for (int i = 0; i < _richTags.Count; i++)
        {
            KeyValuePair<string, RichTagData> richTag = _richTags.ElementAt(i);
            RichTagData data = richTag.Value;
            DialogueUtility.DeserializeRichTags(content, richTag.Key, out data.Sequences);
            _richTags[richTag.Key] = data;
        }

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
        _dialogues.Add(_playerIndex, this);

        DialogueUtility.InitializeAllTags(this, _tags, _hierarchyTags, _richTags);
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

    #region Console Commands
    private void StartDialogueConsole(string address, params string[] aliases) => StartDialogue(address, aliases);
    private void SetAutoTextSpeedConsole(float speed) => _autoTextSpeed = speed;
    #endregion

    private IEnumerator AutoType(ParagraphAsset paragraph)
    {
        _isAutoTextCompleted = false;

        List<CurrentRichTagData> richTagsCurrent = new List<CurrentRichTagData>();
        List<RichTagData> richTagsUsed = new List<RichTagData>();
        foreach (RichTagData data in _richTags.Values)  // Look for all rich tags used in this paragraph.
            if (data.Sequences.Count > 0)
            {
                (string, RangeInt) tag = data.Sequences.Dequeue();
                richTagsCurrent.Add(new CurrentRichTagData { Execution = data.Execution, Sequence = tag });
                richTagsUsed.Add(data);
            }

        _content.ForceMeshUpdate();

        string parsedText = _content.GetParsedText().ToUpper();

        int maxLength = _currentTextMaxLength = _content.textInfo.characterCount + 1;
        for (int currentIndex = 0; currentIndex < maxLength; currentIndex++)  
        {
            object yieldValue = null;
            _currentTextIndex = currentIndex;

            _content.maxVisibleCharacters = currentIndex;   // How many letters that should be visible.

            for (int i = 0; i < _currentNode.NodeBehaviour.Length; i++)   // Loop through all current tags.
            {
                IEnumerator enumerator = _currentNode.NodeBehaviour[i].Type.ProcessTag(Taggable.CreatePackage(this, _currentLayer), _currentTextIndex, _currentTextMaxLength, _currentNode.NodeBehaviour[i].Parameter);
                enumerator.MoveNext();
                object returnValue = enumerator.Current;

                if (returnValue != null)  // Don't Return if the IEnumerator returns null.
                    yieldValue = returnValue;
            }

            //if (_audible && currentIndex > 0)
            //{
            //    string letter = parsedText[currentIndex - 1].ToString();

            //    if (SoundPlayer.Instance.TryGetSound(letter, out SoundPlayer.Sound sound))
            //        SoundPlayer.Instance.Play(letter, 0, _pitch - sound.Pitch);
            //}

            for (int i = richTagsUsed.Count - 1; i >= 0; i--)   // Loop through all current rich tags.
            {
                CurrentRichTagData data = richTagsCurrent[i];
                
                if (currentIndex >= data.Sequence.Item2.start && currentIndex < data.Sequence.Item2.end) // Execute in range.
                {
                    if (!data.HasExecuted)  // Execute at the start of the process.
                    {
                        data.HasExecuted = true;
                        data.Execution.EnterTag(Taggable.CreatePackage(this, _currentLayer), currentIndex, data.Sequence.Item2, data.Sequence.Item1);
                    }

                    richTagsCurrent[i] = data;

                    IEnumerator enumerator = data.Execution.ProcessTag(Taggable.CreatePackage(this, _currentLayer), currentIndex, data.Sequence.Item2, data.Sequence.Item1);
                    enumerator.MoveNext();
                    object returnValue = enumerator.Current;
 
                    if (returnValue != null)  // Don't Return if the IEnumerator returns null.
                        yieldValue = returnValue;
                }
                else if (data.HasExecuted)  // If executed and no longer in range.
                {
                    data.Execution.ExitTag(Taggable.CreatePackage(this, _currentLayer), currentIndex, data.Sequence.Item2, data.Sequence.Item1);

                    if (richTagsUsed[i].Sequences.Count > 0)    // Try get next rich tag from this type.
                    {
                        richTagsCurrent[i] = new CurrentRichTagData { Execution = richTagsUsed[i].Execution, Sequence = richTagsUsed[i].Sequences.Dequeue() };
                    }
                    else    // Remove if no more rich tags of this type is found.
                    {
                        richTagsUsed.RemoveAt(i);
                        richTagsCurrent.RemoveAt(i);
                    }
                }
            }

            if (yieldValue != null) // Don't yield if value is null.
                yield return yieldValue;
        }

        _isAutoTextCompleted = true;
    }
}
