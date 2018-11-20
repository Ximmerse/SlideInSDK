using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Initialize slide in SDK unity editor.
    /// </summary>
    public static class InitializeSlideInSDKUnityEditor
    {
        [MenuItem ("Tools/Slide in/Initialize SDK")]
        public static void InitializeSdk ()
        {
            string[] includeShaders = new string[1]{
                "Unlit/Color"
            };

            SerializedObject graphicsSettings = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/GraphicsSettings.asset") [0]);
            SerializedProperty it = graphicsSettings.GetIterator ();
            SerializedProperty dataPoint;
            while (it.NextVisible(true)) {
                if (it.name == "m_AlwaysIncludedShaders") {
                    
                    for (int i = 0; i < includeShaders.Length; i++) 
                    { 
                        Shader shader = Shader.Find(includeShaders[i]);
                        bool exists = false;
                        for (int prop = 0; prop < it.arraySize; prop++)
                        {
                            if(it.GetArrayElementAtIndex(prop).objectReferenceValue != null && 
                               it.GetArrayElementAtIndex(prop).objectReferenceValue == shader)//already included:
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (exists == false)
                        {
                            it.InsertArrayElementAtIndex(i);
                            dataPoint = it.GetArrayElementAtIndex (i);
                            dataPoint.objectReferenceValue = Shader.Find(includeShaders[i]);
                        }
                    }

                    graphicsSettings.ApplyModifiedProperties ();
                }
            }

            Debug.LogFormat("Slide in SDK initialized");
        }
    }
}