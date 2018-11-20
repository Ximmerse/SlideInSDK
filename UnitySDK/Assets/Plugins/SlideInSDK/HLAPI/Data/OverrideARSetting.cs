using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyEngine;


namespace Ximmerse.SlideInSDK
{

    /// <summary>
    /// Override AR setting.
    /// If the corresponding field is unchecked, means this field is not overrided then the defualt setting value is not changed.
    /// </summary>
    [CreateAssetMenu(menuName = "Ximmerse/SlideInSDK/Overrided AR setting", fileName = "Overrided AR Setting")]
    public class OverrideARSetting : ScriptableObject
    {
        [SerializeField, Multiline(3)]
        string m_Description = string.Empty;

        public string Description
        {
            get
            {
                return m_Description;
            }
        }

        [Header (" --- Override : Len Profile ---"), Tooltip ("Below fields overrides the len profile")]

        [SerializeField]
        NullableSingle m_hFov = 40;

        [SerializeField]
        NullableSingle m_vFov = 40;
//        float m_vFov = 40f;//h=42,v=40 for layni; h=60,v=40 for Ned+

        [SerializeField]
        NullableSingle m_EyeSeparation = 0.032f;

        /// <summary>
        /// Gets or sets the eye separation distance.
        /// </summary>
        /// <value>The eye separation.</value>
        public float? EyeSeparation
        {
            get
            {
                return m_EyeSeparation.Value;
            }
            set
            {
                m_EyeSeparation = value;
            }
        }

        /// <summary>
        /// Gets or sets the hortizontal fov
        /// </summary>
        /// <value>The HF ov.</value>
        public float? hFov
        {
            get
            {
                return m_hFov.Value;
            }

            set
            {
                m_hFov = value;
            }
        }
        /// <summary>
        /// Gets or sets the vertical fov.
        /// </summary>
        /// <value>The VF ov.</value>
        public float? vFov
        {
            get
            {
                return m_vFov.Value;
            }

            set
            {
                m_vFov = value;
            }
        }

        [SerializeField]
        NullableVector3 m_UndistortionMeshViewDirection = new Vector3(0, 180, 0);

        /// <summary>
        /// Gets or sets the undistortion mesh view direction.
        /// </summary>
        /// <value>The undistortion mesh view direction.</value>
        public Vector3? UndistortionMeshViewDirection
        {
            get
            {
                return m_UndistortionMeshViewDirection.Value;
            }
            set
            {
                m_UndistortionMeshViewDirection = value;
            }
        }

        [SerializeField]
        NullableVector3 m_UndistortionMeshScale = Vector3.one;

        /// <summary>
        /// Gets or sets the undistortion mesh scale.
        /// </summary>
        /// <value>The undistortion mesh scale.</value>
        public Vector3? UndistortionMeshScale
        {
            get
            {
                return m_UndistortionMeshScale.Value;
            }
            set
            {
                m_UndistortionMeshScale = value;
            }
        }

        [SerializeField]
        NullableVector3 m_UndistortionMeshOffset = new Vector3 (0.031f,0,0);

        /// <summary>
        /// Gets or sets the undistortion mesh offset.
        /// </summary>
        /// <value>The undistortion mesh offset.</value>
        public Vector3? UndistortionMeshOffset
        {
            get
            {
                return m_UndistortionMeshOffset.Value;
            }
            set
            {
                m_UndistortionMeshOffset = value;
            }
        }

        [SerializeField]
        NullableVector3 m_LeftUndistortionMeshOffset = new Vector3(0, 0, 0);

        /// <summary>
        /// Gets or sets the left undistortion mesh offset.
        /// </summary>
        /// <value>The undistortion mesh offset.</value>
        public Vector3? LeftUndistortionMeshOffset
        {
            get
            {
                return m_LeftUndistortionMeshOffset.Value;
            }
            set
            {
                m_LeftUndistortionMeshOffset = value;
            }
        }

        [SerializeField]
        NullableVector3 m_RightUndistortionMeshOffset = new Vector3(0, 0, 0);

        /// <summary>
        /// Gets or sets the right undistortion mesh offset.
        /// </summary>
        /// <value>The undistortion mesh offset.</value>
        public Vector3? RightUndistortionMeshOffset
        {
            get
            {
                return m_RightUndistortionMeshOffset.Value;
            }
            set
            {
                m_RightUndistortionMeshOffset = value;
            }
        }

        [SerializeField]
        Mesh m_UndistortionMesh = null;

        /// <summary>
        /// Gets  the undistortion mesh.
        /// </summary>
        /// <value>The undistortion mesh.</value>
        public Mesh UndistortionMesh
        {
            get
            {
                return m_UndistortionMesh;
            }
            set 
            {
                m_UndistortionMesh = value;
            }
        }

        [Header (" --- Override : Tracking Config ---"), Tooltip ("Below fields overrides the tracking config")]
        [SerializeField]
        NullableVector3 m_Positional_Offset = Vector3.zero;
        /// <summary>
        /// Gets or sets the tracking anchor positional offset.
        /// </summary>
        /// <value>The tracking anchor positional offset.</value>
        public Vector3? TrackingAnchor_Positional_Offset
        {
            get
            {
                return m_Positional_Offset.Value;
            }
            set
            {
                m_Positional_Offset.Value = value;
            }
        }


        /// <summary>
        /// The euler offset.
        /// </summary>
        [SerializeField]
        NullableVector3 m_Euler_Offset = Vector3.zero;
        /// <summary>
        /// Gets or sets the tracking anchor euler offset.
        /// </summary>
        /// <value>The tracking anchor euler offset.</value>
        public Vector3? TrackingAnchor_Euler_Offset
        {
            get
            {
                return m_Euler_Offset.Value;
            }
            set
            {
                m_Euler_Offset.Value = value;
            }
        }


        /// <summary>
        /// The scalar offset.
        /// </summary>
        [SerializeField]
        NullableVector3 m_Scale = new Vector3(1,1,1);

        /// <summary>
        /// Gets or sets the scale
        /// </summary>
        /// <value>The scalar offset.</value>
        public Vector3? TrackingAnchor_Scale
        {
            get
            {
                return m_Scale.Value;
            }
            set
            {
                m_Scale.Value = value;
            }
        }


        [Header(" -- Override: Tracking Data Process -- ")]
        [SerializeField , Tooltip("The index to retrieve raw position.Sign is used to multiple the raw value.")]
        NullableVector3Int m_RawPositionIndex = new NullableVector3Int();

        /// <summary>
        /// The index to retrieve raw position.Sign is used to multiple the raw value.
        /// </summary>
        /// <value>The index of the raw position.</value>
        public Vector3Int? RawPositionIndex
        {
            get
            {
                return m_RawPositionIndex.Value;
            }
            set
            {
                m_RawPositionIndex.Value = value;
            }
        }



        [SerializeField, Tooltip ("Multiplier to raw tracked position")]
        NullableVector3 m_RawPositionFieldMultiplier = new NullableVector3();

        /// <summary>
        /// Gets or sets the raw position field multiplier.
        /// </summary>
        /// <value>The raw rotation field multiplier.</value>
        public Vector3? RawPositionFieldMultiplier
        {
            get
            {
                return m_RawPositionFieldMultiplier.Value;
            }
            set
            {
                m_RawPositionFieldMultiplier.Value = value;
            }
        }

        [SerializeField, Tooltip ("The index to retrieve raw oritentation.")]
        NullableVector4Int m_RawRotationIndex = new NullableVector4Int();

        /// <summary>
        /// The index to retrieve raw oritentation.
        /// </summary>
        /// <value>The index of the raw rotation.</value>
        public pVector4Int? RawRotationIndex
        {
            get
            {
                return m_RawRotationIndex.Value;
            }
            set
            {
                m_RawRotationIndex.Value = value;
            }
        }

        [SerializeField, Tooltip ("Multiplier to raw tracked rotation")]
        NullableVector4 m_RawRotationFieldMultiplier = new NullableVector4 ();

        /// <summary>
        /// Gets or sets the raw rotation field multiplier.
        /// </summary>
        /// <value>The raw rotation field multiplier.</value>
        public Vector4? RawRotationFieldMultiplier
        {
            get
            {
                return m_RawRotationFieldMultiplier.Value;
            }
            set
            {
                m_RawRotationFieldMultiplier.Value = value;
            }
        }

        [SerializeField, Tooltip ("Pre-Tilt after retrieved marker raw pose")]
        NullableVector3 m_MarkerPosePreTilt = new NullableVector3();

        /// <summary>
        /// Gets the marker pose pre tilt.
        /// </summary>
        /// <value>The marker pose pre tilt.</value>
        public Vector3? MarkerPosePreTilt
        {
            get
            {
                return m_MarkerPosePreTilt.Value;
            }
            set 
            {
                m_MarkerPosePreTilt.Value = value;
            }
        }


        [SerializeField, Tooltip ("Post-Tilt after retrieved marker raw pose")]
        NullableVector3 m_MarkerPosePostTilt = new NullableVector3();

        /// <summary>
        /// Gets the marker pose post tilt.
        /// </summary>
        /// <value>The marker pose pre tilt.</value>
        public Vector3? MarkerPosePostTilt
        {
            get
            {
                return m_MarkerPosePostTilt.Value;
            }
            set 
            {
                m_MarkerPosePostTilt.Value = value;
            }
        }
    }
}