using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ximmerse.SlideInSDK
{
    [CustomEditor(typeof(TagTracker))]
    public class TagTrackerInspector : Editor
    {
        TagTracker tagTracker { get { return this.target as TagTracker; } }

        SerializedProperty m_ScriptProp = null;
        SerializedProperty m_DefaultTrackingProfileProp = null;

        void OnEnable()
        {
            m_ScriptProp = this.serializedObject.FindProperty("m_Script");
            m_DefaultTrackingProfileProp = this.serializedObject.FindProperty("m_DefaultTrackingProfile");
        }

        bool isDirty = false;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.PropertyField(m_ScriptProp, true);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(m_DefaultTrackingProfileProp, new GUIContent ("Tag Profile"), true);

            if(Application.isPlaying)
            {
                var _Shift = EditorGUILayout.Vector3Field("Tracking Anchor Shift", tagTracker.TrackingAnchorPosition);
                if(_Shift != tagTracker.TrackingAnchorPosition)
                {
                    tagTracker.TrackingAnchorPosition = _Shift;
                    isDirty = true;
                }

                var _Tilt = EditorGUILayout.Vector3Field("Tracking Anchor Tilt", tagTracker.TrackingAnchorTilt);
                if(_Tilt != tagTracker.TrackingAnchorTilt)
                {
                    tagTracker.TrackingAnchorTilt = _Tilt;
                    isDirty = true;
                }

                var _Scale = EditorGUILayout.Vector3Field("Tracking Anchor Scale", tagTracker.TrackingAnchorScale);
                if(_Scale != tagTracker.TrackingAnchorScale)
                {
                    tagTracker.TrackingAnchorScale = _Scale;
                    isDirty = true;
                }

                var _PreTilt = EditorGUILayout.Vector3Field("Marker Pose Pre-Tilt", tagTracker.MarkerPosePreTilt);
                if(_PreTilt != tagTracker.MarkerPosePreTilt)
                {
                    tagTracker.MarkerPosePreTilt = _PreTilt;
                    isDirty = true;
                }

                var _PostTilt = EditorGUILayout.Vector3Field("Marker Pose Post-Tilt", tagTracker.MarkerPosePostTilt);
                if(_PostTilt != tagTracker.MarkerPosePostTilt)
                {
                    tagTracker.MarkerPosePostTilt = _PostTilt;
                    isDirty = true;
                }

                if(isDirty)
                {
                    if(GUILayout.Button("Save Setting"))
                    {
                        TrackingConfig.TrackingAnchor_Positional_Offset = _Shift;
                        TrackingConfig.TrackingAnchor_Euler_Offset = _Tilt;
                        TrackingConfig.TrackingAnchor_Scale = _Scale;

                        TrackingConfig.MarkerPosePreTilt = _PreTilt;
                        TrackingConfig.MarkerPosePostTilt = _PostTilt;

                        EditorUtility.SetDirty(TrackingConfig.Singleton);
                    }
                }
            }

            this.serializedObject.ApplyModifiedProperties ();
        }
    }
}