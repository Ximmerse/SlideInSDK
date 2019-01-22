using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ximmerse.SlideInSDK
{

    [CreateAssetMenu(menuName = "Ximmerse/SlideInSDK/Phone Profile", fileName = "PhoneProfile")]
    public class PhoneProfileContainer : ScriptableObject
    {
        [Header("Phone profiles")]

        [SerializeField]
        List<PhoneProfile> phones = new List<PhoneProfile>();

        static PhoneProfileContainer m_singleton;
        static PhoneProfileContainer singleton 
        {
            get 
            {
                if(!m_singleton)
                {
                    m_singleton = Resources.Load<PhoneProfileContainer>("PhoneProfiles");
                    if (m_singleton == null)
                    {
                        throw new UnityException("Fatal : default phone profile does not present in Resource folder.");
                    }
                }
                return m_singleton;
            }
        }

        /// <summary>
        /// Gets the profile for current phone.
        /// </summary>
        /// <returns>The current.</returns>
        public static PhoneProfile GetCurrent ()
        {
            return singleton.phones[0];
        }
       
    }
}