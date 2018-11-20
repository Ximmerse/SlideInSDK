using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ControllerButton = Ximmerse.InputSystem.ControllerButton;
using UnityEngine.Events;
using System.Linq;


namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Ximmerse bluetooth controller module, driving ximmerse LLAPI input module.
    /// </summary>
    public class XimmerseInputModule : MonoBehaviour
    {
        void Start ()
        {
            XimmerseControllerInput.Initialize();
        }

        void Update ()
        {
            XimmerseControllerInput.Update();
        }
    }
}