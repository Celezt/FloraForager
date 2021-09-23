using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class DialogueUtility
{
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
