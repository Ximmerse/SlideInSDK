using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace Ximmerse.SlideInSDK
{
    [CustomEditor(typeof(ARCamera), true)]
    public class ARCameraInspector : Editor
    {
        [MenuItem("GameObject/Create Other/Ximmerse/Slide In SDK/AR Camera")]
        static void CreateARCameraInScene()
        {
            GameObject ARCameraObject = new GameObject("AR Camera", new System.Type[] 
            {
                typeof(Camera),typeof(ARCamera), typeof(TagTracker), typeof(AudioListener),
            });

            ARCamera arCamera = ARCameraObject.GetComponent<ARCamera>();
            arCamera.GetType().GetMethod("InitARCamera_Editor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(arCamera, null); 

            Selection.activeTransform = ARCameraObject.transform;
            EditorGUIUtility.PingObject(ARCameraObject);
        }

        ARCamera arCamera { get { return this.target as ARCamera; } }
        SerializedProperty m_ScriptProp = null;
        SerializedProperty m_trackingModeProp = null;

        void OnEnable()
        {
            m_ScriptProp = this.serializedObject.FindProperty("m_Script");
            m_trackingModeProp = serializedObject.FindProperty("m_HeadMode");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.PropertyField(m_ScriptProp, true);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(m_trackingModeProp);

            var _renderingMode = (ARCamera.StereoRenderingMode)EditorGUILayout.EnumPopup ("Rendering Mode", arCamera.RenderingMode);
            if(_renderingMode != arCamera.RenderingMode)
            {
                arCamera.RenderingMode = _renderingMode;
            }

            if(arCamera.RenderingMode == ARCamera.StereoRenderingMode.Stereo)
            {
                //Properties only works under stereo mode:
                var _covengenceMode = (ARCamera.EyeCovengenceRenderingMode)EditorGUILayout.EnumPopup ("Eye Covergence Mode", arCamera.EyeCovergenceMode);
                if(_covengenceMode != arCamera.EyeCovergenceMode)
                {
                    arCamera.EyeCovergenceMode = _covengenceMode;
                }

                if(arCamera.EyeCovergenceMode != ARCamera.EyeCovengenceRenderingMode.Infinity)
                {
                    float value = EditorGUILayout.Slider(arCamera.EyeCovergenceDistance, 0.1f, 30f);
                    if(value != arCamera.EyeCovergenceDistance)
                    {
                        arCamera.EyeCovergenceDistance = value;
                    }
                }
            }

            if(Application.isPlaying)
            {
                if(arCamera.RenderingMode == ARCamera.StereoRenderingMode.Stereo) 
                {
                    var _EyeSeparation = EditorGUILayout.Vector3Field("Eye separation", arCamera.EyeSeparation);
                    if(_EyeSeparation != arCamera.EyeSeparation)
                    {
                        arCamera.EyeSeparation = _EyeSeparation;
                    }
                }

                var _UndistortionMeshPanOffset = EditorGUILayout.Vector3Field("Undistortion-Mesh Pan", arCamera.UndistortionMeshPanOffset);
                if(_UndistortionMeshPanOffset != arCamera.UndistortionMeshPanOffset)
                {
                    arCamera.UndistortionMeshPanOffset = _UndistortionMeshPanOffset;
                }

                var _UndistortionMeshEuler = EditorGUILayout.Vector3Field("Undistortion-Euler", arCamera.UndistortionMeshEuler);
                if(_UndistortionMeshEuler != arCamera.UndistortionMeshEuler)
                {
                    arCamera.UndistortionMeshEuler = _UndistortionMeshEuler;
                }

                var _UndistortionMeshScale = EditorGUILayout.Vector3Field("Undistortion-Scale", arCamera.UndistortionMeshScale);
                if(_UndistortionMeshScale != arCamera.UndistortionMeshScale)
                {
                    arCamera.UndistortionMeshScale = _UndistortionMeshScale;
                }

                var _hFov = EditorGUILayout.FloatField("hFov", arCamera.hFov);
                if(_hFov != arCamera.hFov)
                {
                    arCamera.hFov = _hFov;
                }

                var _vFov = EditorGUILayout.FloatField("vFov", arCamera.vFov);
                if(_vFov != arCamera.vFov)
                {
                    arCamera.vFov = _vFov;
                }

                var _fovScale = EditorGUILayout.Slider("Fov Scale", arCamera.fovScale, 0.1f, 5);
                if(_fovScale != arCamera.fovScale)
                {
                    arCamera.fovScale = _fovScale;
                }

            }
            this.serializedObject.ApplyModifiedProperties ();
        }

    }
}