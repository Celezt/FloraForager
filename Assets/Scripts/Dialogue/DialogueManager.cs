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
    public IReadOnlyDictionary<string, ITag> TagTypes => _tagTypes;
    public IReadOnlyDictionary<string, IHierarchyTag> HierarchyTagTypes => _hierarchyTagTypes;
    public IReadOnlyDictionary<string, RichTagData> RichTagTypes => _richTagTypes;
    public DialogueUtility.Tree Tree => _tree;
    public Node RootNode => _rootNode;
    public Node CurrentNode => _currentNode;
    public Node CurrentParent => _currentParent;
    public int CurrentLayer => _currentLayer;
    public int CurrentTextIndex => _currentTextIndex;
    public int CurrentTextMaxLength => _currentTextMaxLength;
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
    private Dictionary<string, ITag> _tagTypes = new Dictionary<string, ITag>();
    private Dictionary<string, IHierarchyTag> _hierarchyTagTypes = new Dictionary<string, IHierarchyTag>();
    private Dictionary<string, RichTagData> _richTagTypes = new Dictionary<string, RichTagData>();
    private Dictionary<string, GameObject> _actors;

    private DialogueUtility.Tree _tree;
    private Node _rootNode;
    private Node _currentNode;
    private Node _currentParent;
    private Coroutine _autoTypeCoroutine;

    private int _currentLayer;
    private int _currentTextIndex;
    private int _currentTextMaxLength;
    private bool _isAutoTextCompleted;

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

        _tagTypes.ForEach(x => x.Value.OnActive(Taggable.CreatePackage(this, _currentLayer)));
        _richTagTypes.ForEach(x => x.Value.Execution.OnActive(Taggable.CreatePackage(this, _currentLayer)));

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

        // Create Tree.
        _tree = new DialogueUtility.Tree();

        // Create root node.
        _rootNode = new Node(null, _currentLayer, DeserializeTags(asset.Tag, _tagTypes));
        _rootNode.Tags.ForEach(x => x.TagBehaviour.EnterTag(Taggable.CreatePackage(this, _rootNode.Layer), x.Parameter));
        _tree.Add(_currentLayer, _rootNode);
        _currentNode = _rootNode;
        _currentParent = _rootNode;

        _paragraphStack = new Stack<ParagraphAsset>(asset.Dialogue.Reverse<ParagraphAsset>());
        _currentLayer++;
        _isAutoTextCompleted = true;
        Next();

        Addressables.Release(handle);
    }

    public void CancelDialogue()
    {
        _currentNode.Tags.ForEach(x => x.TagBehaviour.ExitTag(Taggable.CreatePackage(this, _currentNode.Layer), x.Parameter));

        FindHierarchyNodes(_hierarchyTagTypes, _currentNode.Parent, (hierarchy, parent, tag) =>
            hierarchy.ExitChild(Taggable.CreatePackage(this, parent.Layer), Taggable.CreatePackage(this, _currentNode.Layer), tag.Parameter));

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
                    _currentNode.Tags.ForEach(x => x.TagBehaviour.ExitTag(Taggable.CreatePackage(this, _currentNode.Layer), x.Parameter));

                    FindHierarchyNodes(_hierarchyTagTypes, _currentNode.Parent, (hierarchy, parent, tag) =>
                        hierarchy.ExitChild(Taggable.CreatePackage(this, parent.Layer), Taggable.CreatePackage(this, _currentNode.Layer), tag.Parameter));

                    _currentParent = _currentNode;
                    _currentLayer++;

                    _paragraphStack = new Stack<ParagraphAsset>(paragraph.Action[index].Dialogue.Reverse<ParagraphAsset>());

                    // Create node.
                    Node node = new Node(_currentParent, _currentLayer, DeserializeTags(paragraph.Action[index].Tag, _tagTypes));
                    node.Tags.ForEach(x => x.TagBehaviour.EnterTag(Taggable.CreatePackage(this, node.Layer), x.Parameter));
                    _tree.Add(_currentLayer, node);
                    _currentNode = node;
                    _currentParent = node;

                    FindHierarchyNodes(_hierarchyTagTypes, _currentNode.Parent, (hierarchy, parent, tag) =>
                         hierarchy.EnterChild(Taggable.CreatePackage(this, parent.Layer), Taggable.CreatePackage(this, node.Layer), tag.Parameter));

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
                CancelDialogue();

            return;
        }

        _currentNode.Tags.ForEach(x => x.TagBehaviour.ExitTag(Taggable.CreatePackage(this, _currentNode.Layer), x.Parameter));

        FindHierarchyNodes(_hierarchyTagTypes, _currentNode.Parent, (hierarchy, parent, tag) =>
            hierarchy.ExitChild(Taggable.CreatePackage(this, parent.Layer), Taggable.CreatePackage(this, _currentNode.Layer), tag.Parameter));

        DestroyActions();

        ParagraphAsset paragraph = _paragraphStack.Pop();
        StringBuilder content = new StringBuilder(paragraph.Text ?? "");

        DialogueUtility.Alias(content, _aliases);

        // Create node.
        Node node = new Node(_currentParent, _currentLayer, DeserializeTags(paragraph.Tag, _tagTypes));
        node.Tags.ForEach(x => x.TagBehaviour.EnterTag(Taggable.CreatePackage(this, node.Layer), x.Parameter));
        _tree.Add(_currentLayer, node);
        _currentNode = node;

        FindHierarchyNodes(_hierarchyTagTypes, _currentNode.Parent, (hierarchy, parent, tag) => 
            hierarchy.EnterChild(Taggable.CreatePackage(this, parent.Layer), Taggable.CreatePackage(this, node.Layer), tag.Parameter));

        for (int i = 0; i < _richTagTypes.Count; i++)
        {
            KeyValuePair<string, RichTagData> richTag = _richTagTypes.ElementAt(i);
            RichTagData data = richTag.Value;
            data.Sequences = DeserializeRichTag(content, richTag.Key);
            _richTagTypes[richTag.Key] = data;
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

        DialogueUtility.InitializeAllTags(this, _tagTypes, _hierarchyTagTypes, _richTagTypes);
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

        List<CurrentRichTagData> currentRichTags = new List<CurrentRichTagData>();
        List<RichTagData> richTagsUsed = new List<RichTagData>();
        foreach (RichTagData data in _richTagTypes.Values)  // Look for all rich tags used in this paragraph.
            if (data.Sequences.Count > 0)
            {
                (string, RangeInt) tag = data.Sequences.Dequeue();
                currentRichTags.Add(new CurrentRichTagData { Execution = data.Execution, Sequence = tag });
                richTagsUsed.Add(data);
            }

        List<(IHierarchyTag, Node, Tag)> _currentHierarchy = new List<(IHierarchyTag, Node, Tag)>();
        FindHierarchyNodes(_hierarchyTagTypes, _currentNode.Parent, (hierarchy, parent, tag) => _currentHierarchy.Add((hierarchy, parent, tag)));

        _content.ForceMeshUpdate();

        string parsedText = _content.GetParsedText().ToUpper();

        int maxLength = _currentTextMaxLength = _content.textInfo.characterCount + 1;
        for (int currentIndex = 0; currentIndex < maxLength; currentIndex++)  
        {
            object yieldValue = null;
            _currentTextIndex = currentIndex;

            _content.maxVisibleCharacters = currentIndex;   // How many letters that should be visible.

            for (int i = 0; i < _currentHierarchy.Count; i++)   // Loop through all current hierarchy tags
            {
                IEnumerator enumerator = _currentHierarchy[i].Item1.ProcessChild(
                                            Taggable.CreatePackage(this, _currentHierarchy[i].Item2.Layer), Taggable.CreatePackage(this, _currentNode.Layer),
                                            currentIndex, maxLength, _currentHierarchy[i].Item3.Parameter);
                enumerator.MoveNext();
                object returnValue = enumerator.Current;

                if (returnValue != null)  // Don't Return if the IEnumerator returns null.
                    yieldValue = returnValue;
            }

            for (int i = 0; i < _currentNode.Tags.Length; i++)   // Loop through all current tags.
            {
                IEnumerator enumerator = _currentNode.Tags[i].TagBehaviour.ProcessTag(Taggable.CreatePackage(this, _currentLayer), _currentTextIndex, _currentTextMaxLength, _currentNode.Tags[i].Parameter);
                enumerator.MoveNext();
                object returnValue = enumerator.Current;

                if (returnValue != null)  // Don't Return if the IEnumerator returns null.
                    yieldValue = returnValue;
            }

            for (int i = richTagsUsed.Count - 1; i >= 0; i--)   // Loop through all current rich tags.
            {
                CurrentRichTagData data = currentRichTags[i];
                
                if (currentIndex >= data.Sequence.Item2.start && currentIndex < data.Sequence.Item2.end) // Execute in range.
                {
                    if (!data.HasExecuted)  // Execute at the start of the process.
                    {
                        data.HasExecuted = true;
                        data.Execution.EnterTag(Taggable.CreatePackage(this, _currentLayer), currentIndex, data.Sequence.Item2, data.Sequence.Item1);
                    }

                    currentRichTags[i] = data;

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
                        currentRichTags[i] = new CurrentRichTagData { Execution = richTagsUsed[i].Execution, Sequence = richTagsUsed[i].Sequences.Dequeue() };
                    }
                    else    // Remove if no more rich tags of this type is found.
                    {
                        richTagsUsed.RemoveAt(i);
                        currentRichTags.RemoveAt(i);
                    }
                }
            }

            if (yieldValue != null) // Don't yield if value is null.
                yield return yieldValue;
        }

        _isAutoTextCompleted = true;
    }
}
