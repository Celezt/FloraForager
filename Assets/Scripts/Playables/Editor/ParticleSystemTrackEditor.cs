using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;

namespace Celezt.Timeline
{
    [CustomEditor(typeof(ParticleSystemTrack)), CanEditMultipleObjects]
    public class ParticleSystemTrackEditor : Editor
    {

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
