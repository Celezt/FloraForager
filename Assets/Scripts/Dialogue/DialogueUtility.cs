using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class DialogueUtility
{
    /// <summary>
    /// Deserialize JSON-file already loaded.
    /// </summary>
    /// <param name="address">Address.</param>
    /// <returns>Stack of <c>ParagraphAsset</c>.</returns>
    public static Stack<ParagraphAsset> Deserialize(string content)
    {
        // Deserialize the string to an object. 
        DialogueAsset asset = JsonConvert.DeserializeObject<DialogueAsset>(content);

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
    /// <param name="wrap">Wrapped reference.</param>
    /// <param name="tags">Tags.</param>
    /// <param name="tagDictionary">All actions.</param>
    public static void Tag<T>(T wrap, IList<string> tags, IDictionary<string, System.Action<Taggable, string>> tagDictionary)
    {
        // Invoke all tags.
        if (tags != null)
            for (int i = 0; i < tags.Count; i++)
            {
                string value = Regex.Match(tags[i], @"(?<=\{).*?(?=\})").Value;        // Match value.
                string tag = Regex.Match(tags[i], @"\w+(?=(?>[^{}]+|\{|\})*$)").Value; // Match tag name.

                if (!tagDictionary.ContainsKey(tag))
                {
                    Debug.LogWarning($"DialogueException: {tag} does not exist");
                    return;
                }
                
                tagDictionary[tag].Invoke(Taggable.CreatePackage(wrap), value);
            }
    }

    /// <summary>
    /// Reflect all tags inside the assembly.
    /// </summary>
    /// <param name="wrap">Wrapped reference.</param>
    /// <param name="tagDictionary">All actions.</param>
    public static void InitializeAllTags<T>(T wrap, IDictionary<string, Action<Taggable, string>> tagDictionary)
    {
        static string ReplaceLastOccurrence(string source, string find, string replace)
        {
            int place = source.LastIndexOf(find);

            if (place == -1)
                return source;

            string result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }

        foreach (Type type in ReflectionUtility.GetTypesWithAttribute<CustomTagAttribute>(Assembly.GetExecutingAssembly()))
        {
            if (!typeof(ITag).IsAssignableFrom(type))
            {
                Debug.LogError($"ReflectException: {type} has no derived {nameof(ITag)}");
                continue;
            }

            ITag instance = (ITag)Activator.CreateInstance(type);
            CustomTagAttribute attribute = (CustomTagAttribute)Attribute.GetCustomAttribute(type, typeof(CustomTagAttribute));
            
            instance.Initalize(Taggable.CreatePackage(wrap));
            tagDictionary.Add(ReplaceLastOccurrence(instance.GetType().Name, "Tag", "").ToLower(), instance.Action);
        }
    }
}
