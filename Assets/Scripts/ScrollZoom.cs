using UnityEngine;

public class ScrollZoom : MonoBehaviour
{
    public float minZoom = 0.75f;
    public float maxZoom = 7.75f;
    void LateUpdate()
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize- Input.mouseScrollDelta.y, minZoom, maxZoom);
    }
}
