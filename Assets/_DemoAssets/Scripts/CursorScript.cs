using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ximmerse.SlideInSDK;
using PolyEngine;


public class CursorScript : MonoBehaviour {

    public float raycastRadius = 0.25f;

    public Transform[] trackingObjects = null;

    public Transform cursorTransform = null;

    public GameObject indicatorPrefab = null;


    /// <summary>
    /// The indicator scalar : x = min distance scale
    /// y = max distance scale
    /// </summary>
    public Vector2 IndicatorScalar = new Vector2(0.15f, 0.5f);

    /// <summary>
    /// The distance range.
    /// </summary>
    public Vector2 DistanceRange = new Vector2(0.3f, 1f);

    Dictionary <Transform, Transform> indicatorDict = new Dictionary<Transform, Transform>();
	
    void Awake ()
    {
        indicatorPrefab.SetActive(false);
    }

	// Update is called once per frame
	void Update () {
        var center = TagTrackingUtil.GetNode(UnityEngine.XR.XRNode.CenterEye);
        var centerCam = center.GetComponent<Camera>();
        foreach (var t in this.trackingObjects)
        {
            Hint(centerCam, t);
        }
	}

    void Hint(Camera cam, Transform target)
    {
        Vector3 screenPoint_T = cam.WorldToViewportPoint(target.position);
        if (screenPoint_T.x >= 0.3f && screenPoint_T.x <= 0.7f && screenPoint_T.y >= 0.3f && screenPoint_T.y <= 0.7f)
        {
            GetIndicator(target).gameObject.SetActive(false);//inbounds, deactivate
        }
        //off screen, show indicator
        else
        {
            //Set direction of indcator
            Transform indicator = GetIndicator(target);
            indicator.gameObject.SetActive(true);
            Vector3 screenDir = Vector3.ProjectOnPlane((target.position - indicator.position), -cam.transform.forward);
            float angle = PEMathf.SignedAngle(screenDir.ToXY (), new Vector2(1, 0));
            Debug.DrawRay(cursorTransform.position, screenDir);
            indicator.transform.localRotation = Quaternion.Euler(0, 0, angle);

//            //Set scalara of indicator:
//            float screenDistance = Vector2.Distance (screenPoint_T.ToXY(), cursorTransform.position.ToXY());
//            float t = Mathf.InverseLerp(DistanceRange.x, DistanceRange.y, screenDistance);
//            float scale = Mathf.Lerp(IndicatorScalar.x, IndicatorScalar.y, t);
//            indicator.localScale = new Vector3(scale, scale, scale);
//            Debug.LogFormat("{0}: {1}, {2}, {3}, {4}, {5}", target.name, screenPoint_T.ToString("F3"), screenDistance, angle, scale, t);
        }
    }

    Transform GetIndicator (Transform key)
    {
        if (indicatorDict.ContainsKey(key))
        {
            return indicatorDict[key];
        }
        else
        {
            GameObject indicator = Instantiate(indicatorPrefab);
            indicator.transform.SetParent(cursorTransform, false);
            indicatorDict.Add(key, indicator.transform);
            return indicator.transform;
        }
    }
}
