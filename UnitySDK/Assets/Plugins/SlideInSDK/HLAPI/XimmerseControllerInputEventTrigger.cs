using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ControllerButton = Ximmerse.InputSystem.ControllerButton;
using UnityEngine.Events;
using System.Linq;


namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Ximmerse controller input event trigger.
    /// </summary>
    public class XimmerseControllerInputEventTrigger : MonoBehaviour
    {
        [System.Serializable]
        public class EventTrigger
        {
            public ControllerButton Button;

            public ControllerButtonEvent Trigger = new ControllerButtonEvent();
        }

        [System.Serializable]
        public class ControllerButtonEvent : UnityEvent<ControllerButton>
        {

        }

        [Header ("Event : key tap")]
        public EventTrigger[] taps = new EventTrigger[] { };
       

        // Update is called once per frame
        void Update()
        {
           
            foreach (var e in taps)
            {
                if (XimmerseControllerInput.IsTap(e.Button))
                {
                    e.Trigger.Invoke(e.Button);
                }
            }
        }
    }
}