using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "Dialogue Animations", menuName = "Game Logic/Dialogue Animations")]
[System.Serializable]
public class DialogueAnimations : SerializedScriptableSingleton<DialogueAnimations>
{
    [OdinSerialize]
    private Dictionary<string, AnimationClip> _clips;
    
    public AnimationClip Get(string id) => _clips[id];
}
