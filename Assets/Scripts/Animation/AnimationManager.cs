using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MyBox;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "AnimationManager", menuName = "Game Logic/ Animation Manager")]
public class AnimationManager : SerializedScriptableSingleton<AnimationManager>
{
#if UNITY_EDITOR
    [Button]
    private void FindAllClips()
    {
        string[] paths = AssetDatabase.FindAssets($"t:{nameof(AnimationClip)}", new string[] {"Assets/Animations/Clips"}).Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();

        foreach (string path in paths)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);

            if (!(assetImporter is ModelImporter))
                continue;

            ModelImporter modelImporter = assetImporter as ModelImporter;

            foreach (var item in modelImporter.clipAnimations)
            {
                Debug.Log(item.name);
            }
        }

    }
#endif
}
