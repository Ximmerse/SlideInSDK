using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ximmerse.InputSystem;

namespace Ximmerse.SlideInSDK
{

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

        Quaternion m_Rotation = Quaternion.identity;

        /// <summary>
        /// Gets the rotation of the input controller.
        /// </summary>
        /// <value>The rotation.</value>
        public Quaternion Rotation
        {
            get
            {
                return m_Rotation;
            }
        }

        float[] m_RawRotation = new float[4];

        /// <summary>
        /// Gets the raw rotation of the input controller.
        /// </summary>
        /// <value>The rotation.</value>
        public float[] RawRotation
        {
            get
            {
                return m_RawRotation;
            }
        }

        Vector3 m_Gyroscope = Vector3.zero;

        /// <summary>
        /// Gets the gyroscope (angular speed in degree)
        /// </summary>
        /// <value>The gyroscope.</value>
        public Vector3 Gyroscope
        {
            get
            {
                return m_Gyroscope;
            }
        }

        Vector3 m_Acceleration = Vector3.zero;

        /// <summary>
        /// Gets the acceleration value in meter/(sec * sec)
        /// </summary>
        /// <value>The acceleration.</value>
        public Vector3 Acceleration
        {
            get
            {
                return m_Acceleration;
            }
        }

        static ControllerButton[] sAllButtons = null;

        Ximmerse.InputSystem.XDevicePlugin.XHandle ctrlHandle;

        private Quaternion m_YawError = Quaternion.identity;

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
            XDevicePlugin.RegisterObserver(ctrlHandle, 
                XDevicePlugin.XControllerAttributes.kXCAttr_Int_ConnectionState,
                new XDevicePlugin.XDeviceConnectStateChangeDelegate(LLAPIConnectionStateChange), 
                ctrlHandle);

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

        void LLAPIConnectionStateChange(int connect_st, System.IntPtr ud)
        {
//            switch ((XDevicePlugin.XControllerAttributes)attr_id)
//            {
//                case XDevicePlugin.XControllerAttributes.kXCAttr_Int_ConnectionState:
//                    m_state = (DeviceConnectionState)System.Runtime.InteropServices.Marshal.ReadInt32(arg);
//                    Debug.LogFormat("Connection state : {0}", m_state);
//                    break;
//            }
            m_state = (DeviceConnectionState)connect_st;
            Debug.LogFormat("Connection state : {0}", m_state);
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
        /// Recenter the controller's IMU rotation by given yaw.
        /// </summary>
        /// <param name="Yaw">Yaw.</param>
        public void RecenterControllerRotation (float Yaw = 0)
        {
            Quaternion rawRotation = new Quaternion(m_RawRotation[1], m_RawRotation[0], m_RawRotation[2], m_RawRotation[3]);
            m_YawError = Quaternion.AngleAxis(Yaw - rawRotation.eulerAngles.y, Vector3.up); 
        }

        /// <summary>
        /// Update this controller.
        /// </summary>
        public void Update ()
        {
            if (this.IsConnected() == false)
                return;

            XDevicePlugin.XAttrControllerState state = new XDevicePlugin.XAttrControllerState();
            XDevicePlugin.GetObject(this.ctrlHandle, XDevicePlugin.XControllerAttributes.kXCAttr_Obj_ControllerState, ref state);

            m_RawRotation[0] = state.rotation[0];
            m_RawRotation[1] = state.rotation[1];
            m_RawRotation[2] = state.rotation[2];
            m_RawRotation[3] = state.rotation[3];

            Quaternion rawRotation = new Quaternion(state.rotation[1], state.rotation[0], state.rotation[2], state.rotation[3]);
            m_Rotation = m_YawError * rawRotation;

            this.m_Gyroscope.Set(state.gyroscope[0], state.gyroscope[1], state.gyroscope[2]);
            this.m_Acceleration.Set(state.accelerometer[0], state.accelerometer[1], state.accelerometer[2]);

            for (int i = 0; i < sAllButtons.Length; i++)
            {
                var btn = sAllButtons[i];
                ButtonState btnState = buttonStates[btn];
                bool isBtnDown = false;
                if (btn != ControllerButton.PrimaryTrigger)
                {
                    isBtnDown = (state.button_state & (uint)btn) != 0;
                }
                else
                {
                    int keyState = XDevicePlugin.GetInt(this.ctrlHandle, XDevicePlugin.XControllerAttributes.kXCAttr_Int_Trigger, 0);
                    isBtnDown = keyState != 0;
                }
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