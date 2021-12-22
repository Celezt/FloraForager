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
    public IReadOnlyDictionary<string, IRichTag> RichTagTypes => _richTagTypes;
    public IReadOnlyDictionary<string, IEventTag> EventTagTypes => _eventTagTypes;
    public IReadOnlyList<RichTag> CurrentRichTag => _currentIRichTags;
    public IReadOnlyList<EventTag> CurrentEventTag => _currentEventTags;
    public TextMeshProUGUI Text => _content;
    public TextMeshProUGUI Namecard => _namecard;
    public DialogueUtility.Tree Tree => _tree;
    public Node RootNode => _rootNode;
    public Node CurrentNode => _currentNode;
    public Node CurrentParent => _currentParent;
    public string ParsedText => _parsedText;
    public int CurrentLayer => _currentLayer;
    public int CurrentTextIndex => _currentTextIndex;
    public int CurrentTextMaxLength => _currentTextMaxLength;
    public int PlayerIndex => _playerIndex;
    /// <summary>
    /// Base text read speed.
    /// </summary>
    public float AutoTextSpeed => _autoTextSpeed;
    /// <summary>
    /// Is the current paragraph skippable. Resets each new paragraph.
    /// </summary>
    public bool IsSkippable
    {
        get => _isSkippable;
        set => _isSkippable = value;
    }
    /// <summary>
    /// If auto text was cancelled. 
    /// </summary>
    public bool IsAutoCancelled => _isAutoCancelled;
    /// <summary>
    /// If dialogue is running.
    /// </summary>
    public bool IsDialogueActive => _isDialogueActive;

    [SerializeField, Min(0)] private int _playerIndex;
    [SerializeField] private TextMeshProUGUI _content;
    [SerializeField] private TextMeshProUGUI _namecard;
    [SerializeField] private TextMeshProUGUI _skipSymbol;
    [SerializeField] private GameObject _namecardUI;
    [SerializeField] private GameObject _dialogueUI;
    [SerializeField] private Transform _buttonParent;
    [SerializeField] private GameObject _buttonType;
    [SerializeField] private AssetLabelReference _aliasLabel;
    [SerializeField] private float _autoTextSpeed = 0.1f;
    [SerializeField] private float _skipFadeSpeed = 0.5f;

    private Stack<ParagraphAsset> _paragraphStack;
    private List<GameObject> _actions;
    private List<EventTag> _currentEventTags;
    private List<RichTag> _currentIRichTags;
    private Dictionary<string, string> _aliases = new Dictionary<string, string>();
    private Dictionary<string, (ITag, string)> _defaultTagTypes = new Dictionary<string, (ITag, string)>();
    private Dictionary<string, ITag> _tagTypes = new Dictionary<string, ITag>();
    private Dictionary<string, IHierarchyTag> _hierarchyTagTypes = new Dictionary<string, IHierarchyTag>();
    private Dictionary<string, IRichTag> _richTagTypes = new Dictionary<string, IRichTag>();
    private Dictionary<string, IEventTag> _eventTagTypes = new Dictionary<string, IEventTag>();

    private DialogueUtility.Tree _tree;
    private Node _rootNode;
    private Node _currentNode;
    private Node _currentParent;
    private Coroutine _autoTypeCoroutine;
    private Coroutine _skipFadeCoroutine;

    private string _parsedText;
    private int _currentLayer;
    private int _currentTextIndex;
    private int _currentTextMaxLength;
    private bool _isAutoTextCompleted;
    private bool _isDialogueActive;
    private bool _isSkippable;
    private bool _isAutoCancelled;

    /// <summary>
    /// Return Dialogue Manager based on the player index connected to it.
    /// </summary>
    /// <param name="playerIndex">Index.</param>
    /// <returns>Dialogue Manager.</returns>
    public static DialogueManager GetByIndex(int playerIndex) => _dialogues[playerIndex];

    public DialogueManager SetDefaultTag(ITag self, bool isDefault, string defualtParameter = "")
    {
        static string ReplaceLastOccurrence(string source, string find, string replace)
        {
            int place = source.LastIndexOf(find);

            if (place == -1)
                return source;

            string result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }

        string name = ReplaceLastOccurrence(self.ToString(), "Tag", "").ToSnakeCase();

        if (isDefault)
        { 
            if (!_defaultTagTypes.ContainsKey(name))
                _defaultTagTypes.Add(name, (self, defualtParameter));
        }
        else
          _defaultTagTypes.Remove(name);

        return this;
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
        _isDialogueActive = true;

        Started.Invoke(this);
        _dialogueUI.SetActive(true);

        _currentLayer = 0;
        _isSkippable = true;

        // Call all OnActive.
        _tagTypes.ForEach(x => x.Value.OnActive(Taggable.CreatePackage(this, int.MinValue)));
        _eventTagTypes.ForEach(x => x.Value.OnActive(Taggable.CreatePackage(this, int.MinValue)));
        _richTagTypes.ForEach(x => x.Value.OnActive(Taggable.CreatePackage(this, int.MinValue)));

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

        {
            // Create Tree.
            _tree = new DialogueUtility.Tree();

            // Create root node.
            _rootNode = new Node(null, _currentLayer, DeserializeTags(asset.Tag, _tagTypes));

            // Add all default tags.
            List<Tag> tempTags = _rootNode.Tags.ToList();
            foreach (KeyValuePair<string, (ITag, string)> defaultTag in _defaultTagTypes)
            {
                if (!tempTags.Any(x => x.Name == defaultTag.Key))
                    tempTags.Add(new Tag() { Name = defaultTag.Key, Behaviour = defaultTag.Value.Item1, Parameter = defaultTag.Value.Item2 });
            }
            _rootNode.Tags = tempTags.ToArray();

            _rootNode.Tags.ForEach(x => x.Behaviour.EnterTag(Taggable.CreatePackage(this, _rootNode.Layer), x.Parameter));
            _tree.Add(_currentLayer, _rootNode);
            _currentNode = _rootNode;
            _currentParent = _rootNode;
        }

        _skipFadeCoroutine = StartCoroutine(SkipFade());

        _paragraphStack = new Stack<ParagraphAsset>(asset.Dialogue.Reverse<ParagraphAsset>());
        _currentLayer++;
        _isAutoTextCompleted = true;
        Next();

        Addressables.Release(handle);
    }

    public void CancelDialogue()
    {
        _isDialogueActive = false;

        _dialogueUI.SetActive(false);

        if (_autoTypeCoroutine != null)
            StopCoroutine(_autoTypeCoroutine);

        if (_skipFadeCoroutine != null)
            StopCoroutine(_skipFadeCoroutine);

        {
            // Exit tags.
            _currentNode.Tags.ForEach(x => x.Behaviour.ExitTag(Taggable.CreatePackage(this, _currentNode.Layer), x.Parameter));

            FindHierarchyNodes(_hierarchyTagTypes, _currentNode.Parent, (hierarchy, parent, tag) =>
                hierarchy.ExitChild(Taggable.CreatePackage(this, parent.Layer), Taggable.CreatePackage(this, _currentNode.Layer), tag.Parameter));
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
                Alias(act, _aliases);

                textMesh.text = act.ToString();
                button.onClick.AddListener(() =>
                {
                    {
                        // Exit node.
                        _currentNode.Tags.ForEach(x => x.Behaviour.ExitTag(Taggable.CreatePackage(this, _currentNode.Layer), x.Parameter));

                        FindHierarchyNodes(_hierarchyTagTypes, _currentNode.Parent, (hierarchy, parent, tag) =>
                            hierarchy.ExitChild(Taggable.CreatePackage(this, parent.Layer), Taggable.CreatePackage(this, _currentNode.Layer), tag.Parameter));
                    }

                    _currentParent = _currentNode;
                    _currentLayer++;

                    _paragraphStack = new Stack<ParagraphAsset>(paragraph.Action[index].Dialogue.Reverse<ParagraphAsset>());

                    {
                        // Create node.
                        Node node = new Node(_currentParent, _currentLayer, DeserializeTags(paragraph.Action[index].Tag, _tagTypes));
                        node.Tags.ForEach(x => x.Behaviour.EnterTag(Taggable.CreatePackage(this, node.Layer), x.Parameter));
                        _tree.Add(_currentLayer, node);
                        _currentNode = node;
                        _currentParent = node;

                        FindHierarchyNodes(_hierarchyTagTypes, _currentNode.Parent, (hierarchy, parent, tag) =>
                             hierarchy.EnterChild(Taggable.CreatePackage(this, parent.Layer), Taggable.CreatePackage(this, node.Layer), tag.Parameter));
                    }

                    _currentLayer++;
                    _isAutoTextCompleted = true;
                    Next();
                });

                _actions.Add(obj);
            }
        }

        if (!_isAutoTextCompleted)  // Skip auto text if not yet completed.
        {
            if (!_isSkippable)      // If not allowed to skip whiles auto text is running.
                return;

            if (_autoTypeCoroutine != null)
                StopCoroutine(_autoTypeCoroutine);

            _isAutoTextCompleted = true;
            _content.maxVisibleCharacters = int.MaxValue;

            {
                _isAutoCancelled = true;

                // Cancel tags.
                _currentNode.Tags.ForEach(x => x.Behaviour.ExitTag(Taggable.CreatePackage(this, _currentNode.Layer), x.Parameter));

                FindHierarchyNodes(_hierarchyTagTypes, _currentNode.Parent, (hierarchy, parent, tag) =>
                    hierarchy.ExitChild(Taggable.CreatePackage(this, parent.Layer), Taggable.CreatePackage(this, _currentNode.Layer), tag.Parameter));

                _currentIRichTags.ForEach(x => x.Pipe(y => y.Behaviour.ExitTag(Taggable.CreatePackage(this, _currentNode.Layer), _currentTextIndex, y.Range, y.Parameter)));

                _currentEventTags.ForEach(x => x.Behaviour.OnTrigger(Taggable.CreatePackage(this, _currentNode.Layer), _currentTextIndex, x.Parameter).MoveNext());

                _isAutoCancelled = false;
            }

            return;
        }

        if (_paragraphStack.IsNullOrEmpty())
        {
            if (_actions.IsNullOrEmpty())   // Cancel dialogue.
                CancelDialogue();

            return;
        }

        {
            // Exit node.
            _currentNode.Tags.ForEach(x => x.Behaviour.ExitTag(Taggable.CreatePackage(this, _currentNode.Layer), x.Parameter));

            FindHierarchyNodes(_hierarchyTagTypes, _currentNode.Parent, (hierarchy, parent, tag) =>
                hierarchy.ExitChild(Taggable.CreatePackage(this, parent.Layer), Taggable.CreatePackage(this, _currentNode.Layer), tag.Parameter));
        }

        DestroyActions();

        ParagraphAsset paragraph = _paragraphStack.Pop();
        StringBuilder text = new StringBuilder(paragraph.Text ?? "");
        Alias(text, _aliases);

        {
            // Create node.
            Node node = new Node(_currentParent, _currentLayer, DeserializeTags(paragraph.Tag, _tagTypes));
            node.Tags.ForEach(x => x.Behaviour.EnterTag(Taggable.CreatePackage(this, node.Layer), x.Parameter));
            _tree.Add(_currentLayer, node);
            _currentNode = node;

            FindHierarchyNodes(_hierarchyTagTypes, _currentNode.Parent, (hierarchy, parent, tag) =>
                hierarchy.EnterChild(Taggable.CreatePackage(this, parent.Layer), Taggable.CreatePackage(this, node.Layer), tag.Parameter));
        }

        {
            // Find all current tags.
            _currentIRichTags = new List<RichTag>();
            for (int i = 0; i < _richTagTypes.Count; i++) // Find current rich tags.
                if (TryDeserializeRichTag(text, _richTagTypes.ElementAt(i).Key, _richTagTypes, out Queue<RichTag> queue))
                    _currentIRichTags.AddRange(queue);
            _currentIRichTags.Reverse();    // Invert the order of all rich tags.

            _currentEventTags = new List<EventTag>();
            for (int i = 0; i < _eventTagTypes.Count; i++)  // Find current event tags.
                if (TryDeserializeEventTag(text, _eventTagTypes.ElementAt(i).Key, _eventTagTypes, out Queue<EventTag> queue))
                    _currentEventTags.AddRange(queue);
        }

        {
            // Render all data.
            _content.text = text.ToString();

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
        }

        if (_autoTypeCoroutine != null)
            StopCoroutine(_autoTypeCoroutine);

        _autoTypeCoroutine = StartCoroutine(AutoType(paragraph));

        CreateActions(paragraph);
    }

    private void Awake()
    {
        static string ReplaceLastOccurrence(string source, string find, string replace)
        {
            int place = source.LastIndexOf(find);

            if (place == -1)
                return source;

            string result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }

        _dialogues.Add(_playerIndex, this);

        // Reflect all tags inside the assembly.
        foreach (Type type in ReflectionUtility.GetTypesWithAttribute<CustomDialogueTagAttribute>(Assembly.GetExecutingAssembly()))
        {
            if (!typeof(ITaggable).IsAssignableFrom(type))
            {
                Debug.LogError($"{DIALOGUE_EXCEPTION}: {type} has no derived {nameof(ITaggable)}");
                continue;
            }

            ITaggable instance = (ITaggable)Activator.CreateInstance(type);
            CustomDialogueTagAttribute attribute = (CustomDialogueTagAttribute)Attribute.GetCustomAttribute(type, typeof(CustomDialogueTagAttribute));

            instance.Initialize(Taggable.CreatePackage(this, int.MinValue));

            if (instance is ITag)
            {
                string name = ReplaceLastOccurrence(instance.GetType().Name, "Tag", "").ToSnakeCase();
                _tagTypes.Add(name, instance as ITag);

                if (instance is IHierarchyTag)  // Optional variant.
                    _hierarchyTagTypes.Add(name, instance as IHierarchyTag);
            }
            else if (instance is IRichTag)
                _richTagTypes.Add(ReplaceLastOccurrence(instance.GetType().Name, "RichTag", "").ToSnakeCase(), instance as IRichTag);
            else if (instance is IEventTag)
                _eventTagTypes.Add(ReplaceLastOccurrence(instance.GetType().Name, "EventTag", "").ToSnakeCase(), instance as IEventTag );
        }
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

    private IEnumerator SkipFade()
    {
        while (true)
        {
            if (_isDialogueActive && _skipSymbol != null)
                _skipSymbol.alpha = _isSkippable ? Mathf.PingPong(Time.time * _skipFadeSpeed, 1) : 0.0f;

            yield return null;
        }
    }

    private IEnumerator AutoType(ParagraphAsset paragraph)
    {
        _isAutoTextCompleted = false;

        List<bool> _richTagsExecuted = Enumerable.Repeat(false, _currentIRichTags.Count).ToList();

        List<(IHierarchyTag, Node, Tag)> _currentHierarchy = new List<(IHierarchyTag, Node, Tag)>();
        FindHierarchyNodes(_hierarchyTagTypes, _currentNode.Parent, (hierarchy, parent, tag) => _currentHierarchy.Add((hierarchy, parent, tag)));   // Look for all hierarchy tags to update.

        _content.ForceMeshUpdate();
        _parsedText = _content.GetParsedText().ToUpper();

        int maxLength = _currentTextMaxLength = _content.textInfo.characterCount;
        for (int currentIndex = 0; currentIndex < maxLength; currentIndex++)  
        {
            object yieldValue = null;
            _currentTextIndex = currentIndex;

            _content.maxVisibleCharacters = currentIndex + 1;   // How many letters that should be visible.

            //
            // Loop through all current event tags.
            //
            object currentValue = null;
            for (int i = _currentEventTags.Count - 1; i >= 0; i--)
            {
                EventTag eventTag = _currentEventTags[i];

                if (currentIndex >= eventTag.Index)
                {
                    IEnumerator enumerator = eventTag.Behaviour.OnTrigger(Taggable.CreatePackage(this, _currentLayer), currentIndex, eventTag.Parameter);
                    enumerator.MoveNext();
                    object returnValue = enumerator.Current;

                    if (returnValue != null && yieldValue == null)            // Don't Return if the IEnumerator returns null and no yield value has been set yet.
                        currentValue = returnValue;

                    _currentEventTags.RemoveAt(i);
                }
            }

            if (yieldValue == null)
                yieldValue = currentValue;

            //
            // Loop through all current rich tags.
            //
            currentValue = null;
            for (int i = _currentIRichTags.Count - 1; i >= 0; i--)
            {
                RichTag richTag = _currentIRichTags[i];

                if (currentIndex >= richTag.Range.start && currentIndex < richTag.Range.end)    // Execute in range.
                {
                    if (!_richTagsExecuted[i])              // Execute at the start of the process.
                    {
                        _richTagsExecuted[i] = true;
                        richTag.Behaviour.EnterTag(Taggable.CreatePackage(this, _currentLayer), currentIndex, richTag.Range, richTag.Parameter);
                    }

                    IEnumerator enumerator = richTag.Behaviour.ProcessTag(Taggable.CreatePackage(this, _currentLayer), currentIndex, richTag.Range, richTag.Parameter);
                    enumerator.MoveNext();
                    object returnValue = enumerator.Current;

                    if (returnValue != null && yieldValue == null)            // Don't Return if the IEnumerator returns null and no yield value has been set yet.
                        currentValue = returnValue;
                }

                if (_richTagsExecuted[i] && currentIndex >= richTag.Range.end - 1)  // If executed and no longer in range.
                {
                    richTag.Behaviour.ExitTag(Taggable.CreatePackage(this, _currentLayer), currentIndex, richTag.Range, richTag.Parameter);

                    _currentIRichTags.RemoveAt(i);
                    _richTagsExecuted.RemoveAt(i);
                }
            }

            if (yieldValue == null)
                yieldValue = currentValue;

            //
            // Loop through all current tags.
            //
            for (int i = 0; i < _currentNode.Tags.Length; i++)
            {
                IEnumerator enumerator = _currentNode.Tags[i].Behaviour.ProcessTag(Taggable.CreatePackage(this, _currentLayer), _currentTextIndex, _currentTextMaxLength, _currentNode.Tags[i].Parameter);
                enumerator.MoveNext();
                object returnValue = enumerator.Current;

                if (returnValue != null && yieldValue == null)            // Don't Return if the IEnumerator returns null and no yield value has been set yet.
                    yieldValue = returnValue;
            }
            //
            // Loop through all current hierarchy tags
            //
            for (int i = 0; i < _currentHierarchy.Count; i++)
            {
                IEnumerator enumerator = _currentHierarchy[i].Item1.ProcessChild(
                                            Taggable.CreatePackage(this, _currentHierarchy[i].Item2.Layer), Taggable.CreatePackage(this, _currentNode.Layer),
                                            currentIndex, maxLength, _currentHierarchy[i].Item3.Parameter);
                enumerator.MoveNext();
                object returnValue = enumerator.Current;

                if (returnValue != null && yieldValue == null)            // Don't Return if the IEnumerator returns null and no yield value has been set yet.
                    yieldValue = returnValue;
            }     

            if (yieldValue != null)                     // Don't yield if value is null.
                yield return yieldValue;
        }

        _isAutoTextCompleted = true;
    }
}
