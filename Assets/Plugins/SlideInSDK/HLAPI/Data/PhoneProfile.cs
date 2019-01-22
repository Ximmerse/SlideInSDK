using UnityEngine;
namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Phone profile.
    /// Config the setting relative to phone.
    /// </summary>
    [System.Serializable]
    public class PhoneProfile
    {
        /// <summary>
        /// The name of the profile.
        /// </summary>
        [SerializeField]
        string m_ProfileName;

        public string ProfileName
        {
            get
            {
                return m_ProfileName;
            }

            set
            {
                m_ProfileName = value;
            }
        }
        /// <summary>
        /// The description of the profile.
        /// </summary>
        [SerializeField, Multiline(2)]
        string m_Description;

        public string Description
        {
            get
            {
                return m_Description;
            }

            set
            {
                m_Description = value;
            }
        }
        /// <summary>
        /// The view port rect for left eye;
        /// By default: left pan = righthand side of the phone screen, this is due to the structure of slide in device.
        /// </summary>
        [SerializeField]
        Rect m_LeftViewRect = new Rect(0.5f, 0, 0.5f, 1);

        /// <summary>
        /// The view port rect for left eye;
        /// </summary>
        public Rect LeftViewRect
        {
            get
            {
                return m_LeftViewRect;
            }
        }

        /// <summary>
        /// The view port rect for right eye;
        /// </summary>
        [SerializeField]
        Rect m_RightViewRect = new Rect(0,0,0.5f,1);


        /// <summary>
        /// The view port rect for right eye;
        /// </summary>
        public Rect RightViewRect
        {
            get
            {
                return m_RightViewRect;
            }
        }

        [SerializeField]
        float m_OrthoSize = 0.68f;

        /// <summary>
        /// Gets or sets the size of the orthographic camera's rect size.
        /// </summary>
        /// <value>The size of the ortho rect.</value>
        public float OrthoRectSize 
        {
            get 
            {
                return m_OrthoSize;
            }
            set 
            {
                m_OrthoSize = value;
            }
        }
    }
}