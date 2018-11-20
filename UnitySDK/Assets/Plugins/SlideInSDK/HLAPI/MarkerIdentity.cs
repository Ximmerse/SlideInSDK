using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Marker identity.Represent a marker object.
    /// </summary>
    public class MarkerIdentity : MonoBehaviour
    {
        internal static List<MarkerIdentity> sAllMarkers = new List<MarkerIdentity> ();
        [SerializeField]
        int m_MarkerID = 0;

        public int MarkerID
        {
            get
            {
                return m_MarkerID;
            }
            set
            {
                m_MarkerID = value;
            }
        }

        void OnEnable ()
        {
            sAllMarkers.Add(this);
        }

        void OnDisable ()
        {
            sAllMarkers.Remove(this);
        }

        bool m_IsVisible = false;

        /// <summary>
        /// Is the marker visible at current frame ?
        /// </summary>
        /// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
        public bool IsVisible 
        {
            get 
            {
                return m_IsVisible;
            }
            set 
            {
                if(value != m_IsVisible)
                {
                    m_IsVisible = value;
                    if(VisibilityChanged != null)
                    {
                        VisibilityChanged(m_IsVisible);
                    }
                }
            }
        }

        public event System.Action<bool> VisibilityChanged = null;

    }
}