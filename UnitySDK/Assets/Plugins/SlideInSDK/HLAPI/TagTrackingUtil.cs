using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ximmerse.InputSystem;
using XDevicePlugin = Ximmerse.InputSystem.XDevicePlugin;
using TrackingResult = Ximmerse.InputSystem.TrackingResult;
using Vector4Int = PolyEngine.pVector4Int;


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
                return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
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
                    return "/sdcard/slidein/data";
                }
                else 
                {
                    return "Assets/Plugins/Android/assets";
                }
            }
        }

        /// <summary>
        /// Initializes the device module.
        /// </summary>
        public static void InitializeDeviceModule ()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Ximmerse.InputSystem.XDevicePlugin.Init();
                //Apply logs:
                XDevicePlugin.SetLogger((int level, string _tag, string log) =>
                    {
                        Debug.LogFormat("level:{0} ,tag:{1}, log:{2}", level, _tag, log);
                    });
                XDevicePlugin.CopyAssetsToPath(kConfigFileDirectory);
                DevicerHandle.SlideInContext = XDevicePlugin.NewContext(XDevicePlugin.XContextTypes.kXContextTypeSlideIn);/// 创建SlideIn设备上下文
                DevicerHandle.HmdHandle = XDevicePlugin.GetDeviceHandle(DevicerHandle.SlideInContext, "XHawk-0");
                TagTrackingUtil.ApplyDefaultConfig();
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
            }
            else
            {
                Debug.LogWarningFormat("Tracking Library not available for current platform : {0}. Currently we supports Android/Windows platform.", Application.platform);
            }
        }

        /// <summary>
        /// Gets marker tracking state, if marker is not tracked, return false. If tracked, out tracked raw position + rotation.
        /// </summary>
        /// <returns><c>true</c> if is marker tracked the specified markerID; otherwise, <c>false</c>.</returns>
        /// <param name="markerID">Marker I.</param>
        internal static bool GetMarkerState (int markerID, out Vector3 rawPos, out Quaternion rawRotation)
        {
            if(IsSupported)
            {
                XDevicePlugin.XAttrTrackingInfo marker_info = new XDevicePlugin.XAttrTrackingInfo(0, markerID);
                XDevicePlugin.GetObject(DevicerHandle.HmdHandle, XDevicePlugin.XVpuAttributes.kXVpuAttr_Obj_TrackingInfo, ref marker_info);

//                XDevicePlugin.DoAction(DevicerHandle.HmdHandle, XDevicePlugin.XActions.kXAct_GetMarkerInfo, ref marker_info);
                rawPos = Vector3.zero;
                rawRotation = Quaternion.identity;
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