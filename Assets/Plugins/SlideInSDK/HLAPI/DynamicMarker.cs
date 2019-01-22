using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Dynamic marker - translate marker's pose per-frame.  
    /// </summary>
    [DisallowMultipleComponent]
    public class DynamicMarker : MarkerTargetBehaviour
    {
        /// <summary>
        /// The previous frame tracked raw position.
        /// </summary>
        protected Vector3 prevRawPosition;

        /// <summary>
        /// The previous frame tracked raw oritentation.
        /// </summary>
        protected Quaternion prevRawOritentation;

        /// <summary>
        /// The previous frame world position.
        /// </summary>
        protected Vector3 prevWorldPosition;

        /// <summary>
        /// The previous frame world rotation.
        /// </summary>
        protected Quaternion prevWorldRotation;

        void LateUpdate ()
        {
            UpdateMarkerPose();
        }

        protected void UpdateMarkerPose()
        {
            //raw pos : the tracked position retrieved from VPU
            Vector3 rawPos = Vector3.zero;
            //raw rot : the tracked oritentation retrieved from VPU
            Quaternion rawRot = Quaternion.identity;
            var markerTrans = this.transform;
            m_SubMarkerMask = 0;
            if (TagTrackingUtil.GetMarkerState(this.MarkerID, out rawPos, out rawRot, out this.m_SubMarkerMask))
            {
                //Anti-jiggering: markers become invisible when TOooo far:
                if(this.HasLimitedTrackingDistance && rawPos.magnitude >= MaxTrackingDistance)
                {
                    prevRawPosition = rawPos;
                    prevRawOritentation = rawRot;
                    prevWorldPosition = transform.position;
                    //too far
                    this.Identity.IsVisible = false;
                    return;
                }

                Vector3 rawTrackedPosition = rawPos;
                this.Identity.IsVisible = true;
                var trackingAnchor = TagTrackingUtil.GetNode(UnityEngine.XR.XRNode.TrackingReference);
               
               
                //smooth XYZ movement:
                if(this.SmoothMarkerPosition)
                {
                    switch(this.smoothPositionMethod)
                    {
                        case SmoothPositionMethod.Lerp:
                            rawTrackedPosition = Vector3.Lerp (prevRawPosition, rawPos, this.PositionSmoothLerp);
                            break;

                        case SmoothPositionMethod.Velocity:
                            rawTrackedPosition = Vector3.MoveTowards(prevRawPosition, rawPos, this.PositionSmoothVelocity * Time.deltaTime);
                            break;

                        case SmoothPositionMethod.LerpByDistance:
                            float distance = Vector3.Distance (rawPos, prevRawPosition);
                            float lerpByDistance = this.SmoothPositionCurveControl_LerpByDistance.Evaluate (distance);
                            rawTrackedPosition = Vector3.Lerp (prevRawPosition, rawPos, lerpByDistance);
                            break;


                        case SmoothPositionMethod.VelocityByDistance:
                            distance = Vector3.Distance (rawPos, prevRawPosition);
                            float velocityByDistance = this.SmoothPositionCurveControl_LerpByDistance.Evaluate (distance);
                            rawTrackedPosition = Vector3.MoveTowards(prevRawPosition, rawPos, velocityByDistance * Time.deltaTime);
                            break;
                    }
                }

                //basic tracking:
                Vector3 tPos = trackingAnchor.TransformPoint(rawTrackedPosition);

                //prediction:
                if(this.PositionPrediction)
                {
                    tPos = this.XYZPrediction.GetPrediction (tPos);
                }
               
                markerTrans.position = tPos;

                //Handle Rotation:
                if(this.ApplyAlgorithmRotation)
                {
                    Quaternion RawTrackedOritentation = rawRot;

                    if(this.SmoothMarkerRotation)
                    {
                        switch(this.smoothRotationMethod)
                        {
                            case SmoothRotationMethod.Lerp:
                                RawTrackedOritentation = Quaternion.Lerp (prevRawOritentation, rawRot, this.SmoothRotationLerpRate);
                                break;

                            case SmoothRotationMethod.Angular:
                                RawTrackedOritentation = Quaternion.RotateTowards (prevRawOritentation, rawRot, this.SmoothRotationAnglarSpeed * Time.deltaTime);
                                break;

                            case SmoothRotationMethod.LerpByDistance:
                                float distance = rawPos.magnitude;
                                float lerp = this.SmoothRotationCurveControl_LerpByDistance.Evaluate (distance);
                                RawTrackedOritentation = Quaternion.Lerp(prevRawOritentation, rawRot, lerp);
                                break;

                            case SmoothRotationMethod.AngularByDistance:
                                distance = rawPos.magnitude;
                                float angular = this.SmoothRotationCurveControl_AngularByDistance.Evaluate (distance);
                                RawTrackedOritentation = Quaternion.RotateTowards(prevRawOritentation, rawRot, angular * Time.deltaTime);
                                break;
                        }
                    }

                    Quaternion worldQ = trackingAnchor.rotation * RawTrackedOritentation;

                    if(this.RotationPrediciton)
                    {
                        this.PosePrediction.Update (worldQ);
                        worldQ = this.PosePrediction.GetRotationPrediction (worldQ, this.RotationPredictionDelayFrame);
                    }

                    markerTrans.rotation = worldQ;
                }

                prevRawPosition = rawPos;
                prevRawOritentation = rawRot;

                prevWorldPosition = transform.position;
                prevWorldRotation = transform.rotation;
            }
            else 
            {
                this.Identity.IsVisible = false;
            }
        }
    }
}