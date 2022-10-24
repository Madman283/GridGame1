using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryCheck : MonoBehaviour
{
    Bounds barrierBounds = new Bounds();

    // Start is called before the first frame update
    void Start()
    {
        barrierBounds = GetComponent<MeshRenderer>().bounds;

    }

    public bool IsWithinBoundaries(Marker marker)
    {
        Bounds markerBounds = marker.gameObject.GetComponent<MeshRenderer>().bounds;
        if (markerBounds == null)
        {
            return false;
        }
        bool inBounds = barrierBounds.Intersects(markerBounds);
        return inBounds;
    }

}
