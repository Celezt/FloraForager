using MyBox;
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

    public class Tree 
    {
        public IReadOnlyList<IReadOnlyList<Node>> Nodes => _nodes;

        private List<List<Node>> _nodes = new List<List<Node>>();

        public void Add(int layer, Node node)
        {
            while (_nodes.Count <= layer)
                _nodes.Add(new List<Node>());

            if (!_nodes[layer].Contains(node))
                _nodes[layer].Add(node);

            node.Tree = this;
        }
    }

    public class Node
    {
        public Tree Tree;
        public Node Parent;
        public int Layer;
        public Tag[] Tags;

        public Node(Node parent, int layer, Tag[] tags)
        {
            Parent = parent;
            Layer = layer;
            Tags = tags;
        }
    }

    public struct Tag : IEquatable<Tag>
    {
        public string Name;
        public ITag TagBehaviour;
        public string Parameter;

        bool IEquatable<Tag>.Equals(Tag other) => other.Name == Name;
    }

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
    /// <returns>Queue with the value and range of effect.</returns>
    public static Queue<(string, RangeInt)> DeserializeRichTag(StringBuilder text, string richTag)
    {
        string copy = text.ToString();
        Queue<(string, RangeInt)> queue = new Queue<(string, RangeInt)>();
        Regex openRegex = new Regex($"<{richTag}(=((?:.(?!\\1|>))*.?)\\1?)?>");
        Regex closeRegex = new Regex($"((<\\/){richTag}(>))");

        MatchCollection openMatches = openRegex.Matches(text.ToString());
        MatchCollection closeMatches = closeRegex.Matches(text.ToString());

        if (openMatches.Count != closeMatches.Count)
            throw new Exception($"{DIALOGUE_EXCEPTION}: Uneven number of open and closed tags");

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

        return queue;
    }

    /// <summary>
    /// Deserialize all tags and convert it into <see cref="ITag"/>.
    /// </summary>
    public static Tag[] DeserializeTags(List<string> serializedTags, IDictionary<string, ITag> tagTypes) =>
        serializedTags?.Select(x =>
        {
            string tag = Regex.Match(x, @"\w+(?=(?>[^{}]+|\{|\})*$)").Value; // Match tag name.
            string parameter = Regex.Match(x, @"(?<=\{).*?(?=\})").Value;        // Match parameter.

            if (!tagTypes.ContainsKey(tag))    // If tag type exist.
                throw new Exception($"{DIALOGUE_EXCEPTION}: {tag} does not exist");

            return new Tag { Name = tag, TagBehaviour = tagTypes[tag], Parameter = parameter };
        }).ToArray() ?? new Tag[0];

    /// <summary>
    /// Find first hierarchy node.
    /// </summary>
    public static void FindHierarchyNodes(Dictionary<string, IHierarchyTag> hierarchyTagTypes, Node parent, Action<IHierarchyTag, Node, Tag> action)
    {
        if (parent == null)
            return;

        hierarchyTagTypes.ForEach(x =>
        {
            void FindHierarchyNodes(Node parent)
            {
                for (int i = 0; i < parent.Tags.Length; i++)
                {
                    if (x.Key == parent.Tags[i].Name)
                    {
                        action.Invoke(x.Value, parent, parent.Tags[i]);
                        return;
                    }

                }

                if (parent.Parent == null)  // Stop searching if no parent of hierarchy type is found.
                    return;

                FindHierarchyNodes(parent.Parent);
            }

            FindHierarchyNodes(parent);
        });
    }

    /// <summary>
    /// Reflect all tags inside the assembly.
    /// </summary>
    /// <param name="manager">Wrapped reference.</param>
    /// <param name="layer">Hierarchy layer.</param>
    /// <param name="tagTypes">All actions.</param>
    public static void InitializeAllTags(DialogueManager manager, IDictionary<string, ITag> tagTypes, IDictionary<string, IHierarchyTag> hierarchyTypes, Dictionary<string, DialogueManager.RichTagData> richTagTypes)
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
            
            instance.Initialize(Taggable.CreatePackage(manager, int.MinValue));

            if (instance is ITag)
            {
                string name = ReplaceLastOccurrence(instance.GetType().Name, "Tag", "").ToSnakeCase();
                tagTypes.Add(name, instance as ITag);

                if (instance is IHierarchyTag)  // Optional variant.
                    hierarchyTypes.Add(name, instance as IHierarchyTag);
            }
            else if (instance is IRichTag)
            {
                richTagTypes.Add(ReplaceLastOccurrence(instance.GetType().Name, "RichTag", "").ToSnakeCase(), new DialogueManager.RichTagData { Execution = instance as IRichTag });
            }
        }
    }
}
