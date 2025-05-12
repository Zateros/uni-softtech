using UnityEngine;
using UnityEngine.EventSystems;

public class MoveCamera : MonoBehaviour
{
    private Vector3 difference;
    private Vector3 origin;

    private bool drag = false;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        Vector3 entrancePos = GameManager.Instance.EntrancePosition;
        entrancePos.z = -10;
        cam.transform.position = entrancePos;
    }

    void LateUpdate()
    {
        int button = GameManager.Instance.PurchaseMode ? 1 : 0; // Default: left click to move, in purchase mode: right click to move
        if (Input.GetMouseButton(button))
        {
            difference = cam.ScreenToWorldPoint(Input.mousePosition) - cam.transform.position;
            if (drag == false)
            {
                drag = true;
                origin = cam.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else { drag = false; }

        if (drag)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            cam.transform.position = origin - difference;
            float x, y;
            x = cam.transform.position.x;
            y = cam.transform.position.y;
            cam.transform.position = new Vector3(Mathf.Clamp(x, Map.LocalBounds.min.x, Map.LocalBounds.max.x), Mathf.Clamp(y, Map.LocalBounds.min.y, Map.LocalBounds.max.y), -10f);
        }
    }
}
