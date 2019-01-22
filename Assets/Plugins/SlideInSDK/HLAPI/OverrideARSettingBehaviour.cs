using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Override AR setting behaviour. 
    /// This scripts attach to AR camera and override the default parameters on the both components.
    /// This script should be a singleton in scene.
    /// </summary>
    [AddComponentMenu("Ximmerse/Slide-In/Override AR Setting Behaviour")]
    public class OverrideARSettingBehaviour : MonoBehaviour
    {
        static OverrideARSettingBehaviour instance;

        public static OverrideARSettingBehaviour Instance
        {
            get
            {
                if(!instance)
                {
                    instance = FindObjectOfType<OverrideARSettingBehaviour>();
                }
                return instance;
            }
        }

        ARCamera arCamera;
        TagTracker tagTracker;

        [SerializeField, Tooltip("Should apply overrided AR setting ?")]
        bool m_ApplyOverridedARSetting;

        /// <summary>
        /// Should apply override AR setting ?
        /// </summary>
        /// <value><c>true</c> if apply override setting; otherwise, <c>false</c>.</value>
        public bool ApplyOverridedARSetting
        {
            get
            {
                return m_ApplyOverridedARSetting;
            }
            set
            {
                m_ApplyOverridedARSetting = value;
            }
        }

        [SerializeField]
        OverrideARSetting m_OverrideARSetting;

        public OverrideARSetting OverrideARSetting
        {
            get
            {
                return m_OverrideARSetting;
            }
            set
            {
                m_OverrideARSetting = value;
            }
        }

        // Use this for initialization
        void Awake()
        {
            instance = this;
        }
	 
        IEnumerator Start ()
        {
            yield return new WaitForEndOfFrame ();//execution after ARCamera.Start() and TagTracker.Start()
            arCamera = ARCamera.Singleton;
            tagTracker = TagTracker.Singleton;
            ApplyOverrideSetting ();
        }

        /// <summary>
        /// Removes the override setting, apply default setting.
        /// </summary>
        [ContextMenu ("Remove Overrided & Apply Default")]
        public void RevertToDefaultSetting ()
        {
            //Apply override setting to ARCamera:
            this.arCamera.RevertToDefaultSetting();
            //Apply override setting to tag tracker:
            this.tagTracker.RevertToDefaultSetting ();
        }

        /// <summary>
        /// Applies the override setting to scene ARCamera and TagTracker component.
        /// </summary>
        [ContextMenu ("Apply Override")]
        public void ApplyOverrideSetting ()
        {
            if(OverrideARSetting == null)
                return;

            var overridedSetting = this.OverrideARSetting;
            //Apply override setting to ARCamera:
            {
                var cam = this.arCamera;
                if(overridedSetting.hFov.HasValue)
                {
                    cam.hFov = overridedSetting.hFov.Value;
                    Debug.LogFormat ("Override hFov => {0}", overridedSetting.hFov.Value);
                }
                if(overridedSetting.vFov.HasValue)
                {
                    cam.vFov = overridedSetting.vFov.Value;
                    Debug.LogFormat ("Override vFov => {0}", overridedSetting.vFov.Value);
                }
                if(overridedSetting.EyeSeparation.HasValue)
                {
                    cam.EyeSeparation = new Vector3(overridedSetting.EyeSeparation.Value, 0, 0);
                    Debug.LogFormat ("Override EyeSeparation => {0}", overridedSetting.EyeSeparation.Value);
                }
                if(overridedSetting.UndistortionMeshViewDirection.HasValue)
                {
                    cam.UndistortionMeshEuler = overridedSetting.UndistortionMeshViewDirection.Value;
                    Debug.LogFormat ("Override UndistortionMeshEuler => {0}", overridedSetting.UndistortionMeshViewDirection.Value);
                }
                if(overridedSetting.UndistortionMeshOffset.HasValue)
                {
                    cam.UndistortionMeshPanOffset = overridedSetting.UndistortionMeshOffset.Value;
                    Debug.LogFormat ("Override UndistortionMeshPanOffset => {0}", overridedSetting.UndistortionMeshOffset.Value);
                }
                if (overridedSetting.LeftUndistortionMeshOffset.HasValue)
                {
                    cam.LeftUndistortionMeshPanOffset = overridedSetting.LeftUndistortionMeshOffset.Value;
                    Debug.LogFormat("Override LeftUndistortionMeshPanOffset => {0}", overridedSetting.LeftUndistortionMeshOffset.Value);
                }
                if (overridedSetting.RightUndistortionMeshOffset.HasValue)
                {
                    cam.RightUndistortionMeshPanOffset = overridedSetting.RightUndistortionMeshOffset.Value;
                    Debug.LogFormat("Override RightUndistortionMeshPanOffset => {0}", overridedSetting.RightUndistortionMeshOffset.Value);
                }
                if (overridedSetting.UndistortionMeshScale.HasValue)
                {
                    cam.UndistortionMeshScale = overridedSetting.UndistortionMeshScale.Value;
                    Debug.LogFormat ("Override UndistortionMeshScale => {0}", overridedSetting.UndistortionMeshScale.Value);
                }
                if(overridedSetting.UndistortionMesh != null)
                {
                    cam.SetUndistortionMesh (overridedSetting.UndistortionMesh);
                    Debug.LogFormat ("Override unDistortionMesh => {0}, total vertice :{1}", overridedSetting.UndistortionMesh.name, overridedSetting.UndistortionMesh.vertexCount);
                }
            }

            //Apply override setting to tag tracker:
            {
                var tracker = this.tagTracker;
                if(overridedSetting.TrackingAnchor_Positional_Offset.HasValue)
                {
                    tracker.TrackingAnchorPosition = overridedSetting.TrackingAnchor_Positional_Offset.Value;
                }

                if(overridedSetting.TrackingAnchor_Euler_Offset.HasValue)
                {
                    tracker.TrackingAnchorTilt = overridedSetting.TrackingAnchor_Euler_Offset.Value;
                }

                if(overridedSetting.TrackingAnchor_Scale.HasValue)
                {
                    tracker.TrackingAnchorScale = overridedSetting.TrackingAnchor_Scale.Value;
                }

                if(overridedSetting.RawPositionIndex.HasValue)
                {
                    TagTrackingUtil.RawPositionindex = overridedSetting.RawPositionIndex.Value;
                }

                if(overridedSetting.RawRotationIndex.HasValue)
                {
                    TagTrackingUtil.RawRotationIndex = overridedSetting.RawRotationIndex.Value;
                }

                if(overridedSetting.RawPositionFieldMultiplier.HasValue)
                {
                    TagTrackingUtil.RawPositionFieldMultiplier = overridedSetting.RawPositionFieldMultiplier.Value;
                }

                if(overridedSetting.RawRotationFieldMultiplier.HasValue)
                {
                    TagTrackingUtil.RawRotationFieldMultiplier = overridedSetting.RawRotationFieldMultiplier.Value;
                }

                if(overridedSetting.MarkerPosePreTilt.HasValue)
                {
                    TagTrackingUtil.MarkerPosePreTilt = Quaternion.Euler (overridedSetting.MarkerPosePreTilt.Value.x,
                        overridedSetting.MarkerPosePreTilt.Value.y, overridedSetting.MarkerPosePreTilt.Value.z);
                }

                if(overridedSetting.MarkerPosePostTilt.HasValue)
                {
                    TagTrackingUtil.MarkerPosePostTilt = Quaternion.Euler (overridedSetting.MarkerPosePostTilt.Value.x,
                        overridedSetting.MarkerPosePostTilt.Value.y, overridedSetting.MarkerPosePostTilt.Value.z);
                }
            }

        }
    }
}