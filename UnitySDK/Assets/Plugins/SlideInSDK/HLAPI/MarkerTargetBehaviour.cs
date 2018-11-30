using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Base class for marker target behaviour.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MarkerIdentity))]
    public abstract class MarkerTargetBehaviour : MonoBehaviour
    {


        public enum SmoothPositionMethod
        {
            /// <summary>
            /// Lerp between frames
            /// </summary>
            Lerp = 0,

            /// <summary>
            /// Move in velocity between frames
            /// </summary>
            Velocity = 1,

            /// <summary>
            /// Lerp between frames by distance.
            /// X = distance, Y = lerp rate.
            /// </summary>
            LerpByDistance = 2,

            /// <summary>
            /// Move in velocity between frames.
            /// X = distance, Y = velocity
            /// </summary>
            VelocityByDistance = 3,
        }


        public enum SmoothRotationMethod
        {
            /// <summary>
            /// Lerp between frames
            /// </summary>
            Lerp = 0,

            /// <summary>
            /// Angular rotating between frames
            /// </summary>
            Angular = 1,

            /// <summary>
            /// Lerp between frames by distance.
            /// X = distance, Y = lerp rate.
            /// </summary>
            LerpByDistance = 2,

            /// <summary>
            /// Angular rotating between frames.
            /// X = distance, Y = angular speed.
            /// </summary>
            AngularByDistance = 3,
        }

        MarkerIdentity m_Identity;

        /// <summary>
        /// Gets the identity.
        /// </summary>
        /// <value>The identity.</value>
        public MarkerIdentity Identity
        {
            get
            {
                if (!m_Identity)
                    m_Identity = GetComponent<MarkerIdentity>();
                return m_Identity;
            }
        }

        /// <summary>
        /// Gets or sets the marker ID.
        /// </summary>
        /// <value>The marker I.</value>
        public int MarkerID
        {
            get
            {
                return this.Identity.MarkerID;
            }
            set
            {
                this.Identity.MarkerID = value;
            }
        }

        #region Marker Tracking Properties

        [Header (" - Anti-Jiggering - ")]
        [SerializeField, Tooltip ("Has max tracking distance? when true, disable tracking when distance >= [m_HasLimitedTrackingDistance]")]
        bool m_HasLimitedTrackingDistance = false;

        /// <summary>
        /// If has limited tracking distance. 
        /// If true, when tracked distance >= MaxTrackingDistance, tracking will be disabled.
        /// </summary>
        /// <value><c>true</c> if this instance has limited tracking distance; otherwise, <c>false</c>.</value>
        public bool HasLimitedTrackingDistance
        {
            get
            {
                return m_HasLimitedTrackingDistance;
            }
            set
            {
                m_HasLimitedTrackingDistance = value;
            }
        }


        [SerializeField, Range(0,2), Tooltip ("Max tracking distance")]
        float m_MaxTrackingDistance = Mathf.Infinity;

        /// <summary>
        /// Gets or sets the max allowed tracking distance.
        /// </summary>
        /// <value>The max tracking distance.</value>
        public float MaxTrackingDistance
        {
            get
            {
                return m_MaxTrackingDistance;
            }
            set
            {
                m_MaxTrackingDistance = value;
            }
        }

        [Header (" - Position - ")]
        [SerializeField, Tooltip ("Smooth marker movement")]
        bool m_SmoothMarkerPosition = false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Ximmerse.SlideInSDK.MarkerTargetBehaviour"/> smooth
        /// marker position.
        /// </summary>
        /// <value><c>true</c> if smooth marker position; otherwise, <c>false</c>.</value>
        public bool SmoothMarkerPosition
        {
            get
            {
                return m_SmoothMarkerPosition;
            }
            set
            {
                m_SmoothMarkerPosition = value;
            }
        }


        [SerializeField, Tooltip ("Smooth position method")]
        SmoothPositionMethod m_SmoothPositionMethod = SmoothPositionMethod.Lerp;

        /// <summary>
        /// Gets or sets the smooth position method.
        /// </summary>
        /// <value>The smooth position method.</value>
        public SmoothPositionMethod smoothPositionMethod
        {
            get
            {
                return m_SmoothPositionMethod;
            }
            set
            {
                m_SmoothPositionMethod = value;
            }
        }

        /// <summary>
        /// The position lerp rate.
        /// </summary>
        [SerializeField, Range(0.01f, 1), Tooltip("Positional lerp rate")]
        float m_PositionSmoothLerp = 0.1f;

        /// <summary>
        /// Gets or sets the positional lerp rate.
        /// When smooth position method == Lerp, this value is the lerp rate between prev-this frame position.
        /// </summary>
        /// <value>The position lerp.</value>
        public float PositionSmoothLerp
        {
            get
            {
                return m_PositionSmoothLerp;
            }
            set
            {
                m_PositionSmoothLerp = value;
            }
        }

        /// <summary>
        /// The positional smoothing velocity.
        /// </summary>
        [SerializeField, Range(0.01f, 10), Tooltip("Positional smooth velocity")]
        float m_PositionalSmoothVelocity = 0.1f;

        /// <summary>
        /// Gets or sets the position smooth velocity.
        /// </summary>
        /// <value>The position smooth velocity.</value>
        public float PositionSmoothVelocity 
        {
            get 
            {
                return m_PositionalSmoothVelocity;
            }
            set 
            {
                m_PositionalSmoothVelocity = value;
            }
        }

        [SerializeField, Tooltip ("Smooth position: lerp by distance")]
        AnimationCurve m_SmoothPosition_LerpByDistance;

        /// <summary>
        /// Gets or sets the smooth position curve control : lerp by distance
        /// </summary>
        /// <value>The smooth position curve control.</value>
        public AnimationCurve SmoothPositionCurveControl_LerpByDistance
        {
            get
            {
                return m_SmoothPosition_LerpByDistance;
            }
            set
            {
                m_SmoothPosition_LerpByDistance = value;
            }
        }

        [SerializeField, Tooltip ("Smooth position curve control : move in velocity by distance")]
        AnimationCurve m_SmoothPosition_VelocityByDistance = AnimationCurve.Linear(0,0,1,1);

        /// <summary>
        /// Gets or sets the smooth position curve control : move in velocity by distance
        /// </summary>
        /// <value>The smooth position curve control.</value>
        public AnimationCurve SmoothPositionCurveControl_VelocityByDistance
        {
            get
            {
                return m_SmoothPosition_VelocityByDistance;
            }
            set
            {
                m_SmoothPosition_VelocityByDistance = value;
            }
        }


        [Header(" - Rotation - ")]
        [SerializeField]
        bool m_ApplyAlgorithmRotation = true;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Ximmerse.SlideInSDK.MarkerTargetBehaviour"/> apply
        /// marker algorithm rotation.
        /// </summary>
        /// <value><c>true</c> if update marker rotation; otherwise, <c>false</c>.</value>
        public bool ApplyAlgorithmRotation
        {
            get
            {
                return m_ApplyAlgorithmRotation;
            }
            set
            {
                m_ApplyAlgorithmRotation = value;
            }
        }

        [SerializeField, Tooltip ("Smooth marker rotation.")]
        bool m_SmoothMarkerRotation = false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Ximmerse.SlideInSDK.MarkerTargetBehaviour"/> smooth
        /// marker rotation.
        /// </summary>
        /// <value><c>true</c> if smooth marker rotation; otherwise, <c>false</c>.</value>
        public bool SmoothMarkerRotation
        {
            get
            {
                return m_SmoothMarkerRotation;
            }
            set
            {
                m_SmoothMarkerRotation = value;
            }
        }


        [SerializeField]
        SmoothRotationMethod m_SmoothRotationMethod = SmoothRotationMethod.Angular;

        public SmoothRotationMethod smoothRotationMethod
        {
            get
            {
                return m_SmoothRotationMethod;
            }
            set
            {
                m_SmoothRotationMethod = value;
            }
        }

        [SerializeField, Range(0, 1)]
        float m_SmoothRotationLerpRate;

        /// <summary>
        /// Gets or sets the smooth lerp rate.
        /// </summary>
        /// <value>The smooth lerp rate.</value>
        public float SmoothRotationLerpRate
        {
            get
            {
                return m_SmoothRotationLerpRate;
            }
            set
            {
                m_SmoothRotationLerpRate = value;
            }
        }

        [SerializeField]
        float m_SmoothRotationAnglarSpeed = 0;

        /// <summary>
        /// Gets or sets the smooth rotation anglar speed.
        /// </summary>
        /// <value>The smooth rotation angle.</value>
        public float SmoothRotationAnglarSpeed
        {
            get
            {
                return m_SmoothRotationAnglarSpeed;
            }
            set
            {
                m_SmoothRotationAnglarSpeed = value;
            }
        }

        [SerializeField, Tooltip ("Smooth position: lerp by distance")]
        AnimationCurve m_SmoothRotation_LerpByDistance;

        /// <summary>
        /// Gets or sets the smooth rotation curve control : lerp by distance
        /// </summary>
        /// <value>The smooth position curve control.</value>
        public AnimationCurve SmoothRotationCurveControl_LerpByDistance
        {
            get
            {
                return m_SmoothRotation_LerpByDistance;
            }
            set
            {
                m_SmoothRotation_LerpByDistance = value;
            }
        }

        [SerializeField, Tooltip ("Smooth rotation curve control : rotate in angular speed by distance")]
        AnimationCurve m_SmoothRotation_AngularByDistance = AnimationCurve.Linear(0,0,1,1);

        /// <summary>
        /// Gets or sets the smooth rotation curve control : move in velocity by distance
        /// </summary>
        /// <value>The smooth position curve control.</value>
        public AnimationCurve SmoothRotationCurveControl_AngularByDistance
        {
            get
            {
                return m_SmoothRotation_AngularByDistance;
            }
            set
            {
                m_SmoothRotation_AngularByDistance = value;
            }
        }

        /// <summary>
        /// Position prediction
        /// </summary>
        [SerializeField]
        [Header(" - Prediction : Position - ")]
        bool m_PositionPrediction;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Ximmerse.SlideInSDK.MarkerTargetBehaviour"/>
        /// position prediction.
        /// </summary>
        /// <value><c>true</c> if position prediction; otherwise, <c>false</c>.</value>
        public bool PositionPrediction
        {
            get
            {
                return m_PositionPrediction;
            }
            set
            {
                m_PositionPrediction = value;
            }
        }

        [SerializeField]
        bool m_SmoothPositionPrediction;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Ximmerse.SlideInSDK.MarkerTargetBehaviour"/> smooth
        /// position prediction.
        /// </summary>
        /// <value><c>true</c> if smooth position prediction; otherwise, <c>false</c>.</value>
        public bool SmoothPositionPrediction
        {
            get
            {
                return m_SmoothPositionPrediction;
            }
            set
            {
                m_SmoothPositionPrediction = value;
                this.XYZPrediction.Smooth = value;
            }
        }

        /// <summary>
        /// The speed used when smoothing, lower the faster.
        /// </summary>
        [SerializeField, Tooltip("The speed used when smoothing, lower the faster."), Range(0, 1)]
        float m_SmoothPositionPredicitonSpeed;

        /// <summary>
        /// Gets or sets the smooth position prediciton speed.
        /// </summary>
        /// <value>The smooth position prediciton speed.</value>
        public float SmoothPositionPredicitonSpeed
        {
            get
            {
                return m_SmoothPositionPredicitonSpeed;
            }
            set
            {
                m_SmoothPositionPredicitonSpeed = value;
                this.XYZPrediction.SmoothSpeed = value;
            }
        }

        /// <summary>
        /// The sub marker mask, when zero means no sub-markers are tracked.
        /// Then GT zero, loops on each byte to acquire sub marker's existence.
        /// </summary>
        protected ulong m_SubMarkerMask;

        /// <summary>
        /// Gets the sub marker mask.
        /// </summary>
        /// <value>The sub marker mask.</value>
        public ulong SubMarkerMask
        {
            get
            {
                return m_SubMarkerMask;
            }
        }


        /// <summary>
        /// Rotation prediction
        /// </summary>
        [SerializeField, Tooltip("Rotation predicition turn on or off.")]
        [Header(" - Prediction : Rotation - ")]
        bool m_RotationPrediciton;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Ximmerse.SlideInSDK.MarkerTargetBehaviour"/>
        /// rotation prediciton.
        /// </summary>
        /// <value><c>true</c> if rotation prediciton; otherwise, <c>false</c>.</value>
        public bool RotationPrediciton
        {
            get
            {
                return m_RotationPrediciton;
            }
            set
            {
                m_RotationPrediciton = value;
            }
        }

        [SerializeField, Range(0, 30)]
        int m_RotationPredictionDelayFrame = 0;

        /// <summary>
        /// Gets or sets the rotation prediction delay frame.
        /// </summary>
        /// <value>The rotation prediction delay frame.</value>
        public int RotationPredictionDelayFrame
        {
            get
            {
                return m_RotationPredictionDelayFrame;
            }
            set
            {
                
                m_RotationPredictionDelayFrame = value;
            }
        }

//        [Space, Header(" - Offset Tilt & Shift -")]
//        [SerializeField, Tooltip("Pre shift position is the XYZ offset apply to final marker world position")]
//        Vector3 m_PreShiftPosition;
//
//        /// <summary>
//        /// Gets or sets the pre shift position.
//        /// </summary>
//        /// <value>The pre shift position.</value>
//        public Vector3 PreShiftPosition
//        {
//            get
//            {
//                return m_PreShiftPosition;
//            }
//            set
//            {
//                m_PreShiftPosition = value;
//            }
//        }
//
//        [SerializeField,Tooltip("Pre tilt euler is the Euler offset apply to final marker world position")]
//        Vector3 m_PreTiltEuler;
//
//        /// <summary>
//        /// Gets or sets the pre tilt euler.
//        /// </summary>
//        /// <value>The pre tilt euler.</value>
//        public Vector3 PreTiltEuler
//        {
//            get
//            {
//                return m_PreTiltEuler;
//            }
//            set
//            {
//                m_PreTiltEuler = value;
//                if (m_PreTiltEuler != Vector3.zero)
//                {
//                    m_PreTileQ = Quaternion.Euler(m_PreTiltEuler);
//                } else
//                {
//                    m_PreTileQ = Quaternion.identity;
//                }
//            }
//        }
//
//        [SerializeField, Tooltip("XYZ applied after VPU rotation. Note: post shift position is applied only when visual oritentation is tracked and applied.")]
//        Vector3 m_PostShiftPosition;
//
//        /// <summary>
//        /// Gets or sets the post shift position.
//        /// Note: post shift position is applied only when visual oritentation is tracked and applied.
//        /// </summary>
//        /// <value>The post shift position.</value>
//        public Vector3 PostShiftPosition
//        {
//            get
//            {
//                return m_PostShiftPosition;
//            }
//            set
//            {
//                m_PostShiftPosition = value;
//            }
//        }
//
//
//        protected Quaternion m_PreTileQ;

        #endregion

        PositionPrediction m_XYZPrediction;

        /// <summary>
        /// Gets the default XYZ prediction.
        /// </summary>
        /// <value>The XYZ prediction.</value>
        internal PositionPrediction XYZPrediction
        {
            get
            {
                if (m_XYZPrediction == null)
                {
                    m_XYZPrediction = new Ximmerse.SlideInSDK.PositionPrediction();
                }
                return m_XYZPrediction;
            }
        }

        RotationPrediction m_PosePrediction;

        /// <summary>
        /// Gets the default pose prediction.
        /// </summary>
        /// <value>The pose prediction.</value>
        internal RotationPrediction PosePrediction
        {
            get
            {
                if (m_PosePrediction == null)
                {
                    m_PosePrediction = new Ximmerse.SlideInSDK.RotationPrediction();
                }
                return m_PosePrediction;
            }
        }

        protected virtual void Start()
        {
            this.XYZPrediction.Smooth = this.SmoothPositionPrediction;
            this.XYZPrediction.SmoothSpeed = this.SmoothPositionPredicitonSpeed;
        }
    }
}