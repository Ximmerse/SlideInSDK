using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Slide in sdk editor menu items.
    /// </summary>
    public static class SlideInSdkEditorMenu
    {
        /// <summary>
        /// Creates a marker target.
        /// </summary>
        [UnityEditor.MenuItem("GameObject/Create Other/Ximmerse/Slide In SDK/Dynamic Marker Target", false, 11)]
        public static void CreateMarker ()
        {
            GameObject markerTarget = new GameObject("Dynamic Marker Target", new System.Type[] { typeof(DynamicMarker) });
            EditorGUIUtility.PingObject(markerTarget);
        }

        /// <summary>
        /// Navigates to sdk resource folder.
        /// </summary>
        [UnityEditor.MenuItem("Assets/SDK/Resources", false, 12)]
        public static void NavigateTo_ProjectResource ()
        {
            var anything = Resources.Load<ReflectionLenProfileContainer>("LenProfiles");
            EditorGUIUtility.PingObject (anything);
        }
    }
}