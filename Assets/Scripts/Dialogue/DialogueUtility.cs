using FF.Json;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class DialogueUtility
{
    /// <summary>
    /// Deserialize JSON-file already loaded.
    /// </summary>
    /// <param name="fileName">File name.</param>
    /// <returns>Stack of <c>ParagraphAsset</c>.</returns>
    private static Stack<ParagraphAsset> Deserialize(string fileName)
    {
        // Deserialize the string to an object. 
        DialogueAsset asset = JsonConvert.DeserializeObject<DialogueAsset>(JsonLoader.GetContent(fileName));

        return new Stack<ParagraphAsset>(asset.Dialogues.Reverse());
    }

    /// <summary>
    /// Convert all id to mapped alias.
    /// </summary>
    /// <param name="content">Text.</param>
    /// <param name="aliases">All aliases.</param>
    /// <returns>StringBuilder.</returns>
    public static StringBuilder Alias(StringBuilder content, IDictionary<string, string> aliases)
    {
        // Get all aliases inside {alias}.
        MatchCollection matches = Regex.Matches(content.ToString(), @"(?<=\{).*?(?=\})");

        // Replace all aliases.
        for (int i = 0; i < matches.Count; i++)
            content = content.Replace($"{{{matches[i].Value}}}", aliases[matches[i].Value]);

        return content;
    }

    /// <summary>
    /// Invoke all actions connected to tags.
    /// </summary>
    /// <param name="tags">Tags.</param>
    /// <param name="actions">All actions.</param>
    public static void Tag(IList<string> tags, IDictionary<string, System.Action<string>> actions)
    {
        // Invoke all tags.
        if (tags != null)
            for (int i = 0; i < tags.Count; i++)
            {
                string value = Regex.Match(tags[i], @"(?<=\{).*?(?=\})").Value;        // Match value.
                string tag = Regex.Match(tags[i], @"\w+(?=(?>[^{}]+|\{|\})*$)").Value; // Match tag name.
                
                actions[tag].Invoke(value);
            }
    }
}
