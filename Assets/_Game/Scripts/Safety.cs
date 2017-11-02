
using UnityEngine;

public class Safety : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Marker marker = other.GetComponentInParent<Marker>();
        if (marker == null)
        {
            return;
        }

        if (marker.Tile != null)
        {
            marker.transform.position = marker.Tile.transform.position + (Vector3.up * 0.33f);
            Debug.Log("Safety: Trying to correct position.");
        }
    }
}
