using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

//#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID || UNITY_WIN || UNITY_MAC
#if true
using UnityEngine;
using UnityEngine.Events;
using XDebug = UnityEngine.Debug;
#else
using XDebug = System.Diagnostics.Debug;
#endif // UNITY_EDITOR

using NativeHandle = System.IntPtr;

namespace Ximmerse.InputSystem
{
    public class XDevicePlugin
    {
        public const bool kSupportDeprecatedApi = true;

        public enum XLogLevels
        {
            kXLogLevel_Silence,
            kXLogLevel_Error,
            kXLogLevel_Warn,
            kXLogLevel_Info,
            kXLogLevel_Debug,
            kXLogLevel_Verbose,
        }
        //////////////////////////////////////////////////////////////////////////
        /// \enum XContextTypes
        /// \brief The parameters of the function, fill in the action_id
        public enum XContextTypes{
            kXContextTypeSlideIn, ///< Slide In product
        };
        
        /// /////////////////////////////////////////////////////////////////////
        /// \enum XControllerTypes
        ///  \bref Types defined of Ximmerse Controllers
        public enum XControllerTypes
        {
            kXControllerSaber = 0x00,  ///< Mirage Saber
            kXControllerKylo = 0x01,   ///< Mirage Kylo
            kXControllerDType = 0x02,  ///< D Controller
            kXControllerPickUp = 0x03, ///< Pick Up Controller
            kXController3Dof = 0x20,   ///< 3-Dof Flip Controller
            kXControllerTag = 0x10,    ///< Tag Controller
        }

        //////////////////////////////////////////////////////////////////////////
        /// \enum TrackingResult
        /// \brief Tracking status
        [System.Flags]
	    public enum XTrackingStates{
		    kXTrackingSt_NotTracked      =    0,  ///< Tracking lost
		    kXTrackingSt_RotationTracked = 1<<0,  ///< Only rotation tracking
		    kXTrackingSt_PositionTracked = 1<<1,  ///< Only position tracking
		    kXTrackingSt_PoseTracked = (kXTrackingSt_RotationTracked | kXTrackingSt_PositionTracked),  ///< Contains position and rotation tracking
		    kXTrackingSt_RotationEmulated = 1<<2, ///< Simulated rotation data
		    kXTrackingSt_PositionEmulated = 1<<3, ///< Simulated position data
	    };
        
        ///////////////////////////////////////////////////////
        /// @enum XControllerButtonMasks
        /// @brief Masks of Controller buttons.
        [System.Flags]
        public enum XControllerButtonMasks {
            kXControllerButton_DpapUp = 0x0001,
            kXControllerButton_DpapDown = 0x0002,
            kXControllerButton_DpadLeft = 0x0004,
            kXControllerButton_DpadRight = 0x0008,
            kXControllerButton_Start = 0x0010,
            kXControllerButton_Back = 0x0020,
            kXControllerButton_LeftThumb = 0x0040,
            kXControllerButton_RightThumb = 0x0080,
            kXControllerButton_LeftShoulder = 0x0100,
            kXControllerButton_RightShoulder = 0x0200,
            kXControllerButton_Guide = 0x0400,
            kXControllerButton_A = 0x1000,
            kXControllerButton_B = 0x2000,
            kXControllerButton_X = 0x4000,
            kXControllerButton_Y = 0x8000,
            // Emulation
            kXControllerButton_LeftThumbMove = 0x10000,
            kXControllerButton_RightThumbMove = 0x20000,
            kXControllerButton_LeftTrigger = 0x40000,
            kXControllerButton_RightTrigger = 0x80000,
            kXControllerButton_LeftThumbUp = 0x100000,
            kXControllerButton_LeftThumbDown = 0x200000,
            kXControllerButton_LeftThumbLeft = 0x400000,
            kXControllerButton_LeftThumbRight = 0x800000,
            kXControllerButton_RightThumbUp = 0x1000000,
            kXControllerButton_RightThumbDown = 0x2000000,
            kXControllerButton_RightThumbLeft = 0x4000000,
            kXControllerButton_RightThumbRight = 0x8000000,
            //
            kXControllerButton_None = 0x0,
            kXControllerButton_ANY = ~kXControllerButton_None,
        };

        
        //////////////////////////////////////
        /// @enum XContextStates
        /// @brief State of Context.
        public enum XContextStates {
            kXContextStInited, ///< context state, context enviroment allocted, but not working.
            kXContextStStarted, ///< context state, devices in context working.
            kXContextStWillRelease, ///< context state, moment before release.
        };

        ////////////////////////////////////////////////////////
        /// @enum XContextAttributes
        /// @brief Attributes of Context
        public enum XContextAttributes {
            //kXCtxAttr_DeviceVersion,
            /// Get the SDK platform support library version 
            kXCtxAttr_Int_SdkVersion,
            /// Get the SDK platform support library version 
            kXCtxAttr_Str_SdkVersion,
            kXCtxAttr_Int_SDKALGVersion, // int
            kXCtxAttr_Int_State, ///< State value indicated context working state, 
        };

        ////////////////////////////////////////////////////////
        /// @enum XVpuAttributes
        /// @brief Attributes of VPU Device.
        public enum XVpuAttributes {
            kXVpuAttr_Str_SoftwareRevision,
            kXVpuAttr_Str_HardwareRevision, 
            kXVpuAttr_Str_SerialNumber,
            kXVpuAttr_Str_DeviceName,
            kXVpuAttr_Str_ModelName,
            kXVpuAttr_Str_FPGAVersion,
            kXVpuAttr_Str_ALGVersion,

            kXVpuAttr_Int_ImuFps,
            kXVpuAttr_Int_FpgaFps,
            /// Reversed
            kXVpuAttr_Int_AlgorithmPoseFps,

            kXVpuAttr_Int_ErrorCode,

            /// Connection state of Controller, see \ref XConnectionStates
            kXVpuAttr_Int_ConnectionState,
    
            kXVpuAttr_Int_PowerMode,
            /// Battery Level. Invalid if battery mode is external power
            kXVpuAttr_Int_Battery,
            kXVpuAttr_Int_BatteryVoltage,
            kXVpuAttr_Int_BatteryTemperature,

            /// IMU info of device, see \ref XAttrImuInfo
            kXVpuAttr_Obj_ImuInfo, 
            /// IMU info of device, Output to variable aguments. Invalid for Unity
            kXVpuAttr_V_ImuInfo, 

            /// 6Dof info of device, see \ref XAttr6DofInfo
            kXVpuAttr_Obj_6DofInfo,
            /// 6Dof info of device, Output to variable aguments. Invalid for Unity
            kXVpuAttr_V_6DofInfo,
    
            /// Pressed button bits
            kXVpuAttr_Int_ButtonBits, // int 

            /// Button events, \ref XButtonEvents
            kXVpuAttr_Int_ButtonEvent = kXVpuAttr_Int_ButtonBits,

            /// Number of Paired controllers
            kXVpuAttr_Int_PairedNumber,

            /// Device infos, imu, 6dof, buttons ... see \ref XAttrControllerState
            kXVpuAttr_Obj_ControllerState,

            /// VPU cammera tracking object pose info. see \ref XAttrTrackingInfo
            kXVpuAttr_Obj_TrackingInfo,

            /// VPU cammera tracking object pose info. Output to variable aguments
            kXVpuAttr_V_TrackingInfo,
            

        };

        ////////////////////////////////////////////////////////
        /// @enum XControllerAttributes
        /// @brief Attribute types of controller.
        public enum XControllerAttributes {
            kXCAttr_Str_FirmwareRevision,
            kXCAttr_Str_SoftwareRevision,
            kXCAttr_Str_HardwareRevision,
            kXCAttr_Str_SerialNumber,
            kXCAttr_Str_DeviceName,
            kXCAttr_Str_ModelName, 
            kXCAttr_Str_ManufacturerName,

            /// FPS of controller device reporting IMU data
            kXCAttr_Int_ImuFps,

            kXCAttr_Int_ErrorCode,

            /// Connection state of Controller, see \ref XConnectionStates
            kXCAttr_Int_ConnectionState,

            kXCAttr_Int_PowerMode,
            /// Battery Level. Invalid if battery mode is external power
            kXCAttr_Int_Battery,
            kXCAttr_Int_BatteryVoltage,
            kXCAttr_Int_BatteryTemperature,


            /// IMU info of device, see \ref XAttrImuInfo
            kXCAttr_Obj_ImuInfo,
            /// IMU info of device, Output to variable aguments. Invalid for Unity
            kXCAttr_V_ImuInfo,

            /// 6Dof info of device, see \ref XAttr6DofInfo
            kXCAttr_Obj_6DofInfo,
            /// 6Dof info of device, Output to variable aguments. Invalid for Unity
            kXCAttr_V_6DofInfo,

            /// Pressed button bits
            kXCAttr_Int_ButtonBits, 
            /// Button events, \ref XButtonEvents
            kXCAttr_Int_ButtonEvent = kXCAttr_Int_ButtonBits,

            kXCAttr_Int_Trigger,
            /// Device touchpad state info. see \ref XAttrTouchPadInfo
            kXCAttr_Obj_TouchPadState,

            //kXCAttr_Int_TouchPadState, // 

            /// Device infos, imu, 6dof, buttons ... see \ref XAttrControllerState
            kXCAttr_Obj_ControllerState, 

        };

        //////////////////////////////////////
        /// @enum XButtonEvents
        /// @brief Button events
        public enum XButtonEvents {
            kXBtnEvt_Down,
            kXBtnEvt_Up,
            kXBtnEvt_LongPress,
        };

        ///////////////////////////////////////
        /// @struct XButtonEventParam
        /// @brief Parameters for button event notification
        [StructLayout(LayoutKind.Sequential)]
        public struct XButtonEventParam {
            /// Button key value.
            public int btn;
            /// Event value. see \ref XButtonEvents
            public int evt;

            public XButtonEventParam(int btn, int evt)
            {
                this.btn = btn;
                this.evt = evt;
            }
        };

        ////////////////////////////////////////
        /// @enum XConnectionStates
        /// @brief Connection state of device
        public enum XConnectionStates {
            kXConnSt_Disconnected = 0,
            kXConnSt_Connecting = 1,
            kXConnSt_Connected = 3,
            //kXConnSt_Disconnected = ?
        };

        ///////////////////////////////////////////////
        /// @struct XAttr6DofInfo
        /// @brief Structure for gettting 6Dof information
        [StructLayout(LayoutKind.Sequential)]
        public struct XAttr6DofInfo {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] position;  /// < float buffer for gettting position values
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] rotation; /// < float buffer for getting quaternions of rotation.
            public UInt64 timestamp;

            public XAttr6DofInfo (UInt64 timestamp = 0, float[] position = null, float[] rotation = null)
            {
                this.timestamp = timestamp;
                this.position = position == null ? new float[3] {0, 0, 0 } : position;
                this.rotation = rotation == null ? new float[4] {0, 0, 0, 0 } : rotation;
            }
        };

        //////////////////////////////////////////////
        /// @struct XAttrImuInfo
        /// @brief Structure for getting IMU information.
        [StructLayout(LayoutKind.Sequential)]
        public struct XAttrImuInfo {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] accelerometer; ///< (out) float buffer for getting accelerometer values.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] gyroscope; ///< float buffer for getting gyroscope values.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] magnetism; /// < float buffer for getting magnetism values.
            public UInt64 timestamp;

            public XAttrImuInfo(
                UInt64 timestamp = 0, 
                float[] accelerometer = null, 
                float[] gyroscope = null, float[] magnetism = null)
            {
                this.timestamp = timestamp;
                this.accelerometer = accelerometer == null ? new float[3] {0, 0, 0 } : accelerometer;
                this.gyroscope = gyroscope == null ? new float[3] {0, 0, 0 } : gyroscope;
                this.magnetism = magnetism == null ? new float[3] {0, 0, 0 } : magnetism;
            }
        };

        /////////////////////////////////////////////
        /// @struct XAttrTrackingInfo
        /// @brief VPU cammera tracking object pose info.
        [StructLayout(LayoutKind.Sequential)]
        public struct XAttrTrackingInfo {
            public int index;
            public int state;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] position;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] rotation;
            public UInt64 timestamp;
            public UInt64 recognized_markers_mask;
            public XAttrTrackingInfo(
                UInt64 timestamp = 0, 
                int index = 0, 
                int state = 0, 
                float[] position = null, 
                float[] rotation = null)
            {
                this.timestamp = timestamp;
                this.index = index;
                this.state = state;
                this.position = position == null ? new float[3] {0, 0, 0} : position;
                this.rotation = rotation == null ? new float[4] {0, 0, 0, 0 } : rotation;
                this.recognized_markers_mask = 0;
            }
        };

        //////////////////////////////////////
        /// @struct XAttrControllerState
        /// @brief Structure for getting controller state informations.
        [StructLayout(LayoutKind.Sequential)]
        public struct XAttrControllerState {
            
            /// quaternion 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] rotation;

            /// < x, y, z
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] position;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] accelerometer;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] gyroscope;

            public UInt64 timestamp;

            /// < bit map indicating button pressed state of controller.
            public UInt32 button_state;

            //uint8_t valid_flag; /// 
            // TouchPad, reverve
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public float[] axes;

            public XAttrControllerState(
                UInt64 timestamp = 0,
                UInt32 button_state = 0,
                float[] rotation = null,
                float[] position = null,
                float[] accelerometer = null,
                float[] gyroscope = null,
                float[] axes = null)
            {
                this.timestamp = timestamp;
                this.button_state = button_state;
                this.rotation = rotation == null ? new float[4] {0, 0, 0, 0 } : rotation;
                this.position = position == null ? new float[3] {0, 0, 0 } : position;
                this.accelerometer = accelerometer == null ? new float[3] {0, 0, 0 } : accelerometer;
                this.gyroscope = gyroscope == null ? new float[3] {0, 0, 0 } : gyroscope;
                this.axes = axes == null ? new float[6] {0,0,0,0,0,0 } : axes;
            }
        };

        
        public enum XAttrTouchPadEvents {
            kAttrTpadEvt_Idle = 0x00,
            kAttrTpadEvt_Press = 0x01,
            kAttrTpadEvt_Release = 0x02,
            kAttrTpadEvt_Move = 0x03,
        };
        
        [StructLayout(LayoutKind.Sequential)]
        public struct XAttrTouchPadState {
            public bool pressed; ///< indicated touchpad is touching.
            public float x; ///< x coordinate value, from 0 to 1.
            public float y; ///< y coordinate value, from 0 to 1.
        };


        //////////////////////////////////////////////////////////////////////////
        /// \enum XActions
        /// \brief The parameters of the DoAction method , fill in the action_id
        public enum XActions {
            
            kXAct_UnpairController, ///< The command to unbind a connected controller
            kXAct_UnpairAllControllers,///< The command to unbind all controllers
            kXAct_StartPairingController,///< The command to start the controller pairing 
            kXAct_StopPairingController,///< The command to stop the controller pairing.

            kXAct_ConnectControllerByBindID,	///< Connect to a paired controller by specifying bind ID.
            kXAct_ConnectAllPairedControllers, ///< Connect to All Paired Controllers.
            kXAct_DisconnectControllerByIndex, ///< Disconnect a connected controller by specifying connected index.
            kXAct_DisconnectControllerByBindID, ///< Disconnect a connected controller by specifying bind ID.
            kXAct_DisconnectAllControllers, ///< Disconnect all connected Controllers.

            kXAct_Vibrate,	///< Set the motor vibration command, the parameter is the structure of ActParam_Vibration
            kXAct_Vibrate_V, ///< Set the motor vibration command with variable parameters , this command is INVALID in Unity.
            kXAct_Sleep, ///< The command to set the device to sleep
            kXAct_Wakeup, ///< The command to set the device to Wakeup

            kXAct_ConnectControllerByMacAddr, ///< Connect to controller by specified MAC address, parameter is a MAC address string with hex format.

            kXAct_CustomBegin,///< The separator of the current enum definition

            kXAct_SlideInActionsBegin = XActions.kXAct_CustomBegin, ///< The offset of an enumeration type
            kXAct_LoadMarkerSettingFile, ///< Load the configuration file of Tag tracking algorithm
            kXAct_ResetMarkerSettings,  ///< Clear the configuration of the Tag tracking algorithm
            kXAct_LoadCameraCalibrationFile, ///< Load camera calibration parameters from the external,used in debugging
            kXAct_SetPositionSmooth, ///< Set the smoothing parameters of the Tag tracking algorithm,set -1 to close the smoothing of the algorithm, the valid range is 0 to 5
            
            kXAct_Max,

            kXAct_DeprecatedOffset = 10000,
            ////////////////////////////// Deprecated Actions ////////////////////
            
            /// deprecated
            kXAct_GetInt_CtxDeviceVersion,

            /// \deprecated use \ref XContextAttributes.kXCtxAttr_Int_SdkVersion instead.
            /// Get the SDK platform support library version 
            kXAct_GetInt_CtxSdkVersion, 
            
            /// \deprecated use \ref XContextAttributes.kXCtxAttr_Int_SDKALGVersion instead. 
            /// Get the SDK algorithm version and the version number returned is hexadecimal
            kXAct_GetInt_CtxSDKALGVersion, 
            
            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Int_ImuFps and \ref XControllerAttributes.kXCAttr_Int_ImuFps instead.
            /// Gets the frame rate of the device
            kXAct_GetInt_FPS,
            
            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Int_ErrorCode and \ref XControllerAttributes.kXCAttr_Int_ErrorCode instead,
            /// Get the device error code
            kXAct_GetInt_ErrorCode, 
            
            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Int_ConnectionState and \ref XControllerAttributes.kXCAttr_Int_ConnectionState instead,
            /// Gets the device connection status
            kXAct_GetInt_ConnectionState, 
            
            /// \deprecated
            /// Gets the ID of the controller marker. Reserved.
            kXAct_GetInt_BlobID, 
            
            /// \deprecated 
            /// Sets the ID of the controller marker. Reserved.
            kXAct_SetBlogID, 
            
            /// \deprecated use \ref XVpuAttributes.kXCAttr_Int_Battery and \ref XControllerAttributes.kXCAttr_Int_Battery instead,
            /// Acquire the battery capacity of the device
            kXAct_GetInt_Battery,
            
            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Int_PowerMode and \ref XControllerAttributes.kXCAttr_Int_PowerMode instead.
            /// Gets the power supply mode of the device
            kXAct_GetInt_BatteryMode, 
            
            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Int_BatteryVoltage and \ref XControllerAttributes.kXCAttr_Int_BatteryVoltage instead.
            /// Gets the battery voltage of the device
            kXAct_GetInt_BatteryVoltage, 

            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Int_BatteryTemperature and \ref XControllerAttributes.kXCAttr_Int_BatteryTemplarature instead.
            /// Gets the battery temperature of the device
            kXAct_GetInt_BatteryTemperature, 
            
            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Str_SoftwareRevision and \ref XControllerAttributes.kXCAttr_Str_SoftwareRevision instead.
            /// Gets firmware version and returns a string
            kXAct_GetStr_SoftwareRevision, 
            
            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Str_HardwareRevision and \ref XControllerAttributes.kXCAttr_Str_HardwareRevision instead.
            /// Gets hardware version and returns a string
            kXAct_GetStr_HardwareRevision, 
            
            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Str_FPGAVersion instead
            /// Gets FPGA version and returns a string
            kXAct_GetStr_FPGAVersion, 

            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Str_ModelName and \ref XControllerAttributes.kXCAttr_Str_ModelName instead.
            /// Gets model name and returns a string
            kXAct_GetStr_ModelName,

            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Str_DeviceName and \ref XControllerAttributes.kXCAttr_Str_DeviceName instead.
            /// Gets display name information and returns a string
            kXAct_GetStr_DisplayName,
            
            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Str_ALGVersion instead.
            /// Gets the version of the algorithm on the firmware and returns a string
            kXAct_GetStr_ALGVersion, 

            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Str_SerialNumber and \ref XControllerAttributes.kXCAttr_Str_SerialNumber instead.
            /// Get the device serial number for production
            kXAct_GetStr_SerialNumber,

            /// \deprecated use \ref XControllerAttributes.kXCAttr_Str_ManufacturerName instead.
            /// Gets the manufacturer of device
            kXAct_GetStr_ManufacturerName, 

            /// \deprecated use \ref XControllerAttributes.kXCAttr_Str_FirmwareRevision instead.
            /// An extended interface to get firmware version information
            kXAct_GetStr_FirmwareRevision, 

            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Obj_ImuInfo and \ref XControllerAttributes.kXCAttr_Obj_ImuInfo instead.
            /// Command to get the IMU information and the argument is the structure of ActParam_IMUInfo
            kXAct_GetImuInfo, 

            /// \deprecated
            /// Command to get IMU information with variable parameters. This command is INVALID in Unity.
            kXAct_GetImuInfo_V, 

            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Obj_6DofInfo and \ref XControllerAttributes.kXCAttr_Obj_6DofInfo instead.
            /// Command to get 6-Dof information and the argument is the structure of ActParam_6DofInfo
            kXAct_Get6DofInfo, 

            /// \deprecated
            /// Command to get 6-Dof information with variable parameters, this command is INVALID in Unity.
            kXAct_Get6DofInfo_V, 

            /// \deprecated use \ref XVpuAttributes.XVpuAttr_Int_ButtonBits and XCAttributes.XVpuAttr_Int_ButtonBits instead.
            /// Command to get buttons information 
            kXAct_GetInt_ButtonBits,

            /// \deprecated use \ref XCAttributes.kXCAttr_Obj_TouchPadState instead.
            /// Command to get touchPad information. Reserved
            kXAct_Get_TouchPadState, 

            /// \deprecated
            /// The command to set the VPU reference time. Reserved
            //kXAct_Set_VPUTime, 
            
            /// \deprecated use \ref  XVpuAttributes.kXVpuAttr_Int_PairedNumber
            kXAct_GetInt_PairedNumber,
            /// \deprecated
            /// Gets the ID list of controllers that have been paired. Reserved
            kXAct_GetPairedList, 
            
             
            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Obj_ControllerState and XCAttributes.kXCAttr_Obj_ControllerState instead.
            /// Command to get controller state and the argument is the structure of ActParam_ControllerState
            kXAct_GetControllerState, 

            /// \deprecated use \ref XVpuAttributes.kXVpuAttr_Obj_TrackingInfo instead.
            /// Get the 6-dof information of Marker
            kXAct_GetMarkerInfo, 
            /// \deprecated 
            /// Obtain the 6-dof information of Marker with variable parameters. This command is INVALID in Unity.
            kXAct_GetMarkerInfo_V, 
        };

        
        //////////////////////////////////////////////////////////////////////////
        /// \brief Input device handle
        public class XHandle
        {
            public NativeHandle mNativeHandle;
            public string mName;
            public XHandle(NativeHandle native_handle, string name = "")
            {
                mNativeHandle = native_handle;
                mName = name;
            }
            //public static bool operator == (XHandle a, XHandle b)
            //{
            //    return a.mNativeHandle == b.mNativeHandle;
            //}
            //public static bool operator != (XHandle a, XHandle b)
            //{
            //    return a.mNativeHandle != b.mNativeHandle;
            //}
           
            public bool Equals(XHandle a)
            {
                return mNativeHandle == a.mNativeHandle;
            }
            //public string ToString()
            //{
            //    return "";
            //}
        }

#region native 

        private const string pluginName = "xdevice"; ///< The name of the platform support library for the device

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int xdev_init();
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int xdev_exit();
            
        private delegate void XDevLogDelegate (int level, string tag, string log);
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        //private static extern int xdev_set_logger(XLogDelegate logger);
            private static extern int xdev_set_logger(XLogDelegate logger, int log_level);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern NativeHandle xdev_new_context(int context_type);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int xdev_release_context(NativeHandle context_handle);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern NativeHandle xdev_get_device_handle(NativeHandle context_handle, string name);
        
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool xdev_get_bool(NativeHandle handle, int attr_id, bool default_value);
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int xdev_get_int(NativeHandle handle, int attr_id, int default_value);
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern float xdev_get_float(NativeHandle handle, int attr_id, float default_value);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        //private static extern string xdev_get_string(NativeHandle handle, int act_id, string default_value);
        private static extern System.IntPtr xdev_get_string(NativeHandle handle, int attr_id, System.IntPtr default_value);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int xdev_get_object(NativeHandle handle, int attr_id, System.IntPtr value);

        //public delegate void XDevAttributeObserveDelegate(NativeHandle handle, int attr_id, System.IntPtr val, int val_size, System.IntPtr ud);
        //[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        //private static extern int xdev_observe(NativeHandle handle, int attr_id, XDevAttributeObserveDelegate dlg, IntPtr ud);

        public delegate void XContextStateChangeDelegate(int st, System.IntPtr ud);
        public delegate void XDeviceConnectStateChangeDelegate(int connect_st, System.IntPtr ud);
        //public delegate void XDeviceButtonStateChangeDelegate(XButtonEventParam btn_param, System.IntPtr ud);
        public delegate void XDeviceButtonStateChangeDelegate(int btn_mask, int evt, System.IntPtr ud);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int xdev_register_observer(NativeHandle handle, int attr_id, IntPtr dlg, System.IntPtr ud);
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int xdev_unregister_observer(NativeHandle handle, int attr_id, IntPtr dlg);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int xdev_do(NativeHandle handle, int action_id);
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int xdev_do(NativeHandle handle, int action_id, string arg);
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int xdev_do(NativeHandle handle, int action_id, int arg);
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int xdev_doa(NativeHandle handle, int action_id, System.IntPtr argv);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern string xdev_get_name(NativeHandle handle);
#endregion native

#region Public Properties

        /////////////////////////////////////
        /// \brief  Initialization of the platform support library for input devices
        /// \return Return 0 indicates success
        public static int Init()
        {
            //ObserverManager.instance.Init();
#if UNITY_ANDROID
            int ret = -1;
            var clazz = new AndroidJavaClass("com.ximmerse.sdk.XDeviceApi2");
            using(AndroidJavaClass jc=new AndroidJavaClass("com.unity3d.player.UnityPlayer")){
				using(AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity")){
                    ret = clazz.CallStatic<int>("init",currentActivity);
                }
            }
            return ret;
#else
            return xdev_init();
#endif
        }

        /////////////////////////////////////
        /// \brief  Exit the platform support library for the input device
        /// \return Return 0 indicates success
        public static int Exit()
        {
            ObserverDict.Clear();
#if UNITY_ANDROID
            var clazz = new AndroidJavaClass("com.ximmerse.sdk.XDeviceApi2");
            return clazz.CallStatic<int>("exit");
#else
            return xdev_exit();
#endif
        }

        /////////////////////////////////////
        /// \brief  Create the device context
        ///    \param [in] context_type device type
        /// \return Return the XHandle of device
        public static XHandle NewContext(XContextTypes context_type)
        {
            return new XHandle(xdev_new_context((int)context_type), "XCtx-"+(int)context_type);
        }

        /////////////////////////////////////
        /// \brief  Destroy the device context
        ///    \param [in] context_handle The device handle
        /// \return Return 0 indicates success
        public static int ReleaseContext(XHandle context_handle)
        {
            return xdev_release_context(context_handle.mNativeHandle);
        }


        /////////////////////////////////////
        /// \brief  Delegate for printing output
        ///    \param [in] level Print level
        ///    \param [in] tag Printed label
        ///    \param [in] log Printed content    
        /// \return no return value
        public delegate void XLogDelegate(int level, string tag, string log);


        static XLogDelegate sLogDelegate = null;
        /////////////////////////////////////
        /// \brief  Set up the print listener of XLogDelegate
        ///    \param [in] logger The parameter of XLogDelegate type 
        /// \return no return value
        public static void SetLogger(XLogDelegate logger, XLogLevels lv = XLogLevels.kXLogLevel_Debug) {
            sLogDelegate = logger;
            //xdev_set_logger(NativieLogDelegate);
            xdev_set_logger(logger, (int) lv);
        }

        ////////////////////////////////////////
        /// \brief Get Name of the provided handle
        /// \param [in] handle
        /// \return return the name.
        public static string GetName(XHandle handle)
        {
            return xdev_get_name(handle.mNativeHandle);
        }

        /////////////////////////////////////
        ///
        /// \brief  Gets the device handle through the device name
        ///    \param [in] context_handle The device handle
        ///    \param [in] name device name
        /// \return Return 0 indicates success
        public static XHandle GetDeviceHandle(XHandle context_handle, string name)
        {
            var native_handle = xdev_get_device_handle(context_handle.mNativeHandle, name);
            return native_handle != null ? new XHandle(native_handle, name) : null;
        }
        
        /////////////////////////////////////
        ///
        /// \brief  Gets the Boolean type parameters for the device
        ///    \param [in] handle The device handle
        ///    \param [in] attr_id The attribute name,defined in the XDeviceConstants file, its name is XActions.kXAct_GetBool_xxxx
        ///    \param [in] default_value Get failure returns a default value
        /// \return Returns the value of the Boolean property if it is valid for handle, otherwise return default_value .
        public static bool GetBool<E>(XHandle handle, E attr_id, bool default_value = false)
        {
            var a_id = Convert.ToInt32(attr_id);
            if (kSupportDeprecatedApi)
            {
                if (a_id >= (int) XActions.kXAct_DeprecatedOffset)
                {
                    // deprecated action
                    a_id = ConvertAction2AttrID(handle, (XActions) a_id);
                }
            }
            return xdev_get_bool(handle.mNativeHandle, a_id, default_value);
        }

        /////////////////////////////////////
        ///
        /// \brief  Gets the Int type parameters for the device
        ///    \param [in] handle The device handle
        ///    \param [in] attr_id The attribute name,defined in the XDeviceConstants file, its name is XActions.kXAct_GetInt_xxxx
        ///    \param [in] default_value Get failure returns a default value
        /// \return Returns the value of the Int property if it is valid for handle, otherwise return default_value .
        public static int GetInt<E>(XHandle handle, E attr_id, int default_value = 0)
        {
            var a_id = Convert.ToInt32(attr_id);
            if (kSupportDeprecatedApi)
            {
                if (a_id >= (int) XActions.kXAct_DeprecatedOffset)
                {
                    // deprecated action
                    a_id = ConvertAction2AttrID(handle, (XActions) a_id);
                }
            }
            return xdev_get_int(handle.mNativeHandle, a_id, default_value);
        }

        /////////////////////////////////////
        ///
        /// \brief  Gets the Float type parameters for the device
        ///    \param [in] handle The device handle
        ///    \param [in] attr_id The attribute name,defined in the XDeviceConstants file, its name is XActions.kXAct_GetFloat_xxxx
        ///    \param [in] default_value Get failure returns a default value
        /// \return Returns the value of the Float property if it is valid for handle, otherwise return default_value .
        public static float GetFloat<E>(XHandle handle, E attr_id, float default_value)
        {
            var a_id = Convert.ToInt32(attr_id);
            if (kSupportDeprecatedApi)
            {
                if (a_id >= (int) XActions.kXAct_DeprecatedOffset)
                {
                    // deprecated action
                    a_id = ConvertAction2AttrID(handle, (XActions) a_id);
                }
            }
            return xdev_get_float(handle.mNativeHandle, a_id, default_value);
        }


        /////////////////////////////////////
        ///
        /// \brief  Gets the String type parameters for the device
        ///    \param [in] handle The device handle
        ///    \param [in] attr_id The attribute name,defined in the XDeviceConstants file, its name is XActions.kXAct_GetStr_xxxx
        ///    \param [in] default_value Get failure returns a default value
        /// \return Returns the value of the string property if it is valid for handle, otherwise return default_value .
        public static string GetString<E>(XHandle handle, E attr_id, string default_value)
        {
            int a_id = Convert.ToInt32(attr_id);
            if (kSupportDeprecatedApi) {
                if (a_id > (int) XActions.kXAct_DeprecatedOffset)
                {
                    a_id = ConvertAction2AttrID(handle, (XActions) a_id);
                }
            }
            //var ret = xdev_get_string(handle.mNativeHandle, Convert.ToInt32(attr_id), null);
            //return ret != null ? ret : default_value;
            var native_ret = xdev_get_string(handle.mNativeHandle, a_id, (System.IntPtr)null);
            if (native_ret != (System.IntPtr)null)
                return Marshal.PtrToStringAnsi(native_ret);
            return default_value;
        }
        public static int GetObject<E, T>(XHandle handle, E attr_id, ref T arg)
        {
            /// \todo: implete GetObject
            System.IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
            Marshal.StructureToPtr(arg, ptr, false);
            int ret = xdev_get_object(handle.mNativeHandle, Convert.ToInt32(attr_id), ptr);
            arg = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return ret;
        }

        /////////////////////////////////////
        ///
        /// \brief  function is used to control input device
        ///    \param [in] handle Handle to the device,
        ///    \param [in] action_id Command name,please see the enum of XActions
        /// \returnResult of the command, 0 for success.
        public static int DoAction(XHandle handle, XActions action_id)
        {
            return xdev_do(handle.mNativeHandle, (int)action_id);
        }

        /////////////////////////////////////
        ///
        /// \brief  function is used to control input device
        ///    \param [in] handle Handle to the device,
        ///    \param [in] action_id Command name,please see the enum of XActions
        ///    \param [in] args String parameter
        /// \return Result of the command, 0 for success.
        public static int DoAction(XHandle handle, XActions action_id, string args)
        {
            //return xdev_do(handle.mNativeHandle, action_id, __arglist(args.ToCharArray()));
            return xdev_do(handle.mNativeHandle, (int)action_id, args);
        }

        /////////////////////////////////////
        ///
        /// \brief  function is used to control input device
        ///    \param [in] handle Handle to the device,
        ///    \param [in] action_id Command name,please see the enum of XActions
        ///    \param [in] args Int parameter
        /// \return Result of the command, 0 for success.
        public static int DoAction(XHandle handle, XActions action_id, int args)
        {
            return xdev_do(handle.mNativeHandle, (int)action_id, args);
        }

        /////////////////////////////////////
        ///
        /// \brief  function is used to control input device
        ///    \param [in] handle Handle to the device,
        ///    \param [in] action_id Command name,please see the enum of XActions
        ///    \param [out] arg template type parameter reference for returning data from handle.
        /// \return Result of the command, 0 for success.
        public static int DoAction<T>(XHandle handle, XActions action_id, ref T arg)
        {
            if (kSupportDeprecatedApi)
            {
                if (action_id >= XActions.kXAct_DeprecatedOffset)
                {
                    // deprecated action
                    int attr_id = ConvertAction2AttrID(handle, action_id);
                    if (attr_id >= 0)
                    {
                        return GetObject(handle, attr_id, ref arg);
                    }
                }
            }
            System.IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
            Marshal.StructureToPtr(arg, ptr, false);
            int ret = xdev_doa(handle.mNativeHandle, (int)action_id, ptr);
            arg = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return ret;
        }

        /////////////////////////////////////
        ///
        /// \brief  function is used to control input device
        ///    \param [in] handle Handle to the device,
        ///    \param [in] action_id Command name,please see the enum of XActions
        ///    \param [in] args <T> parameter
        /// \return Result of the command, 0 for success.
        public static int DoAction<T>(XHandle handle, XActions action_id, T arg)
        {
            var t = typeof(T);
            if (t.IsEnum)
            {
                return DoAction(handle, action_id, System.Convert.ToInt32(arg));
            }
            System.IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(t));
            Marshal.StructureToPtr(arg, ptr, false);
            int ret = xdev_doa(handle.mNativeHandle, (int)action_id, ptr);
            Marshal.FreeHGlobal(ptr);
            return ret;
        }
        public struct ObserveCbParam
        {
            public IntPtr Value;
            public int ValueSize;
            public IntPtr UserData;

            public ObserveCbParam(IntPtr val, int val_size, IntPtr ud)
            {
                Value = val;
                ValueSize = val_size;
                UserData = ud;
            }
        }
        //private class ObserverManager {
        //    private static ObserverManager eventManager;

        //    private class ObsUnityEvent :UnityEvent<NativeHandle, int, IntPtr> { };

        //    private Dictionary<IntPtr, Dictionary<int, ObsUnityEvent>> ObserserDictionry = null;

        //    public static ObserverManager instance
        //    {
        //        get
        //        {
        //            if (eventManager == null)
        //            {
        //                //eventManager = FindObjectOfType (typeof (ObserverManager)) as ObserverManager;
        //                eventManager = new ObserverManager();
        //                if (eventManager == null)
        //                {
        //                    Debug.LogError ("There needs to be one active EventManger script on a GameObject in your scene.");
        //                }
        //                else
        //                {
        //                    eventManager.Init (); 
        //                }
        //            }

        //            return eventManager;
        //        }
        //    }
        //    public void Init()
        //    {
        //        ObserserDictionry = new Dictionary<IntPtr, Dictionary<int, ObsUnityEvent>>();
        //    }
        //    public void AddObserver(NativeHandle hdl, int attr_id, UnityAction<NativeHandle, int, IntPtr> listener)
        //    {
        //        ObsUnityEvent thisEvent = null;
        //        Dictionary<int, ObsUnityEvent> obs_dic = null;
        //        if (instance.ObserserDictionry.TryGetValue (hdl, out obs_dic))
        //        {
        //            if (obs_dic.TryGetValue(attr_id, out thisEvent)) {
        //                thisEvent.AddListener (listener);
        //            } else
        //            {
        //                thisEvent = new ObsUnityEvent();
        //                thisEvent.AddListener (listener);
        //                obs_dic.Add(attr_id, thisEvent);
        //            }
        //        } 
        //        else
        //        {
        //            thisEvent = new ObsUnityEvent();
        //            thisEvent.AddListener (listener);
        //            obs_dic = new Dictionary<int, ObsUnityEvent>();
        //            obs_dic.Add(attr_id, thisEvent);
        //            instance.ObserserDictionry.Add (hdl, obs_dic);
        //        }
        //    }
        //    public void RemoveObserver(NativeHandle hdl, int attr_id, UnityAction<NativeHandle, int, IntPtr> listener)
        //    {
        //        if (eventManager == null) return;
        //        ObsUnityEvent thisEvent = null;
        //        Dictionary<int, ObsUnityEvent> dict = null;
        //        if (instance.ObserserDictionry.TryGetValue (hdl, out dict))
        //        {
        //            if (dict.TryGetValue(attr_id, out thisEvent)) {
        //                thisEvent.RemoveListener (listener);
        //            }
        //        }

        //    }
        //    public void TriggerEvent (NativeHandle hdl, int attr_id, IntPtr arg)
        //    {
        //        XDebug.Log("Trigger Event " + attr_id);
        //        ObsUnityEvent thisEvent = null;
        //        Dictionary<int, ObsUnityEvent> dict = null;
        //        if (instance.ObserserDictionry.TryGetValue (hdl, out dict))
        //        {
        //            if (dict.TryGetValue(attr_id, out thisEvent)) {
        //                XDebug.Log("Trigger invoke " + attr_id);
        //                thisEvent.Invoke(hdl, attr_id, arg);
        //            }
        //        }
        //        if (thisEvent == null)
        //        {
        //            XDebug.Log("No callback found");
        //        }
        //    }
        //}

        //private static void ObserveDelegate(NativeHandle native_handle, int attr_id, IntPtr val, int val_size, IntPtr ud)
        //{
        //    XDebug.Log("ObservDelegate " + attr_id);
        //    ObserverManager.instance.TriggerEvent(native_handle, attr_id, val);
        //}
        //public delegate void XAttributeObserveDelegate(NativeHandle native_handle, int attr_id, IntPtr param);
        //public static int Observe<E>(XHandle handle, E attr_id, XAttributeObserveDelegate dlg)
        //{
        //    ObserverManager.instance.AddObserver(handle.mNativeHandle, Convert.ToInt32(attr_id), new UnityAction<IntPtr, int, IntPtr>(dlg));
        //    return xdev_observe(handle.mNativeHandle, Convert.ToInt32(attr_id), ObserveDelegate, (IntPtr)0);
        //    //return xdev_observe(handle.mNativeHandle, Convert.ToInt32(attr_id), dlg, (IntPtr) 0);
        //}
//#if UNITY_ANDROID
        public static int CopyAssetsToPath(string dest_directory_path, string name_regex = ".*\\.(json|dat)$")
        {
            int ret = 0;
            using(AndroidJavaClass jc=new AndroidJavaClass("com.unity3d.player.UnityPlayer")){
				using(AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity")){
                    var clazz = new AndroidJavaClass("com.ximmerse.sdk.XDeviceApi2");
                    ret = clazz.CallStatic<int>("copyAsset2Directory",currentActivity, name_regex, dest_directory_path);
                }
            }
            return ret;
        }
//#endif //
        protected class ObserverParam
        {
            public object Observer;
            public bool IsUdExist;
            public GCHandle UdGch;
            public ObserverParam()
            {
                Observer = null;
                IsUdExist = false;
            }
            public ObserverParam(object observer, bool isUdExist, GCHandle udGch)
            {
                Observer = observer;
                UdGch = udGch;
                IsUdExist = isUdExist;
            }
        };
        protected static System.Collections.Generic.Dictionary<string, ObserverParam> ObserverDict = new System.Collections.Generic.Dictionary<string, ObserverParam>();
        public static int RegisterObserver<E, U>(XHandle handle, E attr_id, System.Delegate observer, U ud)
        {
            //GCHandle gch = GCHandle.Alloc(ud);
            ////GCHandle obs_gch = GCHandle.Alloc(observer);

            ////int ret = xdev_register_observer(handle.mNativeHandle, Convert.ToInt32(attr_id), GCHandle.ToIntPtr(obs_gch), GCHandle.ToIntPtr(gch));
            //int ret = xdev_register_observer(handle.mNativeHandle, Convert.ToInt32(attr_id), Marshal.GetFunctionPointerForDelegate(observer), GCHandle.ToIntPtr(gch));
            //gch.Free();
            //if (ret == 0)
            //{
            //    var key = "" + handle.mNativeHandle + attr_id + observer;
            //    ObserverDict.Add(key, observer);
            //    Console.WriteLine("xim register observer: " + key);
            //}
            ////obs_gch.Free();
            //return ret;
            bool isUdExist = (ud != null);
            GCHandle gch = GCHandle.Alloc(ud);
            
            var ud_ptr = GCHandle.ToIntPtr(gch);
            int ret = xdev_register_observer(handle.mNativeHandle, Convert.ToInt32(attr_id), Marshal.GetFunctionPointerForDelegate(observer), ud_ptr);
            //gch.Free();
            if (ret == 0)
            {
                var key = "" + handle.mNativeHandle + attr_id + observer;
                if (ObserverDict.ContainsKey(key))
                {
                    ObserverParam old_param = ObserverDict[key];
                    if (old_param.IsUdExist)
                        old_param.UdGch.Free();
                }
                ObserverParam param = new ObserverParam(observer, isUdExist, gch);
                ObserverDict.Add(key, param);
                Debug.Log("xim register observer: " + key + ", ud " + ud_ptr);
            } else {
                gch.Free();
            }
            return ret;
        }
        public static int UnregisterObserver<E>(XHandle handle, E attr_id, System.Delegate observer)
        {
            ////GCHandle gch = GCHandle.Alloc(observer);
            //int ret = xdev_unregister_observer(handle.mNativeHandle, Convert.ToInt32(attr_id), Marshal.GetFunctionPointerForDelegate(observer));
            ////gch.Free();
            //var key = "" + handle.mNativeHandle + attr_id + observer;
            //ObserverDict.Remove(key);
            int ret = xdev_unregister_observer(handle.mNativeHandle, Convert.ToInt32(attr_id), Marshal.GetFunctionPointerForDelegate(observer));
            //gch.Free();
            var key = "" + handle.mNativeHandle + attr_id + observer;
            if (ObserverDict.ContainsKey(key)) {
                ObserverParam old_param = ObserverDict[key];
                if (old_param.IsUdExist)
                    old_param.UdGch.Free();
                ObserverDict.Remove(key);
            }
            Debug.Log("xim unregister observer: " + key);
            return ret;
        }
#endregion

#region API Test

        private static void TestLog(string log, bool result)
        {
            if (sLogDelegate != null)
            {
                sLogDelegate((int) (result ? XLogLevels.kXLogLevel_Info : XLogLevels.kXLogLevel_Error), "ApiTest", log);
            }
        }
        public static bool TestActions(XHandle xctx_hdl, XHandle xhmd_hdl, XHandle xctrl_0_hdl, XHandle xctrl_1_hdl)
        {
            bool result = true;
            int act_rs = 0;
            // kXAct_Get_CtxDeviceVersionInt,///< Deprecated

            //kXAct_GetInt_CtxSdkVersion
            act_rs = XDevicePlugin.GetInt(xctx_hdl, XDevicePlugin.XActions.kXAct_GetInt_CtxSdkVersion, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("kXAct_GetInt_CtxSdkVersion: " + act_rs, result);

            //kXAct_GetInt_CtxSDKALGVersion
            act_rs = XDevicePlugin.GetInt(xctx_hdl, XDevicePlugin.XActions.kXAct_GetInt_CtxSDKALGVersion, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("kXAct_GetInt_CtxSDKALGVersion: " + act_rs, result);

            // kXAct_GetInt_FPS
            act_rs = XDevicePlugin.GetInt(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetInt_FPS, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("hmd kXAct_GetInt_FPS: " + act_rs, result);
            act_rs = XDevicePlugin.GetInt(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_GetInt_FPS, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("c0 kXAct_GetInt_FPS: " + act_rs, result);
            act_rs = XDevicePlugin.GetInt(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_GetInt_FPS, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("c1 kXAct_GetInt_FPS: " + act_rs, result);
            //
            // kXAct_GetInt_ErrorCode
            act_rs = XDevicePlugin.GetInt(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetInt_ErrorCode, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("hmd kXAct_GetInt_ErrorCode: " + act_rs, result);
            //act_rs = XDevicePlugin.GetInt(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_GetInt_ErrorCode, -1);
            //XDebug.Assert(act_rs != -1);
            //result = result ? act_rs != -1 : result;
            //TestLog("c0 kXAct_GetInt_ErrorCode: " + act_rs, result);
            //act_rs = XDevicePlugin.GetInt(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_GetInt_ErrorCode, -1);
            //XDebug.Assert(act_rs != -1);
            //result = result ? act_rs != -1 : result;
            //TestLog("c1 kXAct_GetInt_ErrorCode: " + act_rs, result);

            //kXAct_Get_ConnectionStateInt
            act_rs = XDevicePlugin.GetInt(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetInt_ConnectionState, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("hmd kXAct_GetInt_ConnectionState: " + act_rs, result);
            act_rs = XDevicePlugin.GetInt(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_GetInt_ConnectionState, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("c0 kXAct_GetInt_ConnectionState: " + act_rs, result);
            act_rs = XDevicePlugin.GetInt(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_GetInt_ConnectionState, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("c1 kXAct_GetInt_ConnectionState: " + act_rs, result);

            // kXAct_GetInt_BlobID, // Reverved
            // kXAct_Set_BlogID, // Reverved

            // kXAct_GetInt_Battery,
            act_rs = XDevicePlugin.GetInt(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetInt_Battery, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("hmd kXAct_GetInt_Battery: " + act_rs, result);
            act_rs = XDevicePlugin.GetInt(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_GetInt_Battery, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("c0 kXAct_GetInt_Battery: " + act_rs, result);
            act_rs = XDevicePlugin.GetInt(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_GetInt_Battery, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("c1 kXAct_GetInt_Battery: " + act_rs, result);

            // kXAct_GetInt_BatteryMode
            act_rs = XDevicePlugin.GetInt(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetInt_BatteryMode, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("hmd kXAct_GetInt_BatteryMode: " + act_rs, result);

            // kXAct_GetInt_BatteryVoltage, 
            act_rs = XDevicePlugin.GetInt(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetInt_BatteryVoltage, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("hmd kXAct_GetInt_BatteryVoltage: " + act_rs, result);
            //act_rs = XDevicePlugin.GetInt(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_GetInt_BatteryVoltage, -1);
            //Debug.Assert(act_rs != -1);
            //result = result ? act_rs != -1 : result;
            //TestLog("c0 kXAct_GetInt_BatteryVoltage: " + act_rs, result);
            //act_rs = XDevicePlugin.GetInt(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_GetInt_BatteryVoltage, -1);
            //Debug.Assert(act_rs != -1);
            //result = result ? act_rs != -1 : result;
            //TestLog("c1 kXAct_GetInt_BatteryVoltage: " + act_rs, result);

            // kXAct_GetInt_BatteryTemperature, 
            act_rs = XDevicePlugin.GetInt(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetInt_BatteryTemperature, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("hmd kXAct_GetInt_BatteryTemperature: " + act_rs, result);
            act_rs = XDevicePlugin.GetInt(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_GetInt_BatteryTemperature, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("c0 kXAct_GetInt_BatteryTemperature: " + act_rs, result);
            act_rs = XDevicePlugin.GetInt(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_GetInt_BatteryTemperature, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("c1 kXAct_GetInt_BatteryTemperature: " + act_rs, result);

            // kXAct_GetStr_SoftwareRevision, ///< Gets firmware version and returns a string
            var str = XDevicePlugin.GetString(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetStr_SoftwareRevision, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("hmd kXAct_GetStr_SoftwareRevision: " + (str != null ? str : "") , result);
            str = XDevicePlugin.GetString(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_GetStr_SoftwareRevision, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("ctrl0 kXAct_GetStr_SoftwareRevision: " + (str != null ? str : ""), result);
            str = XDevicePlugin.GetString(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_GetStr_SoftwareRevision, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("ctrl1 kXAct_GetStr_SoftwareRevision: " + (str != null ? str : ""), result);

            // kXAct_GetStr_HardwareRevision, ///< Gets hardware version and returns a string
            str = XDevicePlugin.GetString(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetStr_HardwareRevision, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("hmd kXAct_GetStr_HardwareRevision: " + str, result);
            str = XDevicePlugin.GetString(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_GetStr_HardwareRevision, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("ctrl0 kXAct_GetStr_HardwareRevision: " + str, result);
            str = XDevicePlugin.GetString(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_GetStr_HardwareRevision, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("ctrl1 kXAct_GetStr_HardwareRevision: " + str, result);

            // kXAct_GetStr_FPGAVersion
            str = XDevicePlugin.GetString(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetStr_FPGAVersion, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("hmd kXAct_GetStr_FPGAVersion: " + str, result);

            //kXAct_GetStr_ModelName
            str = XDevicePlugin.GetString(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetStr_ModelName, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("hmd kXAct_GetStr_ModelName: " + str, result);
            str = XDevicePlugin.GetString(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_GetStr_ModelName, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("ctrl0 kXAct_GetStr_ModelName: " + str, result);
            str = XDevicePlugin.GetString(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_GetStr_ModelName, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("ctrl1 kXAct_GetStr_ModelName: " + str, result);

            // kXAct_GetStr_DisplayName
            str = XDevicePlugin.GetString(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetStr_DisplayName, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("hmd kXAct_GetStr_DisplayName: " + str, result);
            str = XDevicePlugin.GetString(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_GetStr_DisplayName, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("ctrl0 kXAct_GetStr_DisplayName: " + str, result);
            str = XDevicePlugin.GetString(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_GetStr_DisplayName, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("ctrl1 kXAct_GetStr_DisplayName: " + str, result);

            // kXAct_Get_ALGVersionStr
            str = XDevicePlugin.GetString(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetStr_ALGVersion, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("hmd kXAct_GetStr_ALGVersion: " + str, result);

            // kXAct_GetStr_SerialNumber
            str = XDevicePlugin.GetString(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetStr_SerialNumber, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("hmd kXAct_GetStr_SerialNumber: " + str, result);
            str = XDevicePlugin.GetString(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_GetStr_SerialNumber, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("ctrl0 kXAct_GetStr_SerialNumber: " + str, result);
            str = XDevicePlugin.GetString(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_GetStr_SerialNumber, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("ctrl1 kXAct_GetStr_SerialNumber: " + str, result);

            // kXAct_GetStr_ManufacturerName
            str = XDevicePlugin.GetString(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_GetStr_ManufacturerName, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("ctrl0 kXAct_GetStr_ManufacturerName: " + str, result);
            str = XDevicePlugin.GetString(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_GetStr_ManufacturerName, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("ctrl1 kXAct_GetStr_ManufacturerName: " + str, result);

            // kXAct_Get_FirmwareRevisionStr
            str = XDevicePlugin.GetString(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_GetStr_ManufacturerName, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("ctrl0 kXAct_GetStr_ManufacturerName: " + str, result);
            str = XDevicePlugin.GetString(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_GetStr_ManufacturerName, null);
            XDebug.Assert(str != null);
            result = result ? str != null : result;
            TestLog("ctrl1 kXAct_Get_ManufacturerNameStr: " + str, result);

            // kXAct_GetImuInfo
            ActParam_IMUInfo imu_info = new ActParam_IMUInfo();
            act_rs = XDevicePlugin.DoAction(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetImuInfo, ref imu_info);
            XDebug.Assert(act_rs >= 0);
            result = result ? act_rs >= 0 : result;
            TestLog("hmd,  kXAct_GetImuInfo: timestamp " 
                + imu_info.timestamp 
                + ", acce : " + imu_info.accelerometer[0] + " " + imu_info.accelerometer[1] + " " + imu_info.accelerometer[2]
                + ", gyro : " + imu_info.gyroscope[0] + " " + imu_info.gyroscope[1] + " " + imu_info.gyroscope[2],
                result);
            act_rs = XDevicePlugin.DoAction(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_GetImuInfo, ref imu_info);
            XDebug.Assert(act_rs >= 0);
            result = result ? act_rs >= 0 : result;
            TestLog("ctrl 0, kXAct_GetImuInfo: timestamp " 
                + imu_info.timestamp 
                + ", acce : " + imu_info.accelerometer[0] + " " + imu_info.accelerometer[1] + " " + imu_info.accelerometer[2]
                + ", gyro : " + imu_info.gyroscope[0] + " " + imu_info.gyroscope[1] + " " + imu_info.gyroscope[2],
                result);
            act_rs = XDevicePlugin.DoAction(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_GetImuInfo, ref imu_info);
            XDebug.Assert(act_rs >= 0);
            result = result ? act_rs >= 0 : result;
            TestLog("ctrl 1, kXAct_GetImuInfo: timestamp " 
                + imu_info.timestamp 
                + ", acce : " + imu_info.accelerometer[0] + " " + imu_info.accelerometer[1] + " " + imu_info.accelerometer[2]
                + ", gyro : " + imu_info.gyroscope[0] + " " + imu_info.gyroscope[1] + " " + imu_info.gyroscope[2],
                result);
            // kXAct_GetImuInfo_V, // Invalid in Unity

            // kXAct_Get6DofInfo, 
            ActParam_6DofInfo sizdof_info = new ActParam_6DofInfo();
            act_rs = XDevicePlugin.DoAction(xhmd_hdl, XDevicePlugin.XActions.kXAct_Get6DofInfo, ref sizdof_info);
            XDebug.Assert(act_rs >= 0);
            result = result ? act_rs >= 0 : result;
            TestLog("hmd,  kXAct_Get6DofInfo: timestamp " 
                + sizdof_info.timestamp 
                + ", position : " + sizdof_info.position[0] + " " + sizdof_info.position[1] + " " + sizdof_info.position[2]
                + ", rotation : " + sizdof_info.rotation[0] + " " + sizdof_info.rotation[1] + " " + sizdof_info.rotation[2] + " " + sizdof_info.rotation[3],
                result);
            act_rs = XDevicePlugin.DoAction(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_Get6DofInfo, ref sizdof_info);
            XDebug.Assert(act_rs >= 0);
            result = result ? act_rs >= 0 : result;
            TestLog("ctrl 0, kXAct_Get6DofInfo: timestamp " 
                + sizdof_info.timestamp 
                + ", position : " + sizdof_info.position[0] + " " + sizdof_info.position[1] + " " + sizdof_info.position[2]
                + ", rotation : " + sizdof_info.rotation[0] + " " + sizdof_info.rotation[1] + " " + sizdof_info.rotation[2] + " " + sizdof_info.rotation[3],
                result);
            act_rs = XDevicePlugin.DoAction(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_Get6DofInfo, ref sizdof_info);
            XDebug.Assert(act_rs >= 0);
            result = result ? act_rs >= 0 : result;
            TestLog("ctrl 1, kXAct_Get6DofInfo: timestamp " 
                + sizdof_info.timestamp 
                + ", position : " + sizdof_info.position[0] + " " + sizdof_info.position[1] + " " + sizdof_info.position[2]
                + ", rotation : " + sizdof_info.rotation[0] + " " + sizdof_info.rotation[1] + " " + sizdof_info.rotation[2] + " " + sizdof_info.rotation[3],
                result);

            // kXAct_Get6DofInfo_V, // invalid in Unity

            // kXAct_GetInt_ButtonBits, 
            act_rs = XDevicePlugin.GetInt(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetInt_ButtonBits, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("hmd kXAct_GetInt_ButtonBits: " + act_rs, result);
            act_rs = XDevicePlugin.GetInt(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_GetInt_ButtonBits, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("ctrl0 kXAct_GetInt_ButtonBits: " + act_rs, result);
            act_rs = XDevicePlugin.GetInt(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_GetInt_ButtonBits, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("ctrl1 kXAct_GetInt_ButtonBits: " + act_rs, result);

            // kXAct_Get_TouchPadState, // reserved
            // kXAct_Set_VPUTime, // reserved

            // kXAct_UnpairController, // See the test demo
            // kXAct_UnpairAllControllers,// See the test demo
            // kXAct_StartPairingController,// See the test demo
            // kXAct_StopPairingController,// See the test demo

            // kXAct_GetInt_PairedNumber,
            act_rs = XDevicePlugin.GetInt(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetInt_PairedNumber, -1);
            XDebug.Assert(act_rs != -1);
            result = result ? act_rs != -1 : result;
            TestLog("hmd kXAct_GetInt_PairedNumber: " + act_rs, result);

            // kXAct_GetPairedList, // reserved

            // kXAct_Vibrate,	// See the test demo
            // kXAct_Vibrate_V, // Invalid for Unity
            // kXAct_Sleep, // See the test demo
            // kXAct_Wakeup, // See the test demo

            // kXAct_GetControllerState, 
            XDevicePlugin.ActParam_ControllerState state = new XDevicePlugin.ActParam_ControllerState();
            act_rs = XDevicePlugin.DoAction(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetControllerState, ref state);
            XDebug.Assert(act_rs >= 0);
            result = result ? act_rs >= 0 : result;
            TestLog("hmd,  kXAct_GetControllerState: timestamp " 
                + state.timestamp 
                + ", position : " + state.position[0] + " " + state.position[1] + " " + state.position[2]
                + ", rotation : " + state.rotation[0] + " " + state.rotation[1] + " " + state.rotation[2] + " " + state.rotation[3]
                + ", acce:" + state.accelerometer[0] + " "  + state.accelerometer[1] + " " + state.accelerometer[2]
                + ", gyroscope:" + state.gyroscope[0] + " "  + state.gyroscope[1] + " " + state.gyroscope[2]
                + ", button bits : " + state.button_state.ToString("X")
                ,result);
            act_rs = XDevicePlugin.DoAction(xctrl_0_hdl, XDevicePlugin.XActions.kXAct_GetControllerState, ref state);
            XDebug.Assert(act_rs >= 0);
            result = result ? act_rs >= 0 : result;
            TestLog("c0,  kXAct_GetControllerState: timestamp " 
                + state.timestamp 
                + ", position : " + state.position[0] + " " + state.position[1] + " " + state.position[2]
                + ", rotation : " + state.rotation[0] + " " + state.rotation[1] + " " + state.rotation[2] + " " + state.rotation[3]
                + ", acce:" + state.accelerometer[0] + " "  + state.accelerometer[1] + " " + state.accelerometer[2]
                + ", gyroscope:" + state.gyroscope[0] + " "  + state.gyroscope[1] + " " + state.gyroscope[2]
                + ", button bits : " + state.button_state.ToString("X")
                ,result);
            act_rs = XDevicePlugin.DoAction(xctrl_1_hdl, XDevicePlugin.XActions.kXAct_GetControllerState, ref state);
            XDebug.Assert(act_rs >= 0);
            result = result ? act_rs >= 0 : result;
            TestLog("c1,  kXAct_GetControllerState: timestamp " 
                + state.timestamp 
                + ", position : " + state.position[0] + " " + state.position[1] + " " + state.position[2]
                + ", rotation : " + state.rotation[0] + " " + state.rotation[1] + " " + state.rotation[2] + " " + state.rotation[3]
                + ", acce:" + state.accelerometer[0] + " "  + state.accelerometer[1] + " " + state.accelerometer[2]
                + ", gyroscope:" + state.gyroscope[0] + " "  + state.gyroscope[1] + " " + state.gyroscope[2]
                + ", button bits : " + state.button_state.ToString("X")
                ,result);


            // kXAct_LoadMarkerSettingFile, // See the test demo
            // kXAct_ResetMarkerSettings,  // See the test demo

            // kXAct_LoadCameraCalibrationFile, // Reserved
            // kXAct_SetPositionSmooth, // Reserved

            // kXAct_GetMarkerInfo, 
            XDevicePlugin.ActParam_MarkerInfo marker_info = new XDevicePlugin.ActParam_MarkerInfo(0);
            act_rs = XDevicePlugin.DoAction(xhmd_hdl, XDevicePlugin.XActions.kXAct_GetMarkerInfo, ref marker_info);
            XDebug.Assert(act_rs >= 0);
            result = result ? act_rs >= 0 : result;
            TestLog("kXAct_GetMarkerInfo: timestamp " + marker_info.timestamp
                + ", Tracking state " + marker_info.state
                + ", position : " + marker_info.position[0] + " " + marker_info.position[1] + " " + marker_info.position[2]
                + ", rotation : " + marker_info.rotation[0] + " " + marker_info.rotation[1] + " " + marker_info.rotation[2] + " " + marker_info.rotation[3]
                , result);

            // kXAct_Get_MarkerInfo_V, // Invalid in Unity

            // kXAct_ConnectControllerByBindID,	///< Connect to a paired controller by specifying bind ID.
            XDevicePlugin.DoAction(xhmd_hdl, XDevicePlugin.XActions.kXAct_ConnectControllerByBindID, 1);

            // kXAct_ConnectAllPairedControllers, ///< Connect to All Paired Controllers.
            XDevicePlugin.DoAction(xhmd_hdl, XDevicePlugin.XActions.kXAct_ConnectAllPairedControllers);

            // kXAct_DisconnectControllerByIndex, ///< Disconnect a connected controller by specifying connected index.
            XDevicePlugin.DoAction(xhmd_hdl, XDevicePlugin.XActions.kXAct_DisconnectControllerByIndex, 0);

            //kXAct_DisconnectControllerByBindID, ///< Disconnect a connected controller by specifying bind ID.
            XDevicePlugin.DoAction(xhmd_hdl, XDevicePlugin.XActions.kXAct_DisconnectControllerByBindID, 1);

            // kXAct_DisconnectAllControllers, ///< Disconnect all connected Controllers.
            XDevicePlugin.DoAction(xhmd_hdl, XDevicePlugin.XActions.kXAct_DisconnectAllControllers);

            return result;
        }
        #endregion

        #region Deprecated
        private static System.Collections.Generic.Dictionary<XActions, XVpuAttributes> mXHawkAttrDic = null;
        private static System.Collections.Generic.Dictionary<XActions, XControllerAttributes> mXCAttrDic = null;
        private static System.Collections.Generic.Dictionary<XActions, XContextAttributes> mXCtxAttrDic = null;
        private static object lock_xhaw_dict = new object();
        private static object lock_ctrl_dict = new object();
        private static object lock_ctx_dict = new object();
        private static int ConvertAction2AttrID(XHandle handle, XActions act_id)
        {
            int ret = -1;
            if (handle.mName.Equals("XHawk-0"))
            {
                if (mXHawkAttrDic == null) {
                    lock(lock_xhaw_dict);
                    if (mXHawkAttrDic == null) {
                        var dict = new System.Collections.Generic.Dictionary<XActions, XVpuAttributes>();
                        dict.Add(XActions.kXAct_GetInt_FPS, XVpuAttributes.kXVpuAttr_Int_FpgaFps);
                        dict.Add(XActions.kXAct_GetInt_ErrorCode, XVpuAttributes.kXVpuAttr_Int_ErrorCode);
                        dict.Add(XActions.kXAct_GetInt_ConnectionState, XVpuAttributes.kXVpuAttr_Int_ConnectionState);
                        dict.Add(XActions.kXAct_GetInt_Battery, XVpuAttributes.kXVpuAttr_Int_Battery);
                        dict.Add(XActions.kXAct_GetInt_BatteryMode, XVpuAttributes.kXVpuAttr_Int_PowerMode);
                        dict.Add(XActions.kXAct_GetInt_BatteryVoltage, XVpuAttributes.kXVpuAttr_Int_BatteryVoltage);
                        dict.Add(XActions.kXAct_GetInt_BatteryTemperature, XVpuAttributes.kXVpuAttr_Int_BatteryTemperature);
                        dict.Add(XActions.kXAct_GetStr_SoftwareRevision, XVpuAttributes.kXVpuAttr_Str_SoftwareRevision);
                        dict.Add(XActions.kXAct_GetStr_HardwareRevision, XVpuAttributes.kXVpuAttr_Str_HardwareRevision);
                        dict.Add(XActions.kXAct_GetStr_FPGAVersion, XVpuAttributes.kXVpuAttr_Str_FPGAVersion);
                        dict.Add(XActions.kXAct_GetStr_ModelName, XVpuAttributes.kXVpuAttr_Str_ModelName);
                        dict.Add(XActions.kXAct_GetStr_DisplayName, XVpuAttributes.kXVpuAttr_Str_DeviceName);
                        dict.Add(XActions.kXAct_GetStr_ALGVersion, XVpuAttributes.kXVpuAttr_Str_ALGVersion);
                        dict.Add(XActions.kXAct_GetStr_SerialNumber, XVpuAttributes.kXVpuAttr_Str_SerialNumber);
                        //dict.Add(XActions.kXAct_GetStr_ManufacturerName, XVpuAttributes.kXVpuAttr_Int_ErrorCode);
                        //dict.Add(XActions.kXAct_GetStr_FirmwareRevision, XVpuAttributes.kXVpuAttr_StInt_ErrorCode);
                        dict.Add(XActions.kXAct_GetImuInfo, XVpuAttributes.kXVpuAttr_Obj_ImuInfo);
                        dict.Add(XActions.kXAct_Get6DofInfo, XVpuAttributes.kXVpuAttr_Obj_6DofInfo);
                        dict.Add(XActions.kXAct_GetInt_ButtonBits, XVpuAttributes.kXVpuAttr_Int_ButtonBits);
                        //dict.Add(XActions.kXAct_Get_TouchPadState, XVpuAttributes.kXVpuAttr_Int_ErrorCode);
                        dict.Add(XActions.kXAct_GetInt_PairedNumber, XVpuAttributes.kXVpuAttr_Int_PairedNumber);
                        dict.Add(XActions.kXAct_GetControllerState, XVpuAttributes.kXVpuAttr_Obj_ControllerState);
                        dict.Add(XActions.kXAct_GetMarkerInfo, XVpuAttributes.kXVpuAttr_Obj_TrackingInfo);
                        mXHawkAttrDic = dict;
                    }
                }
                if (mXHawkAttrDic.ContainsKey(act_id))
                    ret = (int) mXHawkAttrDic[act_id];
            } else if (handle.mName.StartsWith("XCobra-"))
            {
                if (mXCAttrDic == null)
                {
                    lock(lock_ctrl_dict);
                    if (mXCAttrDic == null) {
                        var dict = new System.Collections.Generic.Dictionary<XActions, XControllerAttributes>();
                    
                        dict.Add(XActions.kXAct_GetInt_FPS, XControllerAttributes.kXCAttr_Int_ImuFps);
                        dict.Add(XActions.kXAct_GetInt_ErrorCode, XControllerAttributes.kXCAttr_Int_ErrorCode);
                        dict.Add(XActions.kXAct_GetInt_ConnectionState, XControllerAttributes.kXCAttr_Int_ConnectionState);
                        dict.Add(XActions.kXAct_GetInt_Battery, XControllerAttributes.kXCAttr_Int_Battery);
                        dict.Add(XActions.kXAct_GetInt_BatteryMode, XControllerAttributes.kXCAttr_Int_PowerMode);
                        dict.Add(XActions.kXAct_GetInt_BatteryVoltage, XControllerAttributes.kXCAttr_Int_BatteryVoltage);
                        dict.Add(XActions.kXAct_GetInt_BatteryTemperature, XControllerAttributes.kXCAttr_Int_BatteryTemperature);
                        dict.Add(XActions.kXAct_GetStr_SoftwareRevision, XControllerAttributes.kXCAttr_Str_SoftwareRevision);
                        dict.Add(XActions.kXAct_GetStr_HardwareRevision, XControllerAttributes.kXCAttr_Str_HardwareRevision);
                        dict.Add(XActions.kXAct_GetStr_ModelName, XControllerAttributes.kXCAttr_Str_ModelName);
                        dict.Add(XActions.kXAct_GetStr_DisplayName, XControllerAttributes.kXCAttr_Str_DeviceName);
                        dict.Add(XActions.kXAct_GetStr_SerialNumber, XControllerAttributes.kXCAttr_Str_SerialNumber);
                        dict.Add(XActions.kXAct_GetStr_ManufacturerName, XControllerAttributes.kXCAttr_Str_ManufacturerName);
                        dict.Add(XActions.kXAct_GetStr_FirmwareRevision, XControllerAttributes.kXCAttr_Str_FirmwareRevision);
                        dict.Add(XActions.kXAct_GetImuInfo, XControllerAttributes.kXCAttr_Obj_ImuInfo);
                        dict.Add(XActions.kXAct_Get6DofInfo, XControllerAttributes.kXCAttr_Obj_6DofInfo);
                        dict.Add(XActions.kXAct_GetInt_ButtonBits, XControllerAttributes.kXCAttr_Int_ButtonBits);
                        dict.Add(XActions.kXAct_Get_TouchPadState, XControllerAttributes.kXCAttr_Obj_TouchPadState);
                        dict.Add(XActions.kXAct_GetControllerState, XControllerAttributes.kXCAttr_Obj_ControllerState);
                        mXCAttrDic = dict;
                    }
                }
                if (mXCAttrDic.ContainsKey(act_id))
                    ret = (int) mXCAttrDic[act_id];
            } else if (handle.mName.StartsWith("XCtx-"))
            {
                if (mXCtxAttrDic == null)
                {
                    lock(lock_ctx_dict);
                    if (mXCtxAttrDic == null) {
                        var dict = new System.Collections.Generic.Dictionary<XActions, XContextAttributes>();
                        //mXCtxAttrDic.Add(XActions.kXAct_GetInt_CtxDeviceVersion, XContextAttributes.kXVpuAttr_Int_FpgaFps);
                        dict.Add(XActions.kXAct_GetInt_CtxSDKALGVersion, XContextAttributes.kXCtxAttr_Int_SDKALGVersion);
                        dict.Add(XActions.kXAct_GetInt_CtxSdkVersion, XContextAttributes.kXCtxAttr_Int_SdkVersion);
                        mXCtxAttrDic = dict;
                    }
                }
                if (mXCtxAttrDic.ContainsKey(act_id))
                    ret = (int) mXCtxAttrDic[act_id];
            }

            //XDebug.Log("Convert action " + act_id + "to attr " + ret + ", hdl " + handle.mName);
            return ret;
        }
        
        //public static bool GetBool(XHandle handle, XActions act, bool default_value = false)
        //{
        //    return GetBool(handle, ConvertAction2AttrID(handle, act), default_value);;
        //}
        //private static int GetInt(XHandle handle, XActions act_id, int default_val)
        //{
        //    return GetInt(handle,  ConvertAction2AttrID(handle, act_id), default_val);
        //}
        //private static string GetString(XHandle hdl, XActions act_id, string dft_val)
        //{
        //    return GetString(hdl, ConvertAction2AttrID(hdl, act_id), dft_val);
        //}
        ///////////////////////////////////////////////
        /// \struct ActParam_6DofInfo
        /// \brief Structure for gettting 6-Dof information
        /// \deprecated use \ref XAttr6DofInfo
        [StructLayout(LayoutKind.Sequential)]
        public struct ActParam_6DofInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] position; ///< float buffer for gettting position values
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] rotation; ///< float buffer for getting quaternions of rotation.

            public System.UInt64 timestamp; ///< timestamp when data get from VPU

            /////////////////////////////////////
            /// \brief  Create structure
            /// \return Structure pointer       
            public static ActParam_6DofInfo Obtain()
            {
                return new ActParam_6DofInfo(0);
            }

            /////////////////////////////////////
            /// \brief  constructor
            ///////////////////////////////////// 
            public ActParam_6DofInfo(float init_val)
            {
                position = new float[3];
                rotation = new float[4];
                timestamp = 0;
            }
        }

        //////////////////////////////////////////////
        /// \struct ActParam_IMUInfo
        /// \brief Structure for getting IMU information.        
        /// \deprecated Use \ref XAttrImuInfo
        [StructLayout(LayoutKind.Sequential)]
        public struct ActParam_IMUInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] accelerometer;   ///< float buffer for getting accelerometer values.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] gyroscope;   ///< float buffer for getting gyroscope values.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] magnetism; /// < float buffer for getting magnetism values.
                
            public System.UInt64 timestamp; ///< timestamp when data get from VPU

            public static ActParam_IMUInfo Obtain()
            {
                return new ActParam_IMUInfo(0);
            }

            public ActParam_IMUInfo(float init_val)
            {
                // Motion
                accelerometer = new float[3];
                gyroscope = new float[3];
                magnetism = new float[3];
                timestamp = 0;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        /// \struct ActParam_VibrateArgs
        /// \brief The structure that sets the vibration parameters is used by the command of kXAct_Get_6DofInfo
        [StructLayout(LayoutKind.Sequential)]
        public struct ActParam_VibrateArgs {
            int strenght; ///< The strength of the vibration ranges from 0 to 100
            int duration; ///< The duration of the shaking, if it's 0, it's in milliseconds
            public ActParam_VibrateArgs(int _strenght, int _duration)
            {
                strenght = _strenght;
                duration = _duration;
            }
        };

                

        //////////////////////////////////////////////////////////////////
        /// @struct ActParam_TouchpadState
        /// @brief Touchpad State structure for \ref kXAct_Get_TouchPadState
        [StructLayout(LayoutKind.Sequential)]
        public struct ActParam_TouchpadState
        {
            public bool pressed; ///< indicated touchpad is touching.
            public float x; ///< x coordinate value, from 0 to 1.
            public float y; ///< y coordinate value, from 0 to 1.
        }

        //////////////////////////////////////////////////////////////////////////
        /// \struct ActParam_MarkerInfo
        /// \brief The structure is Marker information returned by the calling command of
        /// \deprecated Use \ref XAttrTrackingInfo 
        [StructLayout(LayoutKind.Sequential)]
        public struct ActParam_MarkerInfo
        {
            public int index; ///< Marker ID of frame,-1 un-available, otherwise available
            public int state; ///< The current tracking status of the Marker,please see TrackingResult for the status value definition
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] position; ///< float buffer for gettting position values
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] rotation; ///< float buffer for gettting rotation values

            public System.UInt64 timestamp; ///< timestamp when data get from VPU

            public ActParam_MarkerInfo(int _index)
            {
                index = _index;
                state = 0;
                position = new float[3];
                rotation = new float[4];
                timestamp = 0;
            }
        }
        
        /////////////////////////////////////////////////////////////////////////
        /// \struct ActParam_ControllerState
        /// \brief The structure for returning controller state by calling DoAction with kXAct_GetControllerState
        /// \deprecated Use \ref XAttrControllerState
        public struct ActParam_ControllerState {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] rotation;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] position;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] accelerometer;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] gyroscope;
            public System.UInt64 timestamp;
            public System.UInt32 button_state; /// < bit map indicating button pressed state of controller.
            
            // TouchPad, reserve
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public float[] axes;
        };
        #endregion // Deprecated
    }
}
