using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Wrapper class to hold slide in device handle reference.
    /// </summary>
    internal static class DevicerHandle
    {
        static Ximmerse.InputSystem.XDevicePlugin.XHandle _sSlideInCtx;

        public static Ximmerse.InputSystem.XDevicePlugin.XHandle SlideInContext
        {
            get
            {
                return _sSlideInCtx;
            }
            set
            {
                _sSlideInCtx = value;
            }
        }



        static Ximmerse.InputSystem.XDevicePlugin.XHandle _sHmdHandle;

        /// <summary>
        /// HMD device handler, tag : "XHawk-0"
        /// </summary>
        /// <value>The hmd hander.</value>
        public static Ximmerse.InputSystem.XDevicePlugin.XHandle HmdHandle
        {
            get
            {
                return _sHmdHandle;
            }
            set
            {
                _sHmdHandle = value;
            }
        }


        static Ximmerse.InputSystem.XDevicePlugin.XHandle _sCtrlHandle01;

        /// <summary>
        /// Gets the controller01 device handle.
        /// </summary>
        /// <value>The controller01.</value>
        public static Ximmerse.InputSystem.XDevicePlugin.XHandle Controller01
        {
            get
            {
                if (_sCtrlHandle01 == null)
                    _sCtrlHandle01 = Ximmerse.InputSystem.XDevicePlugin.GetDeviceHandle(DevicerHandle.SlideInContext, "XCobra-0");
                return _sCtrlHandle01;
            }
           
        }


        static Ximmerse.InputSystem.XDevicePlugin.XHandle _sCtrlHandle02;

        /// <summary>
        /// Gets the controller02 device handle.
        /// </summary>
        /// <value>The controller01.</value>
        public static Ximmerse.InputSystem.XDevicePlugin.XHandle Controller02
        {
            get
            {
                if (_sCtrlHandle02 == null)
                    _sCtrlHandle02 = Ximmerse.InputSystem.XDevicePlugin.GetDeviceHandle(DevicerHandle.SlideInContext, "XCobra-1");
                return _sCtrlHandle02;
            }
        }

        /// <summary>
        /// Get Controller on the specified index.
        /// </summary>
        /// <param name="index">Index.</param>
        public static Ximmerse.InputSystem.XDevicePlugin.XHandle GetController(int index = 0)
        {
            if(index == 0)
            {
                return Controller01;
            }
            else 
            {
                return Controller02;
            }
        }
    }
}