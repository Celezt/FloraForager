using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnimationPostProcessor : AssetPostprocessor
{
    private void OnPostprocessAnimation(GameObject root, AnimationClip clip)
    {
        AnimationManager.Instance.Set(clip);
    }
}
