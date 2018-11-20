using UnityEngine;
using System.Collections.Generic;

namespace Ximmerse.SlideInSDK
{
    [CreateAssetMenu(menuName = "Ximmerse/SlideInSDK/Reflection Len Profile", fileName = "LenProfiles")]
    public class ReflectionLenProfileContainer : ScriptableObject
    {
        [Header("Len profiles")]

        [SerializeField]
        ReflectionLenProfile m_LenProfile = new ReflectionLenProfile ();

        static ReflectionLenProfileContainer m_singleton;
        static ReflectionLenProfileContainer singleton
        {
            get
            {
                if (!m_singleton)
                {
                    m_singleton = Resources.Load<ReflectionLenProfileContainer>("LenProfiles");
                    if (m_singleton == null)
                    {
                        throw new UnityException("Fatal : len profile does not present in Resource folder.");
                    }
                }
                return m_singleton;
            }
        }

        public float hFov 
        {
            get
            {
                return m_LenProfile.hFov;
            }
            set 
            {
                m_LenProfile.hFov = value;
            }
        }

        public float vFov 
        {
            get
            {
                return m_LenProfile.vFov;
            }
            set 
            {
                m_LenProfile.vFov = value;
            }
        }

        /// <summary>
        /// Gets or sets the eye separation.
        /// </summary>
        /// <value>The eye separation.</value>
        public float eyeSeparation 
        {
            get
            {
                return m_LenProfile.EyeSeparation;
            }
            set 
            {
                m_LenProfile.EyeSeparation = value;
            }
        }

        /// <summary>
        /// Gets the profile for current len. Automatically fallback to default phone setting if no current phone profile is matched.
        /// </summary>
        /// <returns>The current.</returns>
        public static ReflectionLenProfile GetLenProfile()
        {
            return singleton.m_LenProfile;
        }

    }
}