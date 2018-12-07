using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ximmerse.SlideInSDK;


public class ControllerAxis : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Quaternion rawRotation = XimmerseControllerInput.GetInputRotation();
        transform.rotation = rawRotation;
    }

    public void Recenter(float finalYaw)
    { 
        XimmerseControllerInput.RecenterControllerIMURotation(finalYaw);
    }

}
