using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


namespace Ximmerse.SlideInSDK
{
    [CustomEditor(typeof(MarkerTrackingProfile))]
    public class MarkerTrackingProfileInspector : Editor
    {

        MarkerTrackingProfile mTarget 
        {
            get
            {
                return this.target as MarkerTrackingProfile;
            }
        }

        void OnEnable()
        {
            FindTuneTarget();
        }


        void OnDisable()
        {
            FindTuneTarget();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        void FindTuneTarget()
        {
            var items = mTarget.trackingItems;
            if(items != null)
            {
                foreach(var item in items)
                {
                    item.jsonName = Path.GetFileName(AssetDatabase.GetAssetPath(item.JSONConfig));
                }
            }
        }
    }
}