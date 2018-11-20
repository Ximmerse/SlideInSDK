using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Marker target behaviour inspector.
    /// </summary>
    [CustomEditor (typeof (MarkerTargetBehaviour), true)]
    public class MarkerTargetBehaviourInspector : Editor
    { 
        MarkerTargetBehaviour mMarkerTargetBehaviour { get { return this.target as MarkerTargetBehaviour; } }

        public override void OnInspectorGUI()
        {
            var m_ScriptProp = this.serializedObject.FindProperty("m_Script");
            var hasMaxTrackingDistanceProp = this.serializedObject.FindProperty ("m_HasLimitedTrackingDistance");
            var maxTrackingDistanceProp = this.serializedObject.FindProperty ("m_MaxTrackingDistance");
            var smoothMarkerPositionProp = this.serializedObject.FindProperty ("m_SmoothMarkerPosition");
            var smoothMarkerPositionMethodProp = this.serializedObject.FindProperty ("m_SmoothPositionMethod");

            var applyMarkerAlgorithmRotation = this.serializedObject.FindProperty ("m_ApplyAlgorithmRotation");
            var shouldSmoothRotationProperty = this.serializedObject.FindProperty ("m_SmoothMarkerRotation");
            var smoothRotationMethodProperty = this.serializedObject.FindProperty ("m_SmoothRotationMethod");

            var smoothRotationCurveProperty_LerpByDistnace = this.serializedObject.FindProperty ("m_SmoothRotation_LerpByDistance");
            var smoothRotationCurveProperty_AngularByDistance = this.serializedObject.FindProperty ("m_SmoothRotation_AngularByDistance");

            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.PropertyField(m_ScriptProp, true);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField (hasMaxTrackingDistanceProp, new GUIContent ("Limited Tracking", "Limit tracking distance?"), true);
            if(hasMaxTrackingDistanceProp.boolValue)
            {
                EditorGUILayout.PropertyField (maxTrackingDistanceProp, new GUIContent ("Max Distance", "Max tracked distance value"), true);
            }

            //Position:
            EditorGUILayout.PropertyField (smoothMarkerPositionProp, new GUIContent ("Smooth Tracking", "Smooth marker position"), true);
            if(smoothMarkerPositionProp.boolValue)
            {
                EditorGUILayout.PropertyField (smoothMarkerPositionMethodProp, new GUIContent ("Method", "Smooth marker position method"), true);
                switch (this.mMarkerTargetBehaviour.smoothPositionMethod)
                {
                    case MarkerTargetBehaviour.SmoothPositionMethod.Lerp:
                        EditorGUILayout.PropertyField (serializedObject.FindProperty ("m_PositionSmoothLerp"), new GUIContent ("XYZ Lerp", "Smooth marker position lerp rate"), true);
                        break;

                    case MarkerTargetBehaviour.SmoothPositionMethod.LerpByDistance:
                        EditorGUILayout.PropertyField (serializedObject.FindProperty ("m_SmoothPosition_LerpByDistance"), new GUIContent ("Lerp by distance.", "Lerp curve by distance. X=Distance, Y=Lerp"), true);
                        break;

                    case MarkerTargetBehaviour.SmoothPositionMethod.Velocity:
                        EditorGUILayout.PropertyField (serializedObject.FindProperty ("m_PositionalSmoothVelocity"), new GUIContent ("Smooth velocity.", "Smooth marker position velocity"), true);
                        break;

                    case MarkerTargetBehaviour.SmoothPositionMethod.VelocityByDistance:
                        EditorGUILayout.PropertyField (serializedObject.FindProperty ("m_SmoothPosition_VelocityByDistance"), new GUIContent ("Smoothy move in velocity by distance.", "X=Distance, Y=Velocity"), true);
                        break;
                }
            }

            //Rotation:
            EditorGUILayout.PropertyField (applyMarkerAlgorithmRotation, new GUIContent ("Apply Rotation", "Apply marker algorithm rotation"), true);
            if(applyMarkerAlgorithmRotation.boolValue)
            {
                EditorGUILayout.PropertyField (shouldSmoothRotationProperty, new GUIContent ("Smooth", "Smooth marker algorithm rotation "), true);
                if(shouldSmoothRotationProperty.boolValue)
                {
                    EditorGUILayout.PropertyField (smoothRotationMethodProperty, new GUIContent ("Method", "Smooth method"), true);
                    if (mMarkerTargetBehaviour.smoothRotationMethod == MarkerTargetBehaviour.SmoothRotationMethod.Lerp)
                    {
                        var SmoothRotationLerpRateProperty = this.serializedObject.FindProperty ("m_SmoothRotationLerpRate");
                        EditorGUILayout.PropertyField (SmoothRotationLerpRateProperty, new GUIContent ("Lerp", "Smooth lerp rate"), true);
                    }
                    else if (mMarkerTargetBehaviour.smoothRotationMethod == MarkerTargetBehaviour.SmoothRotationMethod.Angular)
                    {
                        var SmoothRotationAnglarSpeedProperty = this.serializedObject.FindProperty ("m_SmoothRotationAnglarSpeed");
                        EditorGUILayout.PropertyField (SmoothRotationAnglarSpeedProperty, new GUIContent ("Angular speed", "Smooth angular speed"), true);
                    }
                    else if (mMarkerTargetBehaviour.smoothRotationMethod == MarkerTargetBehaviour.SmoothRotationMethod.LerpByDistance)
                    {
                        EditorGUILayout.PropertyField (smoothRotationCurveProperty_LerpByDistnace, new GUIContent ("Lerp rate", "Smooth lerp rate by distance. X = tracked distance, Y = lerp rate."), true);
                    }
                    else if (mMarkerTargetBehaviour.smoothRotationMethod == MarkerTargetBehaviour.SmoothRotationMethod.AngularByDistance)
                    {
                        EditorGUILayout.PropertyField (smoothRotationCurveProperty_AngularByDistance, new GUIContent ("Angular speed", "Smooth angular velocity by distance. X = tracked distance, Y = angular speed."), true);
                    }
                }
            }

            var positionPredictionProperty = this.serializedObject.FindProperty ("m_PositionPrediction");
            EditorGUILayout.PropertyField (positionPredictionProperty, new GUIContent("On"), true);
            if(positionPredictionProperty.boolValue)
            {
                
                var smoothPositionPredictionProperty = this.serializedObject.FindProperty ("m_SmoothPositionPrediction");
                EditorGUILayout.PropertyField (smoothPositionPredictionProperty, new GUIContent("Smooth"), true);
                if(smoothPositionPredictionProperty.boolValue)
                {
                    EditorGUILayout.PropertyField(this.serializedObject.FindProperty ("m_SmoothPositionPredicitonSpeed"));
                }
            }


            var rotationPredicitonProperty = this.serializedObject.FindProperty ("m_RotationPrediciton");
            EditorGUILayout.PropertyField (rotationPredicitonProperty, new GUIContent("On"), true);
            if(rotationPredicitonProperty.boolValue)
            {
                var rotationPredictionFrameDelay = this.serializedObject.FindProperty ("m_RotationPredictionDelayFrame");
                EditorGUILayout.PropertyField (rotationPredictionFrameDelay, new GUIContent("Frame Delay"), true);
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}