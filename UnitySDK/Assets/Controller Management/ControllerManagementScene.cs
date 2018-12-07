using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ximmerse.SlideInSDK;
using UnityEngine.UI;
using Ximmerse.InputSystem;

namespace Ximmerse
{
    public class ControllerManagementScene : MonoBehaviour
    {

        public Text DeviceGeneralInfo, ButtonEventInfo;

        public Button btnStartPairing, btnUnpairAll, btnTestVibrate;

        public Button btnReload;

        public Dropdown controllerList;

        ControllerButton[] buttons = null;

        System.Text.StringBuilder buffer = new System.Text.StringBuilder();

        float buttonLogDirtyTime = 0;

        public float ButtonLogStayTime = 8;

        XDevicePlugin.XControllerTypes[] controllerTypes = new XDevicePlugin.XControllerTypes[]
            {
                XDevicePlugin.XControllerTypes.kXControllerPickUp, 
                XDevicePlugin.XControllerTypes.kXController3Dof, 
                XDevicePlugin.XControllerTypes.kXControllerDType, 
                XDevicePlugin.XControllerTypes.kXControllerKylo,
                XDevicePlugin.XControllerTypes.kXControllerSaber, 
                XDevicePlugin.XControllerTypes.kXControllerTag, 
            };

        void Awake ()
        {
            //initialize the device and create global handler:
            TagTrackingUtil.InitializeDeviceModule();

            XimmerseControllerInput.Initialize();

            var controllerButtons = System.Enum.GetValues(typeof(ControllerButton));
            buttons = new ControllerButton[controllerButtons.Length];
            for (int i = 0; i < controllerButtons.Length; i++)
            {
                buttons[i] = (ControllerButton)controllerButtons.GetValue(i);
            }

            btnStartPairing.onClick.AddListener(
                new UnityEngine.Events.UnityAction(
                    () =>
                    {
                        XimmerseControllerInput.StartPairing (controllerTypes[controllerList.value]);
                        Debug.LogFormat ("Start pairing controller type: {0}" , controllerTypes[controllerList.value]);
                    }));

            btnUnpairAll.onClick.AddListener(
                new UnityEngine.Events.UnityAction(
                    () =>
                    {
                        XimmerseControllerInput.UnpairAll ();
                    }));

            btnTestVibrate.onClick.AddListener(
                new UnityEngine.Events.UnityAction(
                    () =>
                    {
                        XimmerseControllerInput.Vibrate (1000, 0.5f);
                    }));

            btnReload.onClick.AddListener (
                new UnityEngine.Events.UnityAction(
                    () =>
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene (0);
                    }));
        }

        void OnEnable ()
        {
            StartCoroutine(RefreshStateUI());
        }

        void OnDisable ()
        {
            StopAllCoroutines();
        }

        IEnumerator RefreshStateUI ()
        {
            WaitForSeconds wait1s = new WaitForSeconds(1);
            while (true)
            {
                RefreshGeneralInfo();
                yield return wait1s;
            }
        }

        void RefreshGeneralInfo ()
        {
            string generalMsg = string.Format("Controller 01 state : {0} \r\nController 02 state : {1}", 
                XimmerseControllerInput.IsControllerConnected(0) ? "Connected" : "Disconnected", 
                XimmerseControllerInput.IsControllerConnected(1) ? "Connected" : "Disconnected");
            DeviceGeneralInfo.text = generalMsg;
        }

        void Update ()
        {
            XimmerseControllerInput.Update();

            if (buffer.Length > 0)
            {
                buffer.Remove(0, buffer.Length);
            }
            foreach (var ctrlBtn in buttons)
            {
                if (XimmerseControllerInput.IsControllerConnected(0) && XimmerseControllerInput.IsKey (ctrlBtn, 0))
                {
                    buffer.AppendLine(string.Format("Controller : {0}, key down button: {1}", 0, ctrlBtn));
                }
                if (XimmerseControllerInput.IsControllerConnected(0) && XimmerseControllerInput.IsKeyUp (ctrlBtn, 0))
                {
                    buffer.AppendLine(string.Format("Controller : {0}, key up button: {1}", 0, ctrlBtn));
                }
                if (XimmerseControllerInput.IsControllerConnected(0) && XimmerseControllerInput.IsTap (ctrlBtn, 0))
                {
                    buffer.AppendLine(string.Format("Controller : {0}, tap button: {1}", 0, ctrlBtn));
                }

                if (XimmerseControllerInput.IsControllerConnected(1) && XimmerseControllerInput.IsKey (ctrlBtn, 1))
                {
                    buffer.AppendLine(string.Format("Controller : {0}, key down button: {1}", 1, ctrlBtn));
                }
                if (XimmerseControllerInput.IsControllerConnected(1) && XimmerseControllerInput.IsKeyUp (ctrlBtn, 1))
                {
                    buffer.AppendLine(string.Format("Controller : {0}, key up button: {1}", 0, ctrlBtn));
                }
                if (XimmerseControllerInput.IsControllerConnected(1) && XimmerseControllerInput.IsTap (ctrlBtn, 1))
                {
                    buffer.AppendLine(string.Format("Controller : {0}, tap button: {1}", 0, ctrlBtn));
                }
            }
            if (buffer.Length > 0)
            {
                ButtonEventInfo.text = buffer.ToString();
                buttonLogDirtyTime = Time.time;
            }

            //Clear log:
            if (ButtonEventInfo.text != string.Empty && (Time.time - buttonLogDirtyTime)>=ButtonLogStayTime)
            {
                ButtonEventInfo.text = string.Empty;
            }
                
        }
    }



}