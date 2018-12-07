using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ximmerse.InputSystem;


namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Generic Ximmerse bluetooth controller input interface.
    /// </summary>
    public static class XimmerseControllerInput
    {
        static bool isInitialized = false;
        static Controller controller01 = null, controller02 = null;
        static float[] emptyRaw = new float[4];

        /// <summary>
        /// The time threshold for button tap.
        /// </summary>
        public static float TapTimeThreshold = 1;

        /// <summary>
        /// Initialize the interface.
        /// </summary>
        public static void Initialize ()
        {
            if (!isInitialized)
            {
                controller01 = new Controller(Controller.ControllerIndex.Controller01);
                controller02 = new Controller(Controller.ControllerIndex.Controller02);
                isInitialized = true;
            }
        }

        /// <summary>
        /// Update the input system.
        /// Should be call one per frame.
        /// </summary>
        public static void Update ()
        {
            if (controller01.IsConnected())
                controller01.Update();
            if (controller02.IsConnected())
                controller02.Update();  
        }

        /// <summary>
        /// Starts pairing.
        /// </summary>
        public static void StartPairing(XDevicePlugin.XControllerTypes controllerType)
        {
            XDevicePlugin.DoAction(DevicerHandle.HmdHandle, XDevicePlugin.XActions.kXAct_StartPairingController, (int)controllerType);
        }

        /// <summary>
        /// Gets paired device count.
        /// </summary>
        /// <param name="PairedNumber">Paired number.</param>
        public static int GetPairedDeviceCount()
        {
            int PairedNumber = XDevicePlugin.GetInt(DevicerHandle.HmdHandle, XDevicePlugin.XVpuAttributes.kXVpuAttr_Int_PairedNumber, 0);
            return PairedNumber;
        }
        /// <summary>
        /// Connects all paired device.
        /// </summary>
        public static void ConnectAll()
        {
            int result = XDevicePlugin.DoAction(DevicerHandle.HmdHandle, XDevicePlugin.XActions.kXAct_ConnectAllPairedControllers);
            Debug.LogFormat("Connect-All result: {0}", result);
        }

        public static void DisconnectAll()
        {
            int result = XDevicePlugin.DoAction(DevicerHandle.HmdHandle, XDevicePlugin.XActions.kXAct_DisconnectAllControllers);
            Debug.LogFormat("Disconnect result: {0}", result);
        }

        /// <summary>
        /// Disconnect the controller at the index.
        /// </summary>
        /// <param name="Index">Index.</param>
        public static void Disconnect (int Index)
        {
            XDevicePlugin.DoAction(DevicerHandle.HmdHandle, XDevicePlugin.XActions.kXAct_DisconnectControllerByIndex, Index);
        }

        /// <summary>
        /// Stops pairing.
        /// </summary>
        public static void StopPairing ()
        {
            int ret = XDevicePlugin.DoAction(DevicerHandle.HmdHandle, XDevicePlugin.XActions.kXAct_StopPairingController);
            Debug.LogFormat ("Stop pairing result : {0}", ret);
        }


        /// <summary>
        /// Unpairs all.
        /// </summary>
        public static void UnpairAll ()
        {
            int ret3 = XDevicePlugin.DoAction(DevicerHandle.HmdHandle, XDevicePlugin.XActions.kXAct_UnpairAllControllers);
            Debug.LogFormat ("Unpair all controllers: {0}",  ret3);
        }

        /// <summary>
        /// Unpair the specified controllerIndex.
        /// </summary>
        /// <param name="controllerIndex">Controller index.</param>
        public static void Unpair (int controllerIndex)
        {
            var ret = XDevicePlugin.DoAction(DevicerHandle.HmdHandle, XDevicePlugin.XActions.kXAct_UnpairController, controllerIndex);
            Debug.LogFormat ("Unpair controller: {0}, result: {1}", controllerIndex, ret);
        }

        /// <summary>
        /// Check if controller 01 is connected.
        /// </summary>
        public static bool IsControllerConnected (int Index)
        {
            if(Application.platform==RuntimePlatform.OSXEditor || Application.platform==RuntimePlatform.OSXPlayer)
            {
                return false;
            }
            if (Index == 0)
                return controller01.IsConnected();
            else if (Index == 1)
                return controller02.IsConnected();
            else
                throw new UnityException("Invalid index : must be 0 or 1");
        }


        /// <summary>
        /// Vibrate the controller of index with specified strength, duration.
        /// If index = -1, will vibrate any connected controller.
        /// You should pass -1 | 0 | 1 only, if Index == 0 or Index == -1, vibrate the controller at the index.
        /// </summary>
        /// <param name="strength">Strength.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="ControllerIndex">Controller index.</param>
        public static void Vibrate(int strength, float duration, int Index = -1)
        {
            if (Index == -1)
            {
                if (controller01.IsConnected())
                    controller01.Vibrate(strength, duration);

                if (controller02.IsConnected())
                    controller02.Vibrate(strength, duration);
            }
            if (Index == 0 && controller01.IsConnected())
            {
                controller01.Vibrate(strength, duration);
            }
            else if (Index == 1 && controller02.IsConnected())
            {
                controller02.Vibrate(strength, duration);
            }
        }

        /// <summary>
        /// Gets the input controller rotation.
        /// Passing controller index = -1 to return the first connected controller's rotation.
        /// If you want to target specific controller index, passing 0 for controller 01, passing 1 for controller 02.
        /// </summary>
        /// <returns>The input rotation.</returns>
        /// <param name="ControllerIndex">Controller index.</param>
        public static Quaternion GetInputRotation(int ControllerIndex = -1)
        {
            if (ControllerIndex == -1)
            {
                if (controller01.IsConnected())
                    return controller01.Rotation;
                else if (controller02.IsConnected())
                    return controller02.Rotation;

                return Quaternion.identity;
            }
            else if (ControllerIndex == 0)
            {
                return controller01.IsConnected() ? controller01.Rotation : Quaternion.identity;
            }
            else if (ControllerIndex == 1)
            {
                return controller02.IsConnected() ? controller02.Rotation : Quaternion.identity;
            }
            return Quaternion.identity;
        }


        /// <summary>
        /// Returns a 4 length array represents raw rotation elements.
        /// </summary>
        /// <returns>The raw rotation.</returns>
        /// <param name="ControllerIndex">Controller index.</param>
        public static float[] GetRawRotation (int ControllerIndex = -1)
        {
            if (ControllerIndex == -1)
            {
                if (controller01.IsConnected())
                    return controller01.RawRotation;
                else if (controller02.IsConnected())
                    return controller02.RawRotation;

                return emptyRaw;
            }
            else if (ControllerIndex == 0)
            {
                return controller01.IsConnected() ? controller01.RawRotation : emptyRaw;
            }
            else if (ControllerIndex == 1)
            {
                return controller02.IsConnected() ? controller02.RawRotation : emptyRaw;
            }
            return emptyRaw;
        }

        /// <summary>
        /// Gets the input controller gyroscope.
        /// Passing controller index = -1 to return the first connected controller's gyroscope.
        /// If you want to target specific controller index, passing 0 for controller 01, passing 1 for controller 02.
        /// </summary>
        /// <returns>The input rotation.</returns>
        /// <param name="ControllerIndex">Controller index.</param>
        public static Vector3 GetInputGyroscope(int ControllerIndex = -1)
        {
            if (ControllerIndex == -1)
            {
                if (controller01.IsConnected())
                    return controller01.Gyroscope;
                else if (controller02.IsConnected())
                    return controller02.Gyroscope;

                return Vector3.zero;
            }
            else if (ControllerIndex == 0)
            {
                return controller01.IsConnected() ? controller01.Gyroscope : Vector3.zero;
            }
            else if (ControllerIndex == 1)
            {
                return controller02.IsConnected() ? controller02.Gyroscope : Vector3.zero;
            }
            return Vector3.zero;
        }
        /// <summary>
        /// Gets the input controller acceleration.
        /// Passing controller index = -1 to return the first connected controller's acceleration.
        /// If you want to target specific controller index, passing 0 for controller 01, passing 1 for controller 02.
        /// </summary>
        /// <returns>The input rotation.</returns>
        /// <param name="ControllerIndex">Controller index.</param>
        public static Vector3 GetInputAcceleration(int ControllerIndex = -1)
        {
            if (ControllerIndex == -1)
            {
                if (controller01.IsConnected())
                    return controller01.Acceleration;
                else if (controller02.IsConnected())
                    return controller02.Acceleration;

                return Vector3.zero;
            }
            else if (ControllerIndex == 0)
            {
                return controller01.IsConnected() ? controller01.Acceleration : Vector3.zero;
            }
            else if (ControllerIndex == 1)
            {
                return controller02.IsConnected() ? controller02.Acceleration : Vector3.zero;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// Recenter the controller IMU rotation
        /// </summary>
        /// <param name="ControllerIndex">Controller index.</param>
        public static void RecenterControllerIMURotation (float yaw = 0, int ControllerIndex=  -1)
        {
            if (ControllerIndex == -1)
            {
                if (controller01.IsConnected())
                    controller01.RecenterControllerRotation(yaw);

                if (controller02.IsConnected())
                    controller02.RecenterControllerRotation(yaw);
            }
            else if (ControllerIndex == 0)
            {
                if (controller01.IsConnected())
                    controller01.RecenterControllerRotation(yaw);
            }
            else if (ControllerIndex == 1)
            {
                if (controller02.IsConnected())
                    controller02.RecenterControllerRotation(yaw);
            }
        }

        /// <summary>
        /// Check if the button is currently pressing down. 
        /// Passing controller index = -1 means both controller 01 or 02 can trigger a key down result.
        /// If you want to target specific controller index, passing 0 for controller 01, passing 1 for controller 02.
        /// </summary>
        /// <returns><c>true</c> if is key the specified button ControllerIndex; otherwise, <c>false</c>.</returns>
        /// <param name="button">Button.</param>
        /// <param name="ControllerIndex">Controller index.</param>
        public static bool IsKey (ControllerButton button, int ControllerIndex = -1)
        {
            if (ControllerIndex == -1)
            {
                return controller01.GetButtonState(button).isDown || controller02.GetButtonState(button).isDown;
            }
            else if (ControllerIndex == 0)
            {
                return controller01.GetButtonState(button).isDown;
            }
            else if (ControllerIndex == 1)
            {
                return controller02.GetButtonState(button).isDown;
            }
            else
            {
                throw new UnityException("Invalid controller index: " + ControllerIndex + ".Must be [-1 | 0 | 1]");
            }
        }

        /// <summary>
        /// Check if the button is currently loosing up. 
        /// Passing controller index = -1 means both controller 01 or 02 can trigger a key down result.
        /// If you want to target specific controller index, passing 0 for controller 01, passing 1 for controller 02.
        /// </summary>
        /// <returns><c>true</c> if is key the specified button ControllerIndex; otherwise, <c>false</c>.</returns>
        /// <param name="button">Button.</param>
        /// <param name="ControllerIndex">Controller index.</param>
        public static bool IsKeyUp (ControllerButton button, int ControllerIndex = -1)
        {
            if (ControllerIndex == -1)
            {
                return controller01.GetButtonState(button).isUp || controller02.GetButtonState(button).isUp;
            }
            else if (ControllerIndex == 0)
            {
                return controller01.GetButtonState(button).isUp;
            }
            else if (ControllerIndex == 1)
            {
                return controller02.GetButtonState(button).isUp;
            }
            else
            {
                throw new UnityException("Invalid controller index: " + ControllerIndex + ".Must be [-1 | 0 | 1]");
            }
        }

        /// <summary>
        /// Check if the button has been triggering a tap event.
        /// Passing controller index = -1 means both controller 01 or 02 can trigger a key down result.
        /// If you want to target specific controller index, passing 0 for controller 01, passing 1 for controller 02.
        /// </summary>
        /// <returns><c>true</c> if is key the specified button ControllerIndex; otherwise, <c>false</c>.</returns>
        /// <param name="button">Button.</param>
        /// <param name="ControllerIndex">Controller index.</param>
        public static bool IsTap (ControllerButton button, int ControllerIndex = -1)
        {
            if (ControllerIndex == -1)
            {
                return controller01.GetButtonState(button).isTap || controller02.GetButtonState(button).isTap;
            }
            else if (ControllerIndex == 0)
            {
                return controller01.GetButtonState(button).isTap;
            }
            else if (ControllerIndex == 1)
            {
                return controller02.GetButtonState(button).isTap;
            }
            else
            {
                throw new UnityException("Invalid controller index: " + ControllerIndex + ".Must be [-1 | 0 | 1]");
            }
        }


        /// <summary>
        /// Gets the controller01 rotation.
        /// </summary>
        /// <returns>The controller01 rotation.</returns>
        public static Quaternion GetController01Rotation()
        {
            XDevicePlugin.ActParam_ControllerState state = new XDevicePlugin.ActParam_ControllerState();
            XDevicePlugin.DoAction(DevicerHandle.Controller01, XDevicePlugin.XActions.kXAct_GetControllerState, ref state);
            return new Quaternion(state.rotation[0],state.rotation[1],state.rotation[2],state.rotation[3]);
        }

        /// <summary>
        /// Gets the touch-pad.
        /// Center point = 0. 
        /// left top = -0.5,
        /// </summary>
        /// <returns><c>true</c>, if touch pad was gotten, <c>false</c> otherwise.</returns>
        /// <param name="touchPad">Touch pad.</param>
        public static bool GetTouchPad(out Vector2 touchPad, int controllerIndex = 0)
        {
            //NOT supported:
            if(Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
            {
                touchPad=Vector2.zero;
                return false;
            }
            XDevicePlugin.ActParam_TouchpadState tpad_st = new XDevicePlugin.ActParam_TouchpadState();
            int tpad_act_rs = XDevicePlugin.DoAction(DevicerHandle.GetController(controllerIndex), XDevicePlugin.XActions.kXAct_Get_TouchPadState, ref tpad_st);
            if (tpad_act_rs >= 0 && tpad_st.pressed) 
            {
                touchPad = new Vector2((tpad_st.x - 0.5f) / 0.5f, ((tpad_st.y - 0.5f) * -1) / 0.5f);
                return true;
            }
            else
            {
                touchPad = Vector2.zero;
                return false;
            }
        }
    }


}