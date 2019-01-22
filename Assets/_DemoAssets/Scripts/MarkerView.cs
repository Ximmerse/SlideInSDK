using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Ximmerse.SlideInSDK;
using System.Linq;

[RequireComponent (typeof(MarkerIdentity))]
public class MarkerView : MonoBehaviour {

    [System.NonSerialized]
    public MarkerIdentity Marker;

    public TextMesh textMesh;

    public UnityEngine.UI.Text uiText;

    public UnityEngine.TextMesh uiText_SubMarkers;

    public bool PrintSubMarkerExistence = false;

    public GameObject visiblityRoot;

	// Use this for initialization
	void Start () 
    {
        Marker = GetComponent<MarkerIdentity>();
        if(textMesh)
        {
           textMesh.text = Marker.MarkerID.ToString();
        }

        if(uiText)
        {
           uiText.text = Marker.MarkerID.ToString();
        }

        //Set GameObject Activation state by marker visibility event.
        Marker.VisibilityChanged += Marker_VisibilityChangedged;
	}

    void OnDestroy ()
    {
        if(Marker)
           Marker.VisibilityChanged -= Marker_VisibilityChangedged;
    }

    void Marker_VisibilityChangedged (bool Visible)
    {
        if (textMesh)
            textMesh.gameObject.SetActive(Visible);
        if (uiText)
            uiText.gameObject.SetActive(Visible);
        if (visiblityRoot)
            visiblityRoot.gameObject.SetActive(Visible);
    }

    void LateUpdate ()
    {
        if (PrintSubMarkerExistence)
        {
            if (Marker.IsVisible)
            {
                ulong subMarkerMask = Marker.GetComponent<MarkerTargetBehaviour>().SubMarkerMask;
                int a1 = (int)(subMarkerMask & uint.MaxValue);
                int a2 = (int)(subMarkerMask >> 32);
                System.Collections.BitArray bits = new BitArray(new int[] { a1, a2 });
                StringBuilder buffer = new StringBuilder();
                for (int i = 0; i < bits.Length; i++)
                {
                    if (bits.Get(i))
                    {
                        buffer.Append(string.Format("[{0}] ", i));
                    }
                }
                uiText_SubMarkers.text = buffer.ToString ();
            }
            else
            {
                uiText_SubMarkers.text = string.Empty;
            }
        }
    }
     
    [ContextMenu ("test m")]
    public void testm ()
    {
        StringBuilder buffer01 = new StringBuilder();
        byte[] submarkerMask = System.BitConverter.GetBytes((ulong)0x81);
        for (int j = 0; j < submarkerMask.Length; j++)
        {
            byte marker = submarkerMask[j];
            buffer01.Append(string.Format("[{0}] ", marker));
        }
        Debug.LogFormat(buffer01.ToString ());

        System.Collections.BitArray b = new BitArray(new int[] { 0x81 });
        bool[] bits = new bool[b.Count];
        b.CopyTo(bits, 0);
        byte[] bitValues = bits.Select(bit => (byte)(bit ? 1 : 0)).ToArray();
        StringBuilder buffer02 = new StringBuilder();
        for (int j = 0; j < bitValues.Length; j++)
        {
            byte marker = bitValues[j];
            buffer02.Append(string.Format("[{0}] ", marker));
        }
        Debug.LogFormat(buffer02.ToString ());
    }
}
