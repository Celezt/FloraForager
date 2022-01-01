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
    /// Extract all custom rich tag and return a queue with the value and range of effect.
    /// </summary>
    public static Queue<RichTag> DeserializeRichTag(StringBuilder text, Dictionary<string, IRichTag> richTagTypes)
    {
        Queue<RichTag> outRichTagQueue = new Queue<RichTag>();
        Stack<RichTag> richTagStack = new Stack<RichTag>();
        string copy = text.ToString();
        bool insideTag = false;
        int length = 0; // Length without any tags or other descriptive information.
        int offset = 0; // How much is removed.

        for (int i = 0; i < copy.Length; i++)
        {
            if (new char[] { '{', '}' }.Contains(copy[i]))
                continue;

            if (copy[i] == '<')
                insideTag = true;
            else if (copy[i] == '>')
                insideTag = false;
            else if (insideTag) // If inside a rich tag.
            {
                int endClampIndex = copy.IndexOf('>', i);
                int parameterIndex = copy.IndexOf('=', i, endClampIndex - i);   // Optional
                string name = copy.Substring(i, (parameterIndex != -1 ? parameterIndex : endClampIndex) - i);

                if (name.IsNullOrEmpty())
                    throw new DialogueException("No name was found inside <?> at " + i);

                if (endClampIndex == -1)
                    throw new DialogueException("Rich tag is missing a > at " + i);

                bool isEndClamp = name[0] == '/';
                name = name.TrimStart('/');

                if (richTagTypes.TryGetValue(name, out IRichTag richTagBehaviour))  // If rich tag exist.
                {
                    if (isEndClamp)
                    {
                        RichTag richTag = richTagStack.Pop();
                        richTag.Range = new RangeInt(richTag.Range.start, length - richTag.Range.start);

                        if (richTag.Name == name)
                            outRichTagQueue.Enqueue(richTag);
                        else
                            throw new DialogueException($"{name} is missing a start");
                    }
                    else
                    {
                        richTagStack.Push(new RichTag
                        {
                            Name = name,
                            Behaviour = richTagBehaviour,
                            Range = new RangeInt(length, 0),
                            Parameter = parameterIndex != -1 ? copy.Substring(parameterIndex + 1, endClampIndex - parameterIndex - 1) : ""
                        });
                    }

                    text.Remove(i - offset - 1, endClampIndex - i + 2);
                    offset += endClampIndex - i + 2;
                }

                i = endClampIndex;
                insideTag = false;
            }
            else
                length++;
        }

        if (richTagStack.Count > 0)
            throw new DialogueException("Not all rich tags has a ending");

        return outRichTagQueue;
    }

    /// <summary>
    /// Extract all custom event tag and return a queue with the value and index.
    /// </summary>
    public static Queue<EventTag> DeserializeEventTag(StringBuilder text, Dictionary<string, IEventTag> eventTagTypes)
    {
        Queue<EventTag> outEventTagQueue = new Queue<EventTag>();
        string copy = text.ToString();
        bool insideTag = false;
        int length = 0; // Length without any tags or other descriptive information.
        int offset = 0; // How much is removed.

        for (int i = 0; i < copy.Length; i++)
        {
            if (new char[] { '{', '}' }.Contains(copy[i]))
                continue;

            if (copy[i] == '<')
                insideTag = true;
            else if (copy[i] == '>')
                insideTag = false;
            else if (insideTag) // If inside a rich tag.
            {
                int endClampIndex = copy.IndexOf('>', i);
                int parameterIndex = copy.IndexOf('=', i, endClampIndex - i);   // Optional
                string name = copy.Substring(i, (parameterIndex != -1 ? parameterIndex : endClampIndex) - i);

                if (name.IsNullOrEmpty())
                    throw new DialogueException("No name was found inside <?> at " + i);

                if (endClampIndex == -1)
                    throw new DialogueException("Event tag is missing a > at " + i);

                if (eventTagTypes.TryGetValue(name, out IEventTag richTagBehaviour))  // If rich tag exist.
                {
                    outEventTagQueue.Enqueue(new EventTag
                    {
                        Name = name,
                        Behaviour = richTagBehaviour,
                        Index = length,
                        Parameter = parameterIndex != -1 ? copy.Substring(parameterIndex + 1, endClampIndex - parameterIndex - 1) : ""
                    });

                    text.Remove(i - offset - 1, endClampIndex - i + 2);
                    offset += endClampIndex - i + 2;
                }

                i = endClampIndex;
                insideTag = false;
            }
            else
                length++;
        }

        return outEventTagQueue;
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
                throw new DialogueException($"{tag} does not exist");

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
