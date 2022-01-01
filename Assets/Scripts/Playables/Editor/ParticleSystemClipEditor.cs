using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;

namespace Celezt.Timeline
{
    [CustomEditor(typeof(ParticleSystemAsset)), CanEditMultipleObjects]
    public class ParticleSystemClipEditor : Editor
    {
        private SerializedProperty _text;
        private SerializedProperty _fontColor;

        private void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
