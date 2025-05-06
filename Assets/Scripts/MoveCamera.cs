using UnityEngine;
using UnityEngine.EventSystems;

public class MoveCamera : MonoBehaviour
{
    private Vector3 difference;
    private Vector3 origin;

    private bool drag = false;

    void LateUpdate()
    {
        int button = GameManager.Instance.PurchaseMode ? 1 : 0; // Default: left click to move, in purchase mode: right click to move
        if (Input.GetMouseButton(button))
        {
            difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
            if (drag == false)
            {
                drag = true;
                origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else { drag = false; }

        if (drag)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            Camera.main.transform.position = origin - difference;
            float x, y;
            x = Camera.main.transform.position.x;
            y = Camera.main.transform.position.y;
            Camera.main.transform.position = new Vector3(Mathf.Clamp(x, Map.LocalBounds.min.x, Map.LocalBounds.max.x), Mathf.Clamp(y, Map.LocalBounds.min.y, Map.LocalBounds.max.y), -10f);
        }
    }
}
