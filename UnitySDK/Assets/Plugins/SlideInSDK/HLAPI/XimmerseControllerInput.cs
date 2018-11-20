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


    /// <summary>
    /// Wrapper controller for LLAPI controller logic
    /// </summary>
    internal class Controller 
    {
        public enum ControllerIndex
        {
            Controller01 = 0, Controller02 = 1
        }

        int indexInt;

        public int Index
        {
            get
            {
                return indexInt;
            }
        }

        static ControllerButton[] sAllButtons = null;

        Ximmerse.InputSystem.XDevicePlugin.XHandle ctrlHandle;

        DeviceConnectionState m_state;

        internal struct ButtonState 
        {
            public bool isDown;//for key down frame == true
            public bool isUp;//for key up frame == true
            public float DownTime;//record key down time
            public bool isTap;//for key up frame and (up - down) time <= tap threshold time
        }

        Dictionary<ControllerButton, ButtonState> buttonStates = new Dictionary<ControllerButton, ButtonState>();

        public Controller (ControllerIndex Index)
        {
            if (Index == ControllerIndex.Controller01)
            {
                ctrlHandle = DevicerHandle.Controller01;
                indexInt = 0;
            }
            else
            {
                ctrlHandle = DevicerHandle.Controller02;
                indexInt = 1;
            }

            m_state = (DeviceConnectionState)XDevicePlugin.GetInt(this.ctrlHandle, XDevicePlugin.XVpuAttributes.kXVpuAttr_Int_ConnectionState, (int) DeviceConnectionState.Disconnected);


            //Add LLAPI event listener:
            XDevicePlugin.Observe(ctrlHandle, XDevicePlugin.XControllerAttributes.kXCAttr_Int_ConnectionState, AttributeObserveDelegate);


            if (sAllButtons == null)
            {
                var controllerButtons = System.Enum.GetValues(typeof(ControllerButton));
                sAllButtons = new ControllerButton[controllerButtons.Length];
                for (int i = 0; i < controllerButtons.Length; i++)
                {
                    sAllButtons[i] = (ControllerButton)controllerButtons.GetValue(i);
                }
            }

            foreach (var button in sAllButtons)
            {
                buttonStates.Add(button, new ButtonState());
            }

        }

        void AttributeObserveDelegate(System.IntPtr handle, int attr_id, System.IntPtr arg)
        {
            switch ((XDevicePlugin.XControllerAttributes)attr_id)
            {
                case XDevicePlugin.XControllerAttributes.kXCAttr_Int_ConnectionState:
                    m_state = (DeviceConnectionState)System.Runtime.InteropServices.Marshal.ReadInt32(arg);
                    Debug.LogFormat("Connection state : {0}", m_state);
                    break;
            }
        }

        public bool IsConnected ()
        {
            return m_state == DeviceConnectionState.Connected;
        }

        public DeviceConnectionState GetState ()
        {
            return m_state;
        }

        public void Disconnect ()
        {
            XimmerseControllerInput.Disconnect(this.indexInt);
        }

        public void Unpair ()
        {
            XimmerseControllerInput.Unpair(this.indexInt);
        }

        public ButtonState GetButtonState (ControllerButton Button)
        {
            return buttonStates[Button];
        }

        internal bool CheckKey (ControllerButton Button)
        {
            XDevicePlugin.XAttrControllerState state = new XDevicePlugin.XAttrControllerState();
            XDevicePlugin.GetObject(this.ctrlHandle, XDevicePlugin.XControllerAttributes.kXCAttr_Obj_ControllerState, ref state);
            bool isKeyDown = (state.button_state & (uint)Button) != 0;
            return isKeyDown;
        }


        /// <summary>
        /// Vibrate the controller of index with specified strength, duration.
        /// </summary>
        /// <param name="strength">Strength.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="ControllerIndex">Controller index.</param>
        public void Vibrate(int strength, float duration)
        {
            XDevicePlugin.ActParam_VibrateArgs arg = new XDevicePlugin.ActParam_VibrateArgs( strength, (int)(duration * 1000));
            XDevicePlugin.DoAction(this.ctrlHandle, XDevicePlugin.XActions.kXAct_Vibrate, arg);
        }

        /// <summary>
        /// Update this controller.
        /// </summary>
        public void Update ()
        {
            if (this.IsConnected() == false)
                return;
            
            for (int i = 0; i < sAllButtons.Length; i++)
            {
                var btn = sAllButtons[i];
                var btnState = buttonStates[btn];
                bool isBtnDown = CheckKey(btn);//is button currently pressing down at this frame.

                //Key down frame:
                if (btnState.isDown == false && isBtnDown)
                {
                    btnState.isDown = true;
                    btnState.isUp = false;
                    btnState.DownTime = Time.time;
                    buttonStates[btn] = btnState;//update button state
                }
                //Key up frame:
                else if (btnState.isDown == true && isBtnDown == false)
                {
                    btnState.isDown = false;
                    btnState.isUp = true;
                    if ((Time.time - btnState.DownTime) <= XimmerseControllerInput.TapTimeThreshold)
                    {
                        btnState.isTap = true;
                    }
                    buttonStates[btn] = btnState;//update button state
                }
                //earse key tap | key up:
                else if ((btnState.isTap == true && isBtnDown == false) 
                    || (btnState.isUp == true && isBtnDown == false))
                {
                    btnState.isDown = false;
                    btnState.isTap = false;
                    btnState.isUp = false;
                    buttonStates[btn] = btnState;//update button state
                }
            }
        }

    }
}