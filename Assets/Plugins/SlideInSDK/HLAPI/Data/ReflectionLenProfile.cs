using UnityEngine;
using PolyEngine;

namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Len profile.
    /// Config the setting relative to reflection len.
    /// </summary>
    [System.Serializable]
    public class ReflectionLenProfile
    {
        [SerializeField]
        string m_ProfileName;

        [SerializeField, Multiline(2)]
        string m_Description;

        [SerializeField]
        float m_hFov = 40f, m_vFov = 40f;//h=42,v=40 for layni; h=60,v=40 for Ned+

        [SerializeField]
        float m_EyeSeparation = 0.032f;

        /// <summary>
        /// Eye separation setting in StereoSinglePass rendering mode.
        /// </summary>
        [SerializeField, Tooltip ("Eye separation setting in StereoSinglePass rendering mode.")]
        float m_MonoEyeSeparation = 0.515f;

        /// <summary>
        /// Gets or sets the eye separation distance.
        /// </summary>
        /// <value>The eye separation.</value>
        public float EyeSeparation
        {
            get
            {
                return m_EyeSeparation;
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
        public float hFov
        {
            get
            {
                return m_hFov;
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
        public float vFov
        {
            get
            {
                return m_vFov;
            }

            set
            {
                m_vFov = value;
            }
        }
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
        }

        /// <summary>
        /// Eye separation setting in StereoSinglePass rendering mode.
        /// </summary>
        /// <value>The mono eye separation.</value>
        public float MonoEyeSeparation
        {
            get
            {
                return m_MonoEyeSeparation;
            }
            set
            {
                m_MonoEyeSeparation = value;
            }
        }

        [SerializeField]
        Vector3 m_UndistortionMeshViewDirection = new Vector3(0, 180, 0);

        /// <summary>
        /// Gets or sets the undistortion mesh view direction.
        /// </summary>
        /// <value>The undistortion mesh view direction.</value>
        public Vector3 UndistortionMeshViewDirection
        {
            get
            {
                return m_UndistortionMeshViewDirection;
            }
            set
            {
                m_UndistortionMeshViewDirection = value;
            }
        }

        [SerializeField]
        Vector3 m_UndistortionMeshScale = Vector3.one;

        /// <summary>
        /// Gets or sets the undistortion mesh scale.
        /// </summary>
        /// <value>The undistortion mesh scale.</value>
        public Vector3 UndistortionMeshScale
        {
            get
            {
                return m_UndistortionMeshScale;
            }
            set
            {
                m_UndistortionMeshScale = value;
            }
        }

        [SerializeField] private Vector3 m_UndistortionMeshOffset = new Vector3(0, 0, 0);

        /// <summary>
        /// Gets or sets the undistortion mesh offset.
        /// </summary>
        /// <value>The undistortion mesh offset.</value>
        public Vector3 UndistortionMeshOffset
        {
            get
            {
                return m_UndistortionMeshOffset;
            }
            set
            {
                m_UndistortionMeshOffset = value;
            }
        }

        [SerializeField] private Vector3 m_LeftUndistortionMeshOffset = new Vector3(0, 0, 0);

        /// <summary>
        /// Gets or sets the left undistortion mesh offset.
        /// </summary>
        /// <value>The undistortion mesh offset.</value>
        public Vector3 LeftUndistortionMeshOffset
        {
            get
            {
                return m_LeftUndistortionMeshOffset;
            }
            set
            {
                m_LeftUndistortionMeshOffset = value;
            }
        }

        [SerializeField] private Vector3 m_RightUndistortionMeshOffset = new Vector3(0, 0, 0);

        /// <summary>
        /// Gets or sets the right undistortion mesh offset.
        /// </summary>
        /// <value>The undistortion mesh offset.</value>
        public Vector3 RightUndistortionMeshOffset
        {
            get
            {
                return m_RightUndistortionMeshOffset;
            }
            set
            {
                m_RightUndistortionMeshOffset = value;
            }
        }



        [SerializeField]
        Mesh m_UndistortionMesh = null;

//        [SerializeField]
//        Vector3 m_Euler_Viewer_Offset1 = new Vector3(0, -180, 0);
//
//        /// <summary>
//        /// Gets or sets the euler viewer offset1.
//        /// </summary>
//        /// <value>The euler viewer offset1.</value>
//        public Vector3 Euler_Viewer_Offset1
//        {
//            get
//            {
//                return m_Euler_Viewer_Offset1;
//            }
//            set
//            {
//                m_Euler_Viewer_Offset1 = value;
//            }
//        }
//
//        [SerializeField]
//        Vector3 m_Euler_Viewer_Offset2 = new Vector3(-30, -180, 0);
//
//        /// <summary>
//        /// Gets or sets the euler viewer offset2.
//        /// </summary>
//        /// <value>The euler viewer offset2.</value>
//        public Vector3 Euler_Viewer_Offset2
//        {
//            get
//            {
//                return m_Euler_Viewer_Offset2;
//            }
//            set
//            {
//                m_Euler_Viewer_Offset2 = value;
//            }
//        }
    }
}