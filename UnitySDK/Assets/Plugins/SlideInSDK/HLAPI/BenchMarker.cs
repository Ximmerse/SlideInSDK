using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Bench Marker behaviour - use as World Center for 6DoF tracking mode.
    /// </summary>
    [DisallowMultipleComponent]
    public class BenchMarker : MarkerTargetBehaviour
    {
        [Space, Header ("Bench marker")]
        [SerializeField, Tooltip ("For 100mm X 4 benchmarker group, need append a fixed quaterion offset to fix the pose error.")]
        bool m_FixedQuaternionError = true;

        /// <summary>
        /// For 100mm X 4 benchmarker group, need append a fixed quaterion offset to fix the pose error.
        /// </summary>
        /// <value><c>true</c> if fixed quaternion error; otherwise, <c>false</c>.</value>
        public bool FixedQuaternionError
        {
            get
            {
                return m_FixedQuaternionError;
            }
            set
            {
                m_FixedQuaternionError = value;
            }
        }

        protected virtual void OnEnable ()
        {
            ARCamera.Singleton.HeadMode = ARCameraTrackingMode.Static;//restrict AR camera to be static
        }

        protected virtual void LateUpdate ()
        {
            BenmarkerUpdate_AlgorithmRotating();
        }

        protected Quaternion prevRawQ;

        /// <summary>
        /// Updates benmarker system with algorithm rotating.
        /// </summary>
        void BenmarkerUpdate_AlgorithmRotating ()
        {
            Vector3 rawPos = Vector3.zero;
            Quaternion rawRot = Quaternion.identity;
            if (!TagTrackingUtil.GetMarkerState(this.MarkerID, out rawPos, out rawRot))
            {
                return;
            }
            if (m_FixedQuaternionError)
            {
                rawRot = rawRot * Quaternion.Euler(-90, 0, 0);
            }
            var trackingAnchor = TagTrackingUtil.GetNode(UnityEngine.XR.XRNode.TrackingReference);
            var head = TagTrackingUtil.GetNode(UnityEngine.XR.XRNode.CenterEye);

            if(this.SmoothMarkerRotation)
            {
                Quaternion rawRot1 = default(Quaternion);
                switch(this.smoothRotationMethod)
                {
                    case SmoothRotationMethod.Lerp:
                        rawRot1 = Quaternion.Lerp (prevRawQ, rawRot, this.SmoothRotationLerpRate);
                        break;

                    case SmoothRotationMethod.Angular:
                        rawRot1 = Quaternion.RotateTowards (prevRawQ, rawRot, this.SmoothRotationAnglarSpeed * Time.deltaTime);
                        break;
                }

                prevRawQ = rawRot;

                rawRot = rawRot1;
            }
            else 
            {
                prevRawQ = rawRot;
            }


            Matrix4x4 markerWorldTRS = Matrix4x4.TRS (trackingAnchor.TransformPoint (rawPos), trackingAnchor.rotation * rawRot, Vector3.one).inverse;
            var headInMarkerWorldXYZ = markerWorldTRS.MultiplyPoint3x4(head.position);
            var headInMarkerWorldEuler = markerWorldTRS.rotation * head.rotation;

            if(this.PositionPrediction)
            {
                headInMarkerWorldXYZ = this.XYZPrediction.GetPrediction (headInMarkerWorldXYZ);
            }
            if(this.RotationPrediciton)
            {
                this.PosePrediction.Update (headInMarkerWorldEuler);
                headInMarkerWorldEuler = this.PosePrediction.GetRotationPrediction (headInMarkerWorldEuler, this.RotationPredictionDelayFrame);
            }

            var markerTrans = this.transform;
            head.position = markerTrans.TransformPoint (headInMarkerWorldXYZ);
            head.rotation = markerTrans.rotation * headInMarkerWorldEuler;

        }





    }
}