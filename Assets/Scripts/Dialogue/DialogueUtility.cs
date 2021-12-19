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
using UnityEngine.ResourceManagement.AsyncOperations;

public static class DialogueUtility
{
    public const string DIALOGUE_EXCEPTION = "DialogueException";

    /// <summary>
    /// Convert all id to mapped alias.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="aliases">All aliases.</param>
    /// <returns>StringBuilder.</returns>
    public static StringBuilder Alias(StringBuilder text, IDictionary<string, string> aliases)
    {
        // Get all aliases inside {alias}.
        MatchCollection matches = Regex.Matches(text.ToString(), @"(?<=\{).*?(?=\})");

        // Replace all aliases.
        for (int i = 0; i < matches.Count; i++)
            if (aliases.TryGetValue(matches[i].Value, out string alias))
                text = text.Replace($"{{{matches[i].Value}}}", alias);

        return text;
    }

    /// <summary>
    /// Load all aliases with label.
    /// </summary>
    /// <param name="aliasLabel">Label reference.</param>
    /// <param name="aliases">Aliases.</param>
    public static async void LoadAliases(AssetLabelReference aliasLabel, IDictionary<string, string> aliases)
    {
        AsyncOperationHandle<IList<TextAsset>> aliasHandle = Addressables.LoadAssetsAsync<TextAsset>(aliasLabel,
            handle =>
            {

            });

        await aliasHandle.Task;

        IList<TextAsset> assets = aliasHandle.Result;

        for (int i = 0; i < assets.Count; i++)
        {
            Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(assets[i].text);
            foreach (var item in dictionary)
                if (!aliases.ContainsKey(item.Key))
                    aliases.Add(item.Key, item.Value);
        }
    }

    /// <summary>
    /// Extract custom rich tag and return a queue with the value and range of effect,
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="richTag">RichTag to find.</param>
    /// <param name="queue">Queue with the value and range of effect.</param>
    public static void ExtractCustomRichTag(StringBuilder text, string richTag, out Queue<(string, RangeInt)> queue)
    {
        string copy = text.ToString();
        queue = new Queue<(string, RangeInt)>(); 
        Regex openRegex = new Regex($"<{richTag}(=((?:.(?!\\1|>))*.?)\\1?)?>");
        Regex closeRegex = new Regex($"((<\\/){richTag}(>))");

        MatchCollection openMatches = openRegex.Matches(text.ToString());
        MatchCollection closeMatches = closeRegex.Matches(text.ToString());

        if (openMatches.Count != closeMatches.Count)
        {
            Debug.LogError($"{DIALOGUE_EXCEPTION}: Uneven number of open and closed tags");
            return;
        }

        for (int i = 0; i < openMatches.Count; i++)
        {
            Group openGroup = openMatches[i].Groups[2];
            Group closeGroup = closeMatches[i].Groups[2];

            bool insideTag = false;
            int closedIndex = 0;
            int openIndex = 0;
            for (int j = 0; j < closeGroup.Index; j++)
            {
                if (copy[j] == '<')
                    insideTag = true;
                else if (copy[j] == '>')
                    insideTag = false;
                else if (!insideTag)
                {
                    closedIndex++;

                    if (j < openGroup.Index)
                        openIndex++;
                }
            }

            queue.Enqueue((openGroup.Value, new RangeInt(openIndex, closedIndex - openIndex)));

            text.Replace(openMatches[i].Groups[0].Value, "");
            text.Replace(closeMatches[i].Groups[0].Value, "");
        }
    }

    /// <summary>
    /// Invoke all enter actions connected to tags.
    /// </summary>
    /// <param name="wrap">Wrapped reference.</param>
    /// <param name="layer">Hierarchy layer.</param>
    /// <param name="tags">Tags.</param>
    /// <param name="tagDictionary">All actions.</param>
    public static void TagEnterInvoke<T>(T wrap, int layer, IList<string> tags, IDictionary<string, ITag> tagDictionary, List<(string, ITag)> tagsToUpdate = null)
    {
        // Invoke all tags.
        if (tags != null)
            for (int i = 0; i < tags.Count; i++)
            {
                string value = Regex.Match(tags[i], @"(?<=\{).*?(?=\})").Value;        // Match value.
                string tag = Regex.Match(tags[i], @"\w+(?=(?>[^{}]+|\{|\})*$)").Value; // Match tag name.

                if (!tagDictionary.ContainsKey(tag))
                {
                    Debug.LogWarning($"{DIALOGUE_EXCEPTION}: {tag} does not exist");
                    return;
                }
                
                tagDictionary[tag].EnterTag(Taggable.CreatePackage(wrap, layer),  value);
                tagsToUpdate?.Add((value, tagDictionary[tag]));
            }
    }

    /// <summary>
    /// Invoke all exit actions connected to tags.
    /// </summary>
    /// <param name="wrap">Wrapped reference.</param>
    /// <param name="layer">Hierarchy layer.</param>
    /// <param name="tags">Tags.</param>
    /// <param name="tagDictionary">All actions.</param>
    public static void TagExitInvoke<T>(T wrap, int layer, IList<string> tags, IDictionary<string, ITag> tagDictionary, List<(string, ITag)> tagsToUpdate = null)
    {
        // Invoke all tags.
        if (tags != null)
            for (int i = 0; i < tags.Count; i++)
            {
                string value = Regex.Match(tags[i], @"(?<=\{).*?(?=\})").Value;        // Match value.
                string tag = Regex.Match(tags[i], @"\w+(?=(?>[^{}]+|\{|\})*$)").Value; // Match tag name.

                if (!tagDictionary.ContainsKey(tag))
                {
                    Debug.LogWarning($"{DIALOGUE_EXCEPTION}: {tag} does not exist");
                    return;
                }

                tagDictionary[tag].ExitTag(Taggable.CreatePackage(wrap, layer), value);
                tagsToUpdate?.Remove((value, tagDictionary[tag]));
            }
    }

    /// <summary>
    /// Reflect all tags inside the assembly.
    /// </summary>
    /// <param name="wrap">Wrapped reference.</param>
    /// <param name="layer">Hierarchy layer.</param>
    /// <param name="tagDictionary">All actions.</param>
    public static void InitializeAllTags<T>(T wrap, int layer, IDictionary<string, ITag> tagDictionary, Dictionary<string, DialogueManager.RichTagData> richTagDicitonary)
    {
        static string ReplaceLastOccurrence(string source, string find, string replace)
        {
            int place = source.LastIndexOf(find);

            if (place == -1)
                return source;

            string result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }

        foreach (Type type in ReflectionUtility.GetTypesWithAttribute<CustomDialogueTagAttribute>(Assembly.GetExecutingAssembly()))
        {
            if (!typeof(ITaggable).IsAssignableFrom(type))
            {
                Debug.LogError($"{DIALOGUE_EXCEPTION}: {type} has no derived {nameof(ITaggable)}");
                continue;
            }

            ITaggable instance = (ITaggable)Activator.CreateInstance(type);
            CustomDialogueTagAttribute attribute = (CustomDialogueTagAttribute)Attribute.GetCustomAttribute(type, typeof(CustomDialogueTagAttribute));
            
            instance.Initialize(Taggable.CreatePackage(wrap, layer));

            if (instance is ITag)
            {

                tagDictionary.Add(ReplaceLastOccurrence(instance.GetType().Name, "Tag", "").ToSnakeCase(), instance as ITag);
            }
            else if (instance is IRichTag)
            {

                richTagDicitonary.Add(ReplaceLastOccurrence(instance.GetType().Name, "RichTag", "").ToSnakeCase(), new DialogueManager.RichTagData { Execution = instance as IRichTag });
            }
        }
    }
}
