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

public class DialogueManager : Singleton<DialogueManager>
{
    private const string DIALOGUE_EXCEPTION = "DialogueException";

    [SerializeField] private TextMeshProUGUI _content;
    [SerializeField] private TextMeshProUGUI _namecard;
    [SerializeField] private Transform _buttonParent;
    [SerializeField] private GameObject _buttonType;

    private Stack<ParagraphAsset> _paragraphStack;
    Queue<(string, int)> _customRichTagQueue;
    private List<GameObject> _actions;
    private Dictionary<string, string> _aliases = new Dictionary<string, string>();
    private Dictionary<string, Action<Taggable, string>> _tags = new Dictionary<string, Action<Taggable, string>>();

    private Coroutine _autoTypeCoroutine;

    private float _autoTextSpeed = 0.1f;

    public void Next()
    {
        if (_paragraphStack.IsNullOrEmpty())
            return;

        DestroyActions();

        ParagraphAsset paragraph = _paragraphStack.Pop();
        StringBuilder content = new StringBuilder(paragraph.Text.ToString());

        DialogueUtility.Alias(content, _aliases);
        DialogueUtility.Tag(this, paragraph.Tag, _tags);
        DialogueUtility.ExtractCustomRichTag(content, "speed", out _customRichTagQueue);

        _content.text = content.ToString();

        if (_aliases.TryGetValue(paragraph.ID, out string alias))
            _namecard.text = _aliases[paragraph.ID];


        if (_autoTypeCoroutine != null)
            StopCoroutine(_autoTypeCoroutine);

        _autoTypeCoroutine = StartCoroutine(AutoType());

        CreateActions(paragraph);
    }

    private void Awake()
    {
        DialogueUtility.InitializeAllTags(this, _tags);
    }

    private async void Start()
    {
        Addressables.LoadAssetAsync<TextAsset>("knock_knock_test_en").Completed += (handle) =>
        {
            TextAsset text = handle.Result;

            Addressables.Release(handle);
        };

        AsyncOperationHandle<TextAsset> aliasHandle = Addressables.LoadAssetAsync<TextAsset>("name_list_test_en");
        AsyncOperationHandle<TextAsset> dialogueHandle = Addressables.LoadAssetAsync<TextAsset>("knock_knock_test_en");

        await Task.WhenAll(aliasHandle.Task, dialogueHandle.Task);

        Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(aliasHandle.Result.text);
        foreach (var item in dictionary)
            _aliases.Add(item.Key, item.Value);

        _paragraphStack = DialogueUtility.Deserialize(dialogueHandle.Result.text);

        Addressables.Release(aliasHandle);
        Addressables.Release(dialogueHandle);

        Next();
    }

    private void DestroyActions()
    {
        if (_actions.NotNullOrEmpty())
        {
            for (int i = 0; i < _actions.Count; i++) 
                Destroy(_actions[i]);

            _actions = null;
        }
    }

    private void CreateActions(ParagraphAsset paragraph)
    {
        if (paragraph.Action != null)
        {
            _actions = new List<GameObject>();

            for (int i = 0; i < paragraph.Action.Count; i++)
            {
                int index = i;
                GameObject obj = Instantiate(_buttonType, _buttonParent);
                Button button = obj.GetComponentInChildren<Button>();
                TextMeshProUGUI textMesh = obj.GetComponentInChildren<TextMeshProUGUI>();

                StringBuilder act = new StringBuilder(paragraph.Action[i].Act);
                DialogueUtility.Alias(act, _aliases);

                textMesh.text = act.ToString();
                button.onClick.AddListener(delegate { OnActionClick(index, paragraph); });

                _actions.Add(obj);
            }
        }
    }

    private void OnActionClick(int i, ParagraphAsset paragraph)
    {
        _paragraphStack = new Stack<ParagraphAsset>(paragraph.Action[i].Dialogues.Reverse());
        Next();
    }

    private IEnumerator AutoType()
    {
        int index = 0;
        float speedMultiplier = 1;

        // Get any speed modifiers.
        if (_customRichTagQueue.Count > 0)
        {
            (string, int) customRichTag = _customRichTagQueue.Dequeue();
            index = customRichTag.Item2;
            if (!float.TryParse(customRichTag.Item1, out speedMultiplier))
            {
                Debug.LogError($"{DIALOGUE_EXCEPTION}: {customRichTag.Item1} could not be parsed to float");
                yield break;
            }

        }

        _content.ForceMeshUpdate();
        int maxCount = _content.textInfo.characterCount + 1;
        for (int count = 0; count < maxCount; count++)
        {
            _content.maxVisibleCharacters = count;

            if (count < index)
            {
                yield return new WaitForSeconds(_autoTextSpeed / speedMultiplier);
            }
            else if (_customRichTagQueue.Count > 0)
            {
                (string, int) customRichTag = _customRichTagQueue.Dequeue();
                index = customRichTag.Item2;
                if (!float.TryParse(customRichTag.Item1, out speedMultiplier))
                {
                    Debug.LogError($"{DIALOGUE_EXCEPTION}: {customRichTag.Item1} could not be parsed to float");
                    yield break;
                }

                if (count < index)
                {
                    yield return new WaitForSeconds(_autoTextSpeed / speedMultiplier);
                }
            }
            else
                yield return new WaitForSeconds(_autoTextSpeed);
        }
    }
}
