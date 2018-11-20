using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using XDevicePlugin = Ximmerse.InputSystem.XDevicePlugin;
using PolyEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Denoting an AR virutal camera behavior in unity scene.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Camera))]
    [AddComponentMenu("Ximmerse/Slide-In/ARCamera")]
    public partial class ARCamera : MonoBehaviour 
    {
        #region Public Fields 

        public enum StereoRenderingMode 
        {
            /// <summary>
            /// The default mode, eyes cameras are created. Main camera is disable.
            /// </summary>
            Stereo = 0,

            /// <summary>
            /// Common single camera mode, typically usage for desktop/TV OS application.
            /// </summary>
            Single = 1,
        }

        [SerializeField, Tooltip ("Rendering mode")]
        StereoRenderingMode m_StereoRenderingMode = StereoRenderingMode.Stereo;

        /// <summary>
        /// Gets or sets the rendering mode.
        /// </summary>
        /// <value>The rendering mode.</value>
        public StereoRenderingMode RenderingMode
        {
            get
            {
                return m_StereoRenderingMode;
            }
            set
            {
                m_StereoRenderingMode = value;
                if(isInitialized)
                {
                    OnRenderingModeChanged ();
                }
            }
        }

        static ARCamera singleton;

        /// <summary>
        /// Singleton of ARCamera (readonly)
        /// </summary>
        /// <value>The singleton.</value>
        public static ARCamera Singleton
        {
            get
            {
                return singleton;
            }
        }

        /// <summary>
        /// Eye node.
        /// </summary>
        public enum EyeNode 
        {
            Middle = 0,

            Left = 1,

            Right = 2,
        }

        /// <summary>
        /// Eye covengence rendering mode.
        /// </summary>
        public enum EyeCovengenceRenderingMode 
        {
            /// <summary>
            /// Eye covergence in infinity far away.
            /// </summary>
            Infinity = 0,

            /// <summary>
            /// eye covergence in a certain distance, but in cross viewport plane
            /// </summary>
            CrossPlane = 1,
        }

        [SerializeField]
        EyeCovengenceRenderingMode m_EyeCovergenceMode = EyeCovengenceRenderingMode.Infinity;

        /// <summary>
        /// Gets or sets the eye covengence mode.
        /// </summary>
        /// <value>The eye covengence mode.</value>
        public EyeCovengenceRenderingMode EyeCovergenceMode
        {
            get
            {
                return m_EyeCovergenceMode;
            }
            set
            {
                m_EyeCovergenceMode = value;
                if(isInitialized)
                {
                    this.UpdateEyeCovengence ();
                }
            }
        }

        [SerializeField, Range(0.1f,10)]
        float m_EyeCovergenceDistance = 5;

        /// <summary>
        /// Gets the eye covergence distance.
        /// </summary>
        /// <value>The eye covergence distance.</value>
        public float EyeCovergenceDistance
        {
            get
            {
                return m_EyeCovergenceDistance;
            }
            set 
            {
                m_EyeCovergenceDistance = value;
                if(isInitialized)
                {
                    this.UpdateEyeCovengence ();
                }
            }
        }

        /// <summary>
        /// The tracking mode.
        /// </summary>
        [SerializeField]
        ARCameraTrackingMode m_HeadMode = ARCameraTrackingMode.RotateLocally;

        public ARCameraTrackingMode HeadMode
        {
            get
            {
                return m_HeadMode;
            }
            set
            {
                m_HeadMode = value;
            }
        }

        Vector3 m_UndistortionMeshScale = Vector3.one;

        /// <summary>
        /// Gets or sets the undistortion mesh scale.
        /// </summary>
        /// <value>The undistortion mesh scale.</value>
        public Vector3 UndistortionMeshScale 
        {
            get 
            {
                return m_UndistortionMeshScale;
            }
            set 
            {
                m_UndistortionMeshScale = value;
                if(this.isInitialized && this.leftEye != null)
                {
                    this.leftEye.GetComponent<StereoEyeRenderer>().UndistortionMeshScale = value;
                    this.rightEye.GetComponent<StereoEyeRenderer>().UndistortionMeshScale = value;
                }
            }
        }

        #endregion


        #region Private fields

        /// <summary>
        /// The rotation recorded at Awake()
        /// </summary>
        Quaternion awakeRotation = Quaternion.identity;

        /// <summary>
        /// For recenter compenstation
        /// </summary>
        Quaternion m_RotationCompensatedError = Quaternion.identity;

        /// <summary>
        /// left and right eye.
        /// </summary>
        public Camera leftEye,rightEye, mainCamera;

        #endregion


        bool isHeadTrackingSupport;
        bool isInitialized = false;

        void Awake()
        {
            singleton = this;
            Application.targetFrameRate = 60;
            InitializeHeadTracking();
            InitializeCameraInstances();
            isInitialized = true;
        }

        void Start()
		{
            this.OnRenderingModeChanged();
		}

		/// <summary>
		/// Event: left-right camera is created. 
		/// </summary>
		public static event System.Action<Camera, Camera> OnCameraIsCreated = null;

        [SerializeField]
        Vector3 m_EyeSeparation;

        /// <summary>
        /// Gets or sets the eye separation distance.
        /// </summary>
        /// <value>The eye separation.</value>
        public Vector3 EyeSeparation { 
            get { return m_EyeSeparation; } 
            set { 
                m_EyeSeparation = value; 
                if(isInitialized)
                {
                    leftEye.transform.localPosition = new Vector3(-1 * (value.x), value.y, value.z);
                    rightEye.transform.localPosition = new Vector3((value.x), value.y, value.z);
                    UpdateEyeCovengence ();
                }
            } 
        } 

        [SerializeField]
        Vector3 m_UndistortionMeshPan;

        /// <summary>
        /// Gets or sets the undistortion mesh pan offset.
        /// </summary>
        /// <value>The undistortion mesh pan.</value>
        public Vector3 UndistortionMeshPanOffset
        {
            get
            {
                return m_UndistortionMeshPan;
            }
            set
            {
                m_UndistortionMeshPan = value;
                SetMeshPanOffset();
            }
        }

        [SerializeField]
        Vector3 m_LeftUndistortionMeshPan;

        /// <summary>
        /// Gets or sets the left undistortion mesh pan offset.
        /// </summary>
        /// <value>The undistortion mesh pan.</value>
        public Vector3 LeftUndistortionMeshPanOffset
        {
            get { return m_LeftUndistortionMeshPan; }
            set
            {
                m_LeftUndistortionMeshPan = value;
                SetMeshPanOffset();
            }
        }

        [SerializeField]
        Vector3 m_RightUndistortionMeshPan;

        /// <summary>
        /// Gets or sets the right undistortion mesh pan offset.
        /// </summary>
        /// <value>The undistortion mesh pan.</value>
        public Vector3 RightUndistortionMeshPanOffset
        {
            get { return m_RightUndistortionMeshPan; }
            set
            {
                m_RightUndistortionMeshPan = value;
                SetMeshPanOffset();
            }
        }


        private void SetMeshPanOffset()
        {
            if (leftEye != null)
            {
                Vector3 offset = UndistortionMeshPanOffset + LeftUndistortionMeshPanOffset;
                offset.x *= -1;
                offset.z = 1;
                leftEye.GetComponent<StereoEyeRenderer>().UpdateUndistortionPan(offset);
            }

            if (rightEye != null)
            {
                Vector3 offset = UndistortionMeshPanOffset + RightUndistortionMeshPanOffset;
                offset.z = 1;
                rightEye.GetComponent<StereoEyeRenderer>().UpdateUndistortionPan(offset);
            }
        }

        [SerializeField]
        Vector3 m_UndistortionMeshEuler;

        /// <summary>
        /// Gets the undistortion mesh euler.
        /// </summary>
        /// <value>The undistortion mesh euler.</value>
        public Vector3 UndistortionMeshEuler
        {
            get
            {
                return m_UndistortionMeshEuler;
            }
            set
            {
                m_UndistortionMeshEuler = value;
                if(leftEye && rightEye)
                {
                    leftEye.GetComponent<StereoEyeRenderer>().UpdateUndistortionMeshEuler(value);
                    rightEye.GetComponent<StereoEyeRenderer>().UpdateUndistortionMeshEuler(value);
                }
            }
        }


        float m_hFov,m_vFov;

        public float hFov
        {
            get
            {
                return m_hFov;
            }
            set
            {
                m_hFov = value;
                UpdateCameraFov();
            }
        }

        public float vFov
        {
            get
            {
                return m_vFov;
            }
            set
            {
                m_vFov = value;
                UpdateCameraFov();
            }
        }

        float m_FovScale = 1;

        public float fovScale
        {
            get
            {
                return m_FovScale;
            }
            set
            {
                m_FovScale = value;
                UpdateCameraFov();
            }
        }

        /// <summary>
        /// Initializes the camera instances.
        /// By default assuming the rendering mode is Stereo. 
        /// </summary>
        private void InitializeCameraInstances ()
        {
            mainCamera = this.GetComponent<Camera>();
            var mainCameraTransform = mainCamera.transform;

            //clear left-right eye if existence.
            if(leftEye != null)
            {
                DestroyImmediate(leftEye.gameObject);
            }
            if (rightEye != null)
            {
                DestroyImmediate(rightEye.gameObject);
            }

            leftEye = new GameObject("LeftEye", new System.Type[] { typeof(Camera) }).GetComponent<Camera>();
            rightEye = new GameObject("RightEye", new System.Type[] { typeof(Camera) }).GetComponent<Camera>();
            leftEye.CopyFrom(mainCamera);//clone camera setting and override some of it later.
            rightEye.CopyFrom(mainCamera);

            PhoneProfile phoneProfile = PhoneProfileContainer.GetCurrent();
            ReflectionLenProfile lenProfile = ReflectionLenProfileContainer.GetLenProfile();

            leftEye.transform.SetParent(mainCameraTransform);
            leftEye.transform.localPosition = new Vector3(-lenProfile.EyeSeparation, 0,0);
            rightEye.transform.SetParent(mainCameraTransform);
            rightEye.transform.localPosition = new Vector3(lenProfile.EyeSeparation, 0,0);
            m_EyeSeparation = new Vector3(lenProfile.EyeSeparation, 0, 0);
            m_UndistortionMeshPan = lenProfile.UndistortionMeshOffset;
            m_LeftUndistortionMeshPan = lenProfile.LeftUndistortionMeshOffset;
            m_RightUndistortionMeshPan = lenProfile.RightUndistortionMeshOffset;
            m_UndistortionMeshEuler = lenProfile.UndistortionMeshViewDirection;
            m_UndistortionMeshScale = lenProfile.UndistortionMeshScale;

            float hFov = lenProfile.hFov, vFov = lenProfile.vFov;
           
            leftEye.cullingMask = rightEye.cullingMask = mainCamera.cullingMask;

            this.m_hFov = hFov;
            this.m_vFov = vFov;
            mainCamera.SetCameraFov(hFov, vFov);

            //left - right bg color = black
            leftEye.clearFlags = rightEye.clearFlags = CameraClearFlags.SolidColor;
            leftEye.backgroundColor = rightEye.backgroundColor = Color.black;

            //Apply phone profile view rect:
            leftEye.rect = phoneProfile.LeftViewRect;
            rightEye.rect = phoneProfile.RightViewRect;

            //Assumes the rendering mode is stereo.
            mainCamera.enabled = false;
            leftEye.stereoTargetEye = StereoTargetEyeMask.Left;
            rightEye.stereoTargetEye = StereoTargetEyeMask.Right;
            mainCamera.stereoTargetEye = StereoTargetEyeMask.Both;
            leftEye.enabled = rightEye.enabled = true;
            mainCamera.enabled = false;//by default assuming rendering mode = stereo.
            leftEye.tag = "MainCamera";//main camera is disable, we need Camera.main return a valid camera.

            if (OnCameraIsCreated != null)
            {
                OnCameraIsCreated(leftEye, rightEye);
            }

            AddCameraComponents();
            //For mono rendering:

            //Update eye covengence at birth:
            this.UpdateEyeCovengence();

        }

        protected virtual void AddCameraComponents()
        {
            leftEye.gameObject.AddComponent<StereoEyeRenderer>();
            rightEye.gameObject.AddComponent<StereoEyeRenderer>();
        }

        private void InitializeHeadTracking ()
        {
            awakeRotation = this.transform.rotation;
            this.isHeadTrackingSupport = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);
            if(isHeadTrackingSupport)
            {
                Screen.orientation = ScreenOrientation.LandscapeRight;
                XRSettings.LoadDeviceByName("cardboard");
                XRSettings.enabled = false;
            }
        }

        void Update ()
        {
            if(isHeadTrackingSupport && this.m_HeadMode != ARCameraTrackingMode.Static)
            {
                transform.localRotation = GetTrackedHeadRotation ();
            }
        }

        void UpdateCameraFov()
        {
            this.mainCamera.SetCameraFovWithScale(this.hFov, this.vFov, this.fovScale);
            this.leftEye.SetCameraFovWithScale(this.hFov, this.vFov, this.fovScale);
            this.rightEye.SetCameraFovWithScale(this.hFov, this.vFov, this.fovScale);
        }

        static readonly Vector3 ViewerOrientationPreTilt = new Vector3 (0, -180, 0);
        static readonly Vector3 ViewerOrientationPostTilt = new Vector3(-43.5f, -180, 0);

        internal Quaternion GetTrackedHeadRotation ()
        {
            Quaternion rawTrackedEuler = InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.Head);
            var headRotation = Quaternion.Euler(ViewerOrientationPreTilt) * rawTrackedEuler * Quaternion.Euler(ViewerOrientationPostTilt);
            headRotation = m_RotationCompensatedError * headRotation;
            return headRotation;
        }


        /// <summary>
        /// Set the undistortion mesh runtime.
        /// This will replace the existsing undistortion mesh with the parameter.
        /// </summary>
        /// <param name="mesh">Mesh.</param>
        public void SetUndistortionMesh (Mesh UNDMesh)
        {
            this.leftEye.GetComponent<StereoEyeRenderer>().UseNewUndistortionMesh(UNDMesh);
            this.rightEye.GetComponent<StereoEyeRenderer>().UseNewUndistortionMesh(UNDMesh);
        }

        /// <summary>
        /// Recenter camera forward to birth oritentation.
        /// Note: when camera's transform is controlled by a benchmarker, Recenter() would do nothing.
        /// </summary>
        [ContextMenu("Recenter")]
        public void Recenter()
        {
//            Debug.Log("Recenter!");
            Quaternion targetRotation = this.awakeRotation;
            Quaternion headTrackedOritentation = InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.Head);
            Quaternion presentRotation = Quaternion.Euler(ViewerOrientationPreTilt) * headTrackedOritentation * Quaternion.Euler(ViewerOrientationPostTilt);
            Quaternion rPresent = Quaternion.Inverse(presentRotation);
            Quaternion offset = targetRotation * rPresent;
            //offset.eulerAngles = new Vector3(0, offset.eulerAngles.y, 0);
            this.m_RotationCompensatedError = offset;
        }

        /// <summary>
        /// Similiar to Recenter(), by only change's Yaw.
        /// </summary>
        /// <param name="yaw">Yaw.</param>
        [ContextMenu ("Recenter to yaw")]
        public void RecenterYaw (float yaw)
        {
            Quaternion targetRotation = Quaternion.Euler (0, yaw, 0);
            Quaternion headTrackedOritentation = InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.Head);
            Quaternion presentRotation = Quaternion.Euler(ViewerOrientationPreTilt) * headTrackedOritentation * Quaternion.Euler(ViewerOrientationPostTilt);
            Quaternion rPresent = Quaternion.Inverse(presentRotation);
            Quaternion offset = targetRotation * rPresent;
            offset.eulerAngles = new Vector3(0, offset.eulerAngles.y, 0);
            this.m_RotationCompensatedError = offset;
        }

        /// <summary>
        /// Recenter this AR camera to target rotation.
        /// </summary>
        /// <param name="targetRotation">Target rotation.</param>
        public void RecenterToEuler(Vector3 targetEuler)
        {
            Quaternion headTrackedOritentation =InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.Head);
            Quaternion presentRotation = Quaternion.Euler(ViewerOrientationPreTilt) * headTrackedOritentation * Quaternion.Euler(ViewerOrientationPostTilt);  
            Quaternion rPresent = Quaternion.Inverse(presentRotation);
            Quaternion offset = Quaternion.Euler(targetEuler) * rPresent;
            this.m_RotationCompensatedError = offset;
        }

        /// <summary>
        /// Updates the eye covengence.
        /// </summary>
        void UpdateEyeCovengence ()
        {
            float eyeCovengenceDistance = this.EyeCovergenceDistance;
            switch (this.EyeCovergenceMode)
            {
                case EyeCovengenceRenderingMode.Infinity:
                    this.leftEye.transform.localRotation = Quaternion.identity;
                    this.rightEye.transform.localRotation = Quaternion.identity;
                    break;
                case EyeCovengenceRenderingMode.CrossPlane:
                    Vector3 covengencePoint = this.mainCamera.ViewportToWorldPoint (new Vector3(0.5f, 0.5f, eyeCovengenceDistance));
                    leftEye.transform.LookAt(covengencePoint, leftEye.transform.up);
                    rightEye.transform.LookAt(covengencePoint, rightEye.transform.up);
                    break;
            }
        }

        /// <summary>
        /// Calls after the rendering mode is set at runtime.
        /// </summary>
        void OnRenderingModeChanged ()
        {
            switch(this.RenderingMode)
            {
                case StereoRenderingMode.Stereo:
                    this.mainCamera.enabled = false;
                    this.mainCamera.tag = "Untagged";
                    this.mainCamera.SetCameraFov(this.hFov, this.vFov);
                    this.leftEye.gameObject.SetActive(true);
                    this.leftEye.tag = "MainCamera";
                    this.rightEye.gameObject.SetActive(true);
                    break;

                case StereoRenderingMode.Single:
                    this.mainCamera.enabled = true;
                    this.mainCamera.tag = "MainCamera";
                    this.mainCamera.ResetProjectionMatrix();
                    this.leftEye.gameObject.SetActive(false);
                    this.leftEye.tag = "Untagged";
                    this.rightEye.gameObject.SetActive(false);
                    break;
            }
            Debug.LogFormat ("Rendering mode => {0}", RenderingMode.ToString());
        }

        /// <summary>
        /// Reverts setting to the default setting.
        /// </summary>
        public void RevertToDefaultSetting ()
        {
            var lenProfile = ReflectionLenProfileContainer.GetLenProfile();
            var arCam = this;
            {
                arCam.hFov = lenProfile.hFov;
                arCam.vFov = lenProfile.vFov;
                arCam.EyeSeparation = new Vector3(lenProfile.EyeSeparation, 0, 0);
                arCam.SetUndistortionMesh (lenProfile.UndistortionMesh);
                arCam.UndistortionMeshEuler = lenProfile.UndistortionMeshViewDirection;
                arCam.UndistortionMeshPanOffset = lenProfile.UndistortionMeshOffset;
                arCam.LeftUndistortionMeshPanOffset = lenProfile.LeftUndistortionMeshOffset;
                arCam.RightUndistortionMeshPanOffset = lenProfile.RightUndistortionMeshOffset;
                arCam.UndistortionMeshScale = lenProfile.UndistortionMeshScale;
            }
        }
    }

}