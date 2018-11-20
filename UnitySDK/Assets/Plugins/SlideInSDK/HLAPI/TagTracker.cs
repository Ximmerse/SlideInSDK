using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDevicePlugin = Ximmerse.InputSystem.XDevicePlugin;
using Path = System.IO.Path;


namespace Ximmerse.SlideInSDK
{
    public class TagTracker : MonoBehaviour
    {
        static TagTracker singleton;

        public static TagTracker Singleton
        {
            get
            {
                return singleton;
            }
        }

        Transform m_TrackingAnchor;

        public static Transform TrackingAnchor
        {
            get
            {
                return Singleton.m_TrackingAnchor;
            }
        }

        [SerializeField]
        Vector3 m_TrackingAnchorPosition, m_TrackingAnchorTilt, m_TrackingAnchorScale;

        /// <summary>
        /// Gets or sets the tracking anchor position.
        /// tracking anchor position is a XYZ value that defines the local position of the tracking anchor node.
        /// </summary>
        /// <value>The tracking anchor position.</value>
        public Vector3 TrackingAnchorPosition
        {
            get
            {
                return m_TrackingAnchorPosition;
            }
            set
            {
                m_TrackingAnchorPosition = value;
                OnTrackingAnchorConfigDirty();
            }
        }

        /// <summary>
        /// Gets or sets the tracking anchor tilt.
        /// tracking anchor tilt is a euler that defines the local euler of the tracking anchor node.
        /// Change this value could cause a significant impact on AR object alignment
        /// </summary>
        /// <value>The tracking anchor tilt.</value>
        public Vector3 TrackingAnchorTilt
        {
            get
            {
                return m_TrackingAnchorTilt;
            }
            set
            {
                m_TrackingAnchorTilt = value;
                OnTrackingAnchorConfigDirty();
            }
        }

        /// <summary>
        /// Gets or sets the tracking anchor scale. this is the scale the tracking anchor.
        /// Change this value could cause a significant impact on AR object alignment
        /// </summary>
        /// <value>The tracking anchor scale.</value>
        public Vector3 TrackingAnchorScale
        {
            get
            {
                return m_TrackingAnchorScale;
            }
            set
            {
                m_TrackingAnchorScale = value;
                OnTrackingAnchorConfigDirty();
            }
        }

        Vector3 m_MarkerPosePreTilt = Vector3.zero;

        public Vector3 MarkerPosePreTilt
        {
            get
            {
                return m_MarkerPosePreTilt;
            }
            set 
            {
                if(m_MarkerPosePreTilt != value)
                {
                    m_MarkerPosePreTilt = value;
                    TagTrackingUtil.MarkerPosePreTilt = Quaternion.Euler (value);
                }
            }
        }


        Vector3 m_MarkerPosePostTilt = Vector3.zero;

        public Vector3 MarkerPosePostTilt
        {
            get
            {
                return m_MarkerPosePostTilt;
            }
            set 
            {
                if(m_MarkerPosePostTilt != value)
                {
                    m_MarkerPosePostTilt = value;
                    TagTrackingUtil.MarkerPosePostTilt = Quaternion.Euler (value);
                }
            }
        }

        [SerializeField]
        MarkerTrackingProfile m_DefaultTrackingProfile;

        /// <summary>
        /// The current tracking profile.
        /// </summary>
        MarkerTrackingProfile m_CurrentTrackingProfile;

        public MarkerTrackingProfile CurrentTrackingProfile
        {
            get
            {
                return m_CurrentTrackingProfile;
            }
        }

        void Awake()
        {
            singleton = this;
            InitTagTrackingModule();
        }

        void Start()
        {
            if (m_DefaultTrackingProfile != null)
            {
                LoadMarkerTrackingProfile(m_DefaultTrackingProfile);
            }
        }

        /// <summary>
        /// Inits the tag tracking module.
        /// </summary>
        void InitTagTrackingModule()
        {
            //Create tracking reference anchor:
            m_TrackingAnchorPosition = TrackingConfig.TrackingAnchor_Positional_Offset;
            m_TrackingAnchorTilt = TrackingConfig.TrackingAnchor_Euler_Offset;
            m_TrackingAnchorScale = TrackingConfig.TrackingAnchor_Scale;

            this.m_MarkerPosePreTilt = TrackingConfig.MarkerPosePreTilt;
            this.m_MarkerPosePostTilt = TrackingConfig.MarkerPosePostTilt;

            m_TrackingAnchor = new GameObject("Tracking-Anchor").transform;
            m_TrackingAnchor.transform.SetParent(this.transform, false);
            OnTrackingAnchorConfigDirty();

            TagTrackingUtil.InitializeDeviceModule();

            Debug.LogFormat("Tag tracking module init done, tracking anchor: {0}-{1}-{2} ", m_TrackingAnchorPosition, m_TrackingAnchorTilt, m_TrackingAnchorScale);
        }

        void OnTrackingAnchorConfigDirty()
        {
            m_TrackingAnchor.localPosition = m_TrackingAnchorPosition;
            m_TrackingAnchor.localEulerAngles = m_TrackingAnchorTilt;
            m_TrackingAnchor.localScale = m_TrackingAnchorScale;
        }

        public void LoadMarkerTrackingProfile(MarkerTrackingProfile profile)
        {
            if (TagTrackingUtil.IsSupported)
            {
                XDevicePlugin.DoAction(DevicerHandle.HmdHandle, XDevicePlugin.XActions.kXAct_ResetMarkerSettings); ///清除配置
                foreach (var jsonItem in profile.trackingItems)
                {
                    //Load json:
                    string jsonPath = Path.Combine(TagTrackingUtil.kConfigFileDirectory, jsonItem.jsonName);
                    int loadResult = XDevicePlugin.DoAction(DevicerHandle.HmdHandle, XDevicePlugin.XActions.kXAct_LoadMarkerSettingFile, jsonPath);  
                    Debug.LogFormat("Load json: {0} result : {1}", jsonPath, loadResult);
                }
            } 
            m_CurrentTrackingProfile = profile;

        }



        /// <summary>
        /// Reverts setting to the default setting.
        /// </summary>
        [ContextMenu("Remove Overrided & Apply Default")]
        public void RevertToDefaultSetting()
        {
            var tagTracker = this;
            tagTracker.TrackingAnchorPosition = TrackingConfig.TrackingAnchor_Positional_Offset;
            tagTracker.TrackingAnchorTilt = TrackingConfig.TrackingAnchor_Euler_Offset;
            tagTracker.TrackingAnchorScale = TrackingConfig.TrackingAnchor_Scale;

            TagTrackingUtil.ApplyDefaultConfig();
        }

    }
}