using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ximmerse.SlideInSDK;

[RequireComponent (typeof(MarkerIdentity))]
public class MarkerView : MonoBehaviour {

    [System.NonSerialized]
    public MarkerIdentity Marker;

    public TextMesh textMesh;

    public UnityEngine.UI.Text uiText;

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


}
