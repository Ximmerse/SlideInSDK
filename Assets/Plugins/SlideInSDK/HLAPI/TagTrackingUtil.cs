using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ximmerse.InputSystem;
using XDevicePlugin = Ximmerse.InputSystem.XDevicePlugin;
using TrackingResult = Ximmerse.InputSystem.TrackingResult;
using Vector4Int = PolyEngine.pVector4Int;
using System.Runtime.InteropServices;

namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Tag tracking HLAPI util class.
    /// </summary>
    public static class TagTrackingUtil 
    {
        /// <summary>
        /// Applies the default config.
        /// </summary>
        internal static void ApplyDefaultConfig ()
        {
            MarkerPosePreTilt = Quaternion.Euler(TrackingConfig.MarkerPosePreTilt);
            MarkerPosePostTilt = Quaternion.Euler(TrackingConfig.MarkerPosePostTilt);

            RawPositionindex = TrackingConfig.RawPositionIndex;
            RawPositionFieldMultiplier = TrackingConfig.RawPositionFieldMultiplier;

            RawRotationIndex = TrackingConfig.RawRotationIndex;
            RawRotationFieldMultiplier = TrackingConfig.RawRotationFieldMultiplier;

            //            Debug.LogFormat ("TrackingConfig -  MarkerPosePreTilt:{0}, MarkerPosePostTilt:{1}, RawRotationFieldMultiplier:{2}", MarkerPosePreTilt, MarkerPosePostTilt, RawRotationFieldMultiplier);
        }

        #region Marker Tracking Utils


        /// <summary>
        /// Is tag tracking currently supported ?
        /// 
        /// </summary>
        /// <value><c>true</c> if is supported; otherwise, <c>false</c>.</value>
        public static bool IsSupported 
        {
            get 
            {
                //Not suported on iOS | OSX:
                return Application.platform == RuntimePlatform.Android ; //Bgfx : not supported in windows anymore...//|| Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
            }
        }

        /// <summary>
        /// The raw rotation field multiplier multiply with the raw tracked pos/rot.
        /// </summary>
        public static Vector4 RawRotationFieldMultiplier = Vector4.one, RawPositionFieldMultiplier = Vector4.one;

        /// <summary>
        /// The pre-tilt , pose-tilt after retrievd raw tracked pos/rot from VPU.
        /// </summary>
        public static Quaternion MarkerPosePreTilt, MarkerPosePostTilt;

        /// <summary>
        /// This is the array index to retrieve the tracked raw position float[] array.
        /// </summary>
        public static Vector3Int RawPositionindex;

        /// <summary>
        /// This is the array index to retrieve the tracked raw rotation float[] array.
        /// </summary>
        public static Vector4Int RawRotationIndex;

        public static string kConfigFileDirectory
        {
            get 
            {
                if(Application.platform == RuntimePlatform.Android)
                {
                    return Application.persistentDataPath;
                }
                else 
                {
                    return "Assets/Plugins/Android/assets";
                }
            }
        }

        static bool isInitialized = false;

        static bool isVPUConnected = false;

        /// <summary>
        /// Gets a value indicating is VPU connected.
        /// </summary>
        /// <value><c>true</c> if is VPU connected; otherwise, <c>false</c>.</value>
        public static bool IsVPUConnected
        {
            get
            {
                return isVPUConnected;
            }
        }

        /// <summary>
        /// Event callback on vpu connection state is changed.
        /// </summary>
        public static event System.Action<bool> OnVPUConnectionStateIsChanged;

        public static bool IsInitialized
        {
            get
            {
                return isInitialized;
            }
        }

        public static void SetAlgSmoothLevel (int level)
        {
            XDevicePlugin.DoAction(DevicerHandle.HmdHandle,
                XDevicePlugin.XActions.kXAct_SetPositionSmooth, level);
        }

        /// <summary>
        /// Initializes the device module.
        /// </summary>
        public static void InitializeDeviceModule (bool isFirst = true)
        {
            if (isInitialized)
                return;
            if (Application.platform == RuntimePlatform.Android)
            {
                Ximmerse.InputSystem.XDevicePlugin.Init();
                //Apply logs:
                XDevicePlugin.SetLogger((int level, string _tag, string log) =>
                    {
                        Debug.LogFormat("level:{0} ,tag:{1}, log:{2}", level, _tag, log);
                    });

                if (isFirst)
                {
                    XDevicePlugin.CopyAssetsToPath(kConfigFileDirectory);
                }
                DevicerHandle.SlideInContext = XDevicePlugin.NewContext(XDevicePlugin.XContextTypes.kXContextTypeSlideIn);/// 创建SlideIn设备上下文
                DevicerHandle.HmdHandle = XDevicePlugin.GetDeviceHandle(DevicerHandle.SlideInContext, "XHawk-0");
                if (isFirst)
                {
                    TagTrackingUtil.ApplyDefaultConfig();
                }

                DeviceConnectionState vpuConn = (DeviceConnectionState)XDevicePlugin.GetInt(DevicerHandle.HmdHandle, XDevicePlugin.XVpuAttributes.kXVpuAttr_Int_ConnectionState, 0);
                isVPUConnected = vpuConn == DeviceConnectionState.Connected;
                XDevicePlugin.RegisterObserver(DevicerHandle.HmdHandle, XDevicePlugin.XVpuAttributes.kXVpuAttr_Int_ConnectionState, new XDevicePlugin.XDeviceConnectStateChangeDelegate(OnVPUConnectionStateChanged), DevicerHandle.HmdHandle);
                isInitialized = true;

                Debug.LogFormat("Slide in HLAPI initialized successfully. HLAPI version : {0}, Algorithm version: {1}", HLAPIVersion.Version, HLAPIVersion.AlgVersion);
                Debug.LogFormat("VPU device state {0} ", vpuConn);
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                Ximmerse.InputSystem.XDevicePlugin.Init();
                XDevicePlugin.SetLogger((int level, string _tag, string log) =>
                    {
                        Debug.LogFormat("level:{0} ,tag:{1}, log:{2}", level, _tag, log);
                    });
                DevicerHandle.SlideInContext = XDevicePlugin.NewContext(XDevicePlugin.XContextTypes.kXContextTypeSlideIn);/// 创建SlideIn设备上下文
                DevicerHandle.HmdHandle = XDevicePlugin.GetDeviceHandle(DevicerHandle.SlideInContext, "XHawk-0");
                Debug.LogFormat("VPU Context: {0}, Hmd handle:{1}", DevicerHandle.SlideInContext.mNativeHandle.ToInt32(), DevicerHandle.HmdHandle.mNativeHandle.ToInt32());
                TagTrackingUtil.ApplyDefaultConfig();


                var vpuConn = (DeviceConnectionState)XDevicePlugin.GetInt(DevicerHandle.HmdHandle, XDevicePlugin.XVpuAttributes.kXVpuAttr_Int_ConnectionState, 0);
                isVPUConnected = vpuConn == DeviceConnectionState.Connected;
                XDevicePlugin.RegisterObserver(DevicerHandle.HmdHandle, XDevicePlugin.XVpuAttributes.kXVpuAttr_Int_ConnectionState, new XDevicePlugin.XDeviceConnectStateChangeDelegate(OnVPUConnectionStateChanged), DevicerHandle.HmdHandle);
                isInitialized = true;

                Debug.LogFormat("Slide in HLAPI initialized successfully. HLAPI version : {0}, Algorithm version: {1}", HLAPIVersion.Version, HLAPIVersion.AlgVersion);
                Debug.LogFormat("VPU device state {0} ", vpuConn);
            }
            else
            {
                Debug.LogWarningFormat("Tracking Library not available for current platform : {0}. Currently we supports Android/Windows platform.", Application.platform);
            }
        }

        /// <summary>
        /// Raises the device connect state change event.
        /// </summary>
        /// <param name="connect_st">Connect st.</param>
        /// <param name="ud">Ud.</param>
        private static void OnVPUConnectionStateChanged(int connect_st, System.IntPtr ud)
        {
            DeviceConnectionState connState = (DeviceConnectionState)connect_st;
            Debug.Log("Device connect state change, " + connState);

            isVPUConnected = connState == DeviceConnectionState.Connected;
            if (OnVPUConnectionStateIsChanged != null)
                OnVPUConnectionStateIsChanged(isVPUConnected);
        }

        /// <summary>
        /// De-initialize the module.
        /// </summary>
        public static void DeInitializeModule ()
        {
            if (isInitialized)
            {
                isInitialized = false;
                XDevicePlugin.ReleaseContext(DevicerHandle.HmdHandle);
                XDevicePlugin.SetLogger(null);
                XDevicePlugin.Exit();
            }
        }

        /// <summary>
        /// Gets marker tracking state, if marker is not tracked, return false. 
        /// If tracked, out tracked raw position + rotation.
        /// SubMarkerMask : the bitmask of sub markers. Each submark takes 1 bit.
        /// </summary>
        /// <returns><c>true</c>, if marker state was gotten, <c>false</c> otherwise.</returns>
        /// <param name="markerID">Marker I.</param>
        /// <param name="rawPos">Raw position.</param>
        /// <param name="rawRotation">Raw rotation.</param>
        /// <param name="SubMarkerMask">Sub marker mask.</param>
        internal static bool GetMarkerState (int markerID, out Vector3 rawPos, out Quaternion rawRotation, out ulong SubMarkerMask)
        {
            if(IsSupported)
            {
                XDevicePlugin.XAttrTrackingInfo marker_info = new XDevicePlugin.XAttrTrackingInfo(0, markerID);
                XDevicePlugin.GetObject(DevicerHandle.HmdHandle, XDevicePlugin.XVpuAttributes.kXVpuAttr_Obj_TrackingInfo, ref marker_info);
                //                XDevicePlugin.DoAction(DevicerHandle.HmdHandle, XDevicePlugin.XActions.kXAct_GetMarkerInfo, ref marker_info);
                rawPos = Vector3.zero;
                rawRotation = Quaternion.identity;
                SubMarkerMask = 0;
                if(marker_info.state == (int)TrackingResult.PoseTracked)
                {
                    var XYZIndex = RawPositionindex;
                    var QIndex = RawRotationIndex;
                    var XYZMul = RawPositionFieldMultiplier;
                    var QMul = RawRotationFieldMultiplier;

                    rawPos = new Vector3(
                        marker_info.position[XYZIndex.x] * XYZMul.x,
                        marker_info.position[XYZIndex.y] * XYZMul.y,
                        marker_info.position[XYZIndex.z] * XYZMul.z);

                    rawRotation = MarkerPosePreTilt * new Quaternion(
                        marker_info.rotation[QIndex.x] * QMul.x, 
                        marker_info.rotation[QIndex.y] * QMul.y, 
                        marker_info.rotation[QIndex.z] * QMul.z, 
                        marker_info.rotation[QIndex.w] * QMul.w)
                        * MarkerPosePostTilt;

                    SubMarkerMask = marker_info.recognized_markers_mask;
                    return true;
                }
                else 
                {
                    return false;
                }
            }
            else 
            {
                rawPos = Vector3.zero;
                rawRotation = Quaternion.identity;
                SubMarkerMask = 0;
                return false;
            }
        }

        static Transform leftEye, rightEye;

        /// <summary>
        /// Gets the node of the AR tracking system.
        /// </summary>
        /// <returns>The AR camera node.</returns>
        /// <param name="node">Node.</param>
        public static Transform GetNode (UnityEngine.XR.XRNode node)
        {
            switch(node)
            {
                case UnityEngine.XR.XRNode.LeftEye:
                    if(leftEye)
                        return leftEye;
                    else
                    {
                        leftEye = ARCamera.Singleton.transform.Find ("LeftEye");
                        return leftEye;
                    }

                case UnityEngine.XR.XRNode.RightEye:
                    if(rightEye)
                        return rightEye;
                    else
                    {
                        rightEye = ARCamera.Singleton.transform.Find ("RightEye");
                        return rightEye;
                    }

                case UnityEngine.XR.XRNode.CenterEye:
                case UnityEngine.XR.XRNode.Head:
                    return ARCamera.Singleton.transform;

                case UnityEngine.XR.XRNode.TrackingReference:
                    return TagTracker.TrackingAnchor;

                default:
                    Debug.LogErrorFormat("Unknown node: {0}", node.ToString());
                    return null;
            }
        }

        #endregion
    }

}