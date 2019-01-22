using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.SlideInSDK
{

    [CreateAssetMenu(menuName = "Ximmerse/SlideInSDK/Tracking Profile", fileName = "TrackingProfile")]
    public class MarkerTrackingProfile : ScriptableObject
    {
        [Multiline(2)]
        public string Description;

        public enum MarkerTrackingType : int
        {
            ScatterCards = 2,

            MarkerGroup = 3,
        }

        [System.Serializable]
        public class TrackingItems 
        {
            public MarkerTrackingType trackingType = MarkerTrackingType.ScatterCards;
            #if UNITY_EDITOR

            /// <summary>
            /// Editor only 
            /// </summary>
            public UnityEngine.Object JSONConfig = null;
            #endif
            /// <summary>
            /// The name of the json file.
            /// </summary>
            [HideInInspector]
            public string jsonName;

        }

        /// <summary>
        /// Config the tracking items.
        /// </summary>
        [SerializeField]
        TrackingItems[] items = new TrackingItems[] { 
        };

        /// <summary>
        /// Gets the tracking items.
        /// </summary>
        /// <value>The tracking items.</value>
        public TrackingItems[] trackingItems
        {
            get
            {
                return items;
            }
        }

        #if UNITY_EDITOR
        void OnValidate ()
        {
            var items = this.trackingItems;
            if(items != null)
            {
                foreach(var item in items)
                {
                    item.jsonName = System.IO.Path.GetFileName(UnityEditor.AssetDatabase.GetAssetPath(item.JSONConfig));
                }
            }
        }
        #endif
    }
}