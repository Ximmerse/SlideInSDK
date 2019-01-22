using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// default trackable behavior : enable/disable children renderer when gain/lost tracking.
    /// </summary>
    [RequireComponent(typeof(MarkerIdentity))]
    public class DefaultTrackableBehaviour : MonoBehaviour
    {

        MarkerIdentity mkIdentity;

        public GameObject[] ActivationObjects;

        void Awake()
        {
            mkIdentity = GetComponent<MarkerIdentity>();
        }

        void Start()
        {
            _OnMarkerVisibilityIsChanged(false);//disable children renderers at awake
        }

        // Use this for initialization
        void OnEnable()
        {
            _OnMarkerVisibilityIsChanged (mkIdentity.IsVisible);
            mkIdentity.VisibilityChanged += _OnMarkerVisibilityIsChanged;
        }

        void OnDisable()
        {
            mkIdentity.VisibilityChanged -= _OnMarkerVisibilityIsChanged;
        }

        /// <summary>
        /// on marker visibility is changed.
        /// </summary>
        /// <param name="visible">If set to <c>true</c> visible.</param>
        virtual protected void _OnMarkerVisibilityIsChanged (bool visible)
        {
            if(ActivationObjects != null && ActivationObjects.Length > 0)
            {
                for (int i = 0; i < ActivationObjects.Length; i++)
                {
                    var go = ActivationObjects[i];
                    go.SetActive(visible);
                }
            }
        }
    }
}