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
        public string Parameter;
        public ITag Behaviour;

        bool IEquatable<Tag>.Equals(Tag other) => other.Name == Name;
        public override int GetHashCode() => Name.GetHashCode();
        public override string ToString() => Name;
    }

    public struct EventTag : IEquatable<EventTag>
    {
        public string Name;
        public string Parameter;
        public int Index;
        public IEventTag Behaviour;

        bool IEquatable<EventTag>.Equals(EventTag other) => other.Name == Name;
        public override int GetHashCode() => Name.GetHashCode();
        public override string ToString() => Name;
    }

    public struct RichTag : IEquatable<RichTag>
    {
        public string Name;
        public string Parameter;
        public IRichTag Behaviour;
        public RangeInt Range;

        bool IEquatable<RichTag>.Equals(RichTag other) => other.Name == Name;
        public override int GetHashCode() => Name.GetHashCode();
        public override string ToString() => Name;
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
    /// Try extract custom rich tag and return a queue with the value and range of effect.
    /// </summary>
    /// <param name="text">Parsed text.</param>
    /// <param name="richTag">Rich tag to find.</param>
    /// <returns>If any exist.</returns>
    public static bool TryDeserializeRichTag(StringBuilder text, string richTag, Dictionary<string, IRichTag> richTagTypes, out Queue<RichTag> queue)
    {
        string copy = text.ToString();
        queue = new Queue<RichTag>();
        Regex openRegex = new Regex($"<{richTag}(=((?:.(?!\\1|>))*.?)\\1?)?>");
        Regex closeRegex = new Regex($"((<\\/){richTag}(>))");

        MatchCollection openMatches = openRegex.Matches(text.ToString());
        MatchCollection closeMatches = closeRegex.Matches(text.ToString());

        if (openMatches.Count != closeMatches.Count)
            throw new DialogueException($"Uneven amount of open and close tags");

        if (openMatches.Count <= 0)
            return false;

        for (int i = 0; i < openMatches.Count; i++)
        {
            int openStartIndex = openMatches[i].Groups[0].Index + openMatches[i].Groups[0].Length;
            int closeStartIndex = closeMatches[i].Groups[0].Index;

            bool insideTag = false;
            int closedIndex = 0;
            int openIndex = 0;
            for (int j = 0; j < closeStartIndex; j++)
            {
                if (new char[] { '{', '}' }.Contains(copy[j]))
                    continue;

                if (copy[j] == '<')
                    insideTag = true;
                else if (copy[j] == '>')
                    insideTag = false;
                else if (!insideTag)
                {
                    closedIndex++;

                    if (j < openStartIndex)
                        openIndex++;
                }
            }

            queue.Enqueue(new RichTag { Name = richTag, Parameter = openMatches[i].Groups[2].Value, Behaviour = richTagTypes[richTag], Range = new RangeInt(openIndex, closedIndex - openIndex) });
            text.Replace(openMatches[i].Groups[0].Value, "");
            text.Replace(closeMatches[i].Groups[0].Value, "");
        }

        return true;
    }

    /// <summary>
    /// Try extract custom event tag and return a queue with the value and index.
    /// </summary>
    /// <param name="text">Parsed text.</param>
    /// <param name="eventTag">Event tag to find.</param>
    /// <returns>If any exist.</returns>
    public static bool TryDeserializeEventTag(StringBuilder text, string eventTag, Dictionary<string, IEventTag> eventTagTypes, out Queue<EventTag> queue)
    {
        string copy = text.ToString();
        queue = new Queue<EventTag>();
        Regex regex = new Regex($"<{eventTag}(=((?:.(?!\\1|>))*.?)\\1?)?>");

        MatchCollection matches = regex.Matches(text.ToString());

        if (matches.Count <= 0)
            return false;

        for (int i = 0; i < matches.Count; i++)
        {
            int endIndex = matches[i].Groups[0].Index;

            int index = 0;
            bool insideTag = false;
            for (int j = 0; j < endIndex; j++)
            {
                if (new char[] { '{', '}' }.Contains(copy[j]))
                    continue;

                if (copy[j] == '<')
                    insideTag = true;
                else if (copy[j] == '>')
                    insideTag = false;
                else if (!insideTag)
                {
                    index++;
                }
            }

            queue.Enqueue(new EventTag { Name = eventTag, Parameter = matches[i].Groups[2].Value, Index = index, Behaviour = eventTagTypes[eventTag] });

            text.Replace(matches[i].Groups[0].Value, "");
        }

        return true;
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

            return new Tag { Name = tag, Behaviour = tagTypes[tag], Parameter = parameter };
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

    public class DialogueException : Exception
    {
        public DialogueException() { }
        public DialogueException(string message) : base(message) { }
        public DialogueException(string message, Exception inner) : base(message, inner) { }
    }
}
