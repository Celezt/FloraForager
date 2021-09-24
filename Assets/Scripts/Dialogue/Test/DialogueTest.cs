using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using MyBox;
using System.Linq;
using System.Text;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
public class DialogueTest : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _textMesh;
    [SerializeField] private AssetReferenceText _asset;
    [SerializeField] private Button[] _buttons;

    private Stack<ParagraphAsset> _dialogueStack;
    private Dictionary<string, string> _aliases = new Dictionary<string, string>
    {
        {"joker_test", "Joker" },
        {"victim_test", "You" }
    };
    private Dictionary<string, System.Action<string>> _tags = new Dictionary<string, System.Action<string>>
    {
        {"audio_test",  (string value) => { Debug.Log(value); } },
        {"activate_test",  (string _) => { Debug.Log("Activated"); } }
    };

    [ButtonMethod]
    private void Deserialize()
    {
        string serializedData = string.Empty;
        _asset.LoadAssetAsync<TextAsset>().Completed += (handle) =>
        {
            serializedData = handle.Result.text;
            Addressables.Release(handle);
        };

        DialogueAsset asset = JsonConvert.DeserializeObject<DialogueAsset>(serializedData);

        _dialogueStack = new Stack<ParagraphAsset>(asset.Dialogues.Reverse());
        Next();
    }

    [ButtonMethod]
    private void Next()
    {
        if (_dialogueStack.IsNullOrEmpty())
            return;

        ParagraphAsset paragraph = _dialogueStack.Pop();
        StringBuilder text = new StringBuilder(paragraph.Text.ToString());

        DialogueUtility.Alias(text, _aliases);
        DialogueUtility.Tag(paragraph.Tag, _tags);

        if (paragraph.Action != null)
        {
            for (int i = 0; i < paragraph.Action.Count; i++)
            {
                StringBuilder act = new StringBuilder(paragraph.Action[i].Act);
                DialogueUtility.Alias(act, _aliases);
                _buttons[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = act.ToString();
            }
        }

        _textMesh.text = $"{_aliases[paragraph.ID]}\n{text}";
    }
}
#endif