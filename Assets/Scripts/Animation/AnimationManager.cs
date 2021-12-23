using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "AnimationManager", menuName = "Game Logic/ Animation Manager")]
public class AnimationManager : SerializedScriptableSingleton<AnimationManager>
{
    public IReadOnlyDictionary<string, AnimationClip> Clips => _clips;

    [OdinSerialize, ReadOnly]
    private Dictionary<string, AnimationClip> _clips;

#if UNITY_EDITOR
    public void Set(AnimationClip clip)
    {
        if (clip == null)
            return;

        string name = clip.name.Substring(clip.name.LastIndexOf('|') + 1).ToSnakeCase();

        if (!_clips.ContainsKey(name))
            _clips.Add(name, clip);

        EditorUtility.SetDirty(this);
    }

    [Button]
    public void FindAllClips()
    {
        _clips = new Dictionary<string, AnimationClip>();

        string[] paths = AssetDatabase.FindAssets($"t:{nameof(AnimationClip)}", new string[] {"Assets/Animations/Clips"}).Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();

        foreach (string path in paths)
        {
            AnimationClip[] clips = AssetDatabase.LoadAllAssetsAtPath(path).Select(x => x as AnimationClip).ToArray();

            foreach (AnimationClip clip in clips)
            {
                if (clip == null)
                    continue;

                string name = clip.name.Substring(clip.name.LastIndexOf('|') + 1).ToSnakeCase();

                if (!_clips.ContainsKey(name))
                    _clips.Add(name, clip);
            }
        }

        EditorUtility.SetDirty(this);
    }
#endif
}
