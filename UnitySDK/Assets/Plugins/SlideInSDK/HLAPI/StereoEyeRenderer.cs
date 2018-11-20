using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using PolyEngine;

namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// This script denotes to single eye camera rendering, functional with a camera.
    /// </summary>
    [RequireComponent (typeof(Camera))]
    public class StereoEyeRenderer : MonoBehaviour 
    {
        /// <summary>
        /// Gets or sets the undistortion mesh scale.
        /// </summary>
        /// <value>The undistortion mesh scale.</value>
        public Vector3 UndistortionMeshScale 
        {
            get 
            {
                if(m_UndistortionMeshInstance != null)
                {
                    return this.m_UndistortionMeshInstance.transform.localScale;
                }
                else 
                {
                    return Vector3.one;
                }
            }
            set 
            {
                if(m_UndistortionMeshInstance)
                {
                    this.m_UndistortionMeshInstance.transform.localScale = value;
                }
            }
        }

        Material m_UndistortionMaterial;

        RenderTexture m_TargetTexture;

        CommandBuffer m_CmdBuffer_SingleEye = null;

        GameObject undistortionRoot;

        GameObject m_UndistortionMeshInstance;

        static Material sMaterial_black = null;

        static Material Material_black
        {
            get
            {
                if(sMaterial_black == null)
                {
                    Shader shader_unlit_color = Shader.Find("Unlit/Color");
                    sMaterial_black = new Material(shader_unlit_color);
                    sMaterial_black.color = Color.black;
                }
                return sMaterial_black;
            }
        }

        Camera m_Camera;

        internal Camera eye
        {
            get
            {
                if(!m_Camera)
                    m_Camera = GetComponent<Camera>();
                return m_Camera;
            }
        }

        internal bool isLeftEye
        {
            get 
            {
                return this.eye.stereoTargetEye == StereoTargetEyeMask.Left;
            }
        }

        internal bool isRightEye
        {
            get 
            {
                return this.eye.stereoTargetEye == StereoTargetEyeMask.Right;
            }
        }

        void Awake ()
        {
            createUndistortionMeshInstance();
            createCommandBufferForSingleEye();
        }


        private void createUndistortionMeshInstance ()
        {
            Camera eye = this.GetComponent<Camera>();
            //Generate undistortion instance:
            if (undistortionRoot != null)
            {
                DestroyImmediate(undistortionRoot);
            }

            //Prepare undistortion view shader:
            Shader undistortionShader = Resources.Load<Shader>("XimmerseAR");
            this.m_UndistortionMaterial = new Material(undistortionShader);
            m_UndistortionMaterial.color = Color.white;

            undistortionRoot = new GameObject("UndistortionView-" + eye.stereoTargetEye.ToString());

            ReflectionLenProfile lenProfile = ReflectionLenProfileContainer.GetLenProfile();
            Vector3 offset = lenProfile.UndistortionMeshOffset;
            offset += eye.stereoTargetEye == StereoTargetEyeMask.Left ? lenProfile.LeftUndistortionMeshOffset : lenProfile.RightUndistortionMeshOffset;
            if (eye.stereoTargetEye == StereoTargetEyeMask.Left)
                offset.x *= -1;

            undistortionRoot.transform.position = offset + Vector3.forward * 1;

            m_UndistortionMeshInstance = new GameObject("UndistortionMesh", new System.Type[] { typeof(MeshFilter), typeof(MeshRenderer) });
            Mesh undistortionMesh = lenProfile.UndistortionMesh;

            if(eye.stereoTargetEye == StereoTargetEyeMask.Left)
            {
                m_UndistortionMaterial.SetFloat ("_ViewCutFromRight", 1);
            }
            else if(eye.stereoTargetEye == StereoTargetEyeMask.Right)
            {
                m_UndistortionMaterial.SetFloat ("_ViewCutFromRight", 2);
            }
            m_UndistortionMeshInstance.GetComponent<MeshFilter>().sharedMesh = undistortionMesh;
            m_UndistortionMeshInstance.GetComponent<MeshRenderer>().sharedMaterial = m_UndistortionMaterial;
            m_UndistortionMeshInstance.transform.localScale = lenProfile.UndistortionMeshScale;
            m_UndistortionMeshInstance.transform.SetParent(undistortionRoot.transform, false);
            m_UndistortionMeshInstance.transform.localEulerAngles = lenProfile.UndistortionMeshViewDirection;


            GameObject undistrotionMeshBg = GameObject.CreatePrimitive(PrimitiveType.Quad);
            undistrotionMeshBg.transform.SetParent (undistortionRoot.transform, false);
            undistrotionMeshBg.transform.localPosition = new Vector3(0, 0,3);
            undistrotionMeshBg.transform.localScale = Vector3.one * 10;//big enough to cover full view
            undistrotionMeshBg.GetComponent<MeshRenderer>().sharedMaterial = Material_black;
            undistrotionMeshBg.name = "Black-Blackground";

            this.undistortionRoot.SetActive(false);
            m_TargetTexture = new RenderTexture(eye.pixelWidth, eye.pixelHeight, 24, RenderTextureFormat.ARGB32);
            m_UndistortionMaterial.mainTexture = m_TargetTexture;

            //undistortionViewMat.mainTexture = Resources.Load<Texture2D>("uvcheck");
        }

        /// <summary>
        /// Creates the command buffer for single eye (left or right)
        /// </summary>
        private void createCommandBufferForSingleEye()
        {
            Camera eye = this.GetComponent<Camera>();

            //Setup orthographic view command buffer:
            float pScreenW, pScreenH;//physical screen width, height
            if(!PEUtils.GetPhysicalScreenSize(out pScreenW, out pScreenH))
            {
                throw new UnityException("Fail to get screen physical size");
            }
            //in case screen ori not correct
            if(pScreenH > pScreenW)
            {
                var t = pScreenH;
                pScreenH = pScreenW;
                pScreenW = t;
            }
//            Debug.LogFormat("Physical w/h: {0}/{1}", pScreenW, pScreenH);
            pScreenW /= 2;

            eye.orthographic = true;
            var phoneProfile = PhoneProfileContainer.GetCurrent ();
            eye.orthographicSize = phoneProfile.OrthoRectSize;
            Matrix4x4 matrix_ortho_prj = eye.projectionMatrix;

            Matrix4x4 matrix_World2Cam = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(1, 1, -1));

            if(m_CmdBuffer_SingleEye != null)
            {
                eye.RemoveCommandBuffer (CameraEvent.AfterImageEffects, m_CmdBuffer_SingleEye);
            }
            m_CmdBuffer_SingleEye = new CommandBuffer();
            m_CmdBuffer_SingleEye.SetViewProjectionMatrices(matrix_World2Cam, matrix_ortho_prj);
            m_CmdBuffer_SingleEye.ClearRenderTarget(clearDepth: true, clearColor: false, backgroundColor: Color.black);
            var undistortionRender = undistortionRoot.transform.GetChild(0).GetComponent<MeshRenderer>();
            var undistortionBGRender = undistortionRoot.transform.GetChild(1).GetComponent<MeshRenderer>();
            m_CmdBuffer_SingleEye.DrawRenderer(undistortionBGRender, undistortionBGRender.sharedMaterial);//order does matter
            m_CmdBuffer_SingleEye.DrawRenderer(undistortionRender, undistortionRender.sharedMaterial);
            eye.AddCommandBuffer(CameraEvent.AfterImageEffects, m_CmdBuffer_SingleEye);


            //Setup rendering camera properties:
            ReflectionLenProfile lenProfile = ReflectionLenProfileContainer.GetLenProfile();
            var hFov = lenProfile.hFov; 
            var vFov = lenProfile.vFov;
            eye.SetCameraFov(hFov, vFov);

            #region Alignment

            var projectionMatrix = eye.projectionMatrix;
            projectionMatrix[1, 1] = 1.23646257F;
            projectionMatrix[1, 2] = -0.0777916671F;
            eye.projectionMatrix = projectionMatrix;

            #endregion
        }

        void OnRenderImage(RenderTexture src,RenderTexture dest)
        {
            Graphics.Blit(src, m_TargetTexture);
        }

        /// <summary>
        /// Updates the undistortion pan.
        /// </summary>
        /// <param name="pan">Pan.</param>
        internal void UpdateUndistortionPan (Vector3 pan)
        {
            this.undistortionRoot.transform.localPosition = pan;
        }

        /// <summary>
        /// Updates the undistortion mesh euler.
        /// </summary>
        /// <param name="euler">Euler.</param>
        internal void UpdateUndistortionMeshEuler (Vector3 euler)
        {
            this.m_UndistortionMeshInstance.transform.localEulerAngles = euler;
        }

        /// <summary>
        /// Uses the new undistortion mesh to replace the existing.
        /// </summary>
        /// <param name="mesh">Mesh.</param>
        internal void UseNewUndistortionMesh (Mesh mesh)
        {
            m_UndistortionMeshInstance.GetComponent<MeshFilter>().mesh = mesh;
        }

        internal void ResetFov ()
        {
            var cam = GetComponent<Camera>();
            var lenProfile = ReflectionLenProfileContainer.GetLenProfile();
            cam.SetCameraFov (lenProfile.hFov, lenProfile.vFov);

            #region Alignment

            var projectionMatrix = eye.projectionMatrix;
            projectionMatrix[1, 1] = 1.23646257F;
            projectionMatrix[1, 2] = -0.0777916671F;
            eye.projectionMatrix = projectionMatrix;

            #endregion
        }
    }

}