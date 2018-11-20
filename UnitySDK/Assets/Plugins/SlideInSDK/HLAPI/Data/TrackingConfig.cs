using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector4Int = PolyEngine.pVector4Int;

namespace Ximmerse.SlideInSDK
{
    [CreateAssetMenu(menuName = "Ximmerse/SlideInSDK/Track anchor config", fileName = "Tracking Config")]
    public class TrackingConfig : ScriptableObject
    {
        [Header(" -- Tracking Anchor -- ")]
        [SerializeField]
        Vector3 m_Positional_Offset;

        public static Vector3 TrackingAnchor_Positional_Offset
        {
            get
            {
                return Singleton.m_Positional_Offset;
            }
            set
            {
                Singleton.m_Positional_Offset = value;
            }
        }


        /// <summary>
        /// The euler offset.
        /// </summary>
        [SerializeField]
        Vector3 m_Euler_Offset;

        public static Vector3 TrackingAnchor_Euler_Offset
        {
            get
            {
                return Singleton.m_Euler_Offset;
            }
            set
            {
                Singleton.m_Euler_Offset = value;
            }
        }


        /// <summary>
        /// The scalar offset.
        /// </summary>
        [SerializeField]
        Vector3 m_Scale = new Vector3(1,1,1);

        /// <summary>
        /// Gets or sets the scale
        /// </summary>
        /// <value>The scalar offset.</value>
        public static Vector3 TrackingAnchor_Scale
        {
            get
            {
                return Singleton.m_Scale;
            }
            set
            {
                Singleton.m_Scale = value;
            }
        }

        [Header(" -- Marker Rotation Algorithm -- ")]

        /// <summary>
        /// use for pre tilt on the raw rotation of marker.
        /// </summary>
        [SerializeField]
        Vector3 m_MarkerPosePreTilt = new Vector3(0,0,90);

        /// <summary>
        /// Gets or sets the marker pose pre tilt.
        /// </summary>
        /// <value>The marker pose pre tilt.</value>
        public static Vector3 MarkerPosePreTilt
        {
            get
            {
                return Singleton.m_MarkerPosePreTilt;
            }
            set
            {
                Singleton.m_MarkerPosePreTilt = value;
            }
        }

        /// <summary>
        /// use for post tilt on the raw rotation of marker.
        /// </summary>
        [SerializeField]
        Vector3 m_MarkerPosePostTilt = new Vector3(-90,0,0);

        /// <summary>
        /// Gets or sets the marker pose post tilt.
        /// </summary>
        /// <value>The marker pose pre tilt.</value>
        public static Vector3 MarkerPosePostTilt
        {
            get
            {
                return Singleton.m_MarkerPosePostTilt;
            }
            set
            {
                Singleton.m_MarkerPosePostTilt = value;
            }
        }




        [Header(" -- Tracking Data Process -- ")]
        [SerializeField , Tooltip("The index to retrieve raw position.Sign is used to multiple the raw value.")]
        /// <summary>
        /// The index to retrieve the raw positional float[] array from VPU.
        /// [1,0,2] : for vertical VPU.
        /// [0,1,2] : for horizontal VPU.
        /// </summary>
        Vector3Int m_RawPositionIndex = new Vector3Int(1, 0, 2);

        /// <summary>
        /// The index to retrieve raw position.Sign is used to multiple the raw value.
        /// </summary>
        /// <value>The index of the raw position.</value>
        public static Vector3Int RawPositionIndex
        {
            get
            {
                return Singleton.m_RawPositionIndex;
            }
            set
            {
                Singleton.m_RawPositionIndex = value;
            }
        }



        [SerializeField, Tooltip ("Multiplier to raw tracked position")]
        Vector3 m_RawPositionFieldMultiplier = new Vector3(1,1,1);

        /// <summary>
        /// Gets or sets the raw position field multiplier.
        /// </summary>
        /// <value>The raw rotation field multiplier.</value>
        public static Vector3 RawPositionFieldMultiplier
        {
            get
            {
                return Singleton.m_RawPositionFieldMultiplier;
            }
            set
            {
                Singleton.m_RawPositionFieldMultiplier = value;
            }
        }

        [SerializeField, Tooltip ("The index to retrieve raw oritentation.")]
        Vector4Int m_RawRotationIndex = new Vector4Int(0, 1, 2, 3);

        /// <summary>
        /// The index to retrieve raw oritentation.
        /// </summary>
        /// <value>The index of the raw rotation.</value>
        public static Vector4Int RawRotationIndex
        {
            get
            {
                return Singleton.m_RawRotationIndex;
            }
            set
            {
                Singleton.m_RawRotationIndex = value;
            }
        }

        [SerializeField, Tooltip ("Multiplier to raw tracked rotation")]
        Vector4 m_RawRotationFieldMultiplier = new Vector4(1, 1, 1, 1);

        /// <summary>
        /// Gets or sets the raw rotation field multiplier.
        /// </summary>
        /// <value>The raw rotation field multiplier.</value>
        public static Vector4 RawRotationFieldMultiplier
        {
            get
            {
                return Singleton.m_RawRotationFieldMultiplier;
            }
            set
            {
                Singleton.m_RawRotationFieldMultiplier = value;
            }
        }

        public static Matrix4x4 TranslationMatrix { get; private set;}

        static TrackingConfig singleton;

        public static TrackingConfig Singleton
        {
            get
            {
                if(!singleton)
                {
                    singleton = Resources.Load<TrackingConfig>("Tracking Config");
                }
                return singleton;
            }
        }
    }
}