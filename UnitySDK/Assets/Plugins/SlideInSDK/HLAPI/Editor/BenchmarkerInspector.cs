using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Benchmarker inspector.
    /// </summary>
    [CustomEditor (typeof(BenchMarker), true)]
    public class BenchmarkerInspector : MarkerTargetBehaviourInspector
    {
        BenchMarker tScript { get { return this.target as BenchMarker; } }

        SerializedProperty m_FixedErrorProperty = null;

        void OnEnable ()
        {
            m_FixedErrorProperty = serializedObject.FindProperty("m_FixedQuaternionError");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(m_FixedErrorProperty, new GUIContent ("Fixed Rotation"), true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}