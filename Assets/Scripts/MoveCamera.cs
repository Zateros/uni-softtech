using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private Vector3 difference;
    private Vector3 origin;

    private bool drag = false;
    private float lowerX, upperX, lowerY, upperY = 0f;

    void Start()
    {
        RecalculateBounds();
        Map.onMapGenerated += RecalculateBounds;
    }

    void RecalculateBounds()
    {
        foreach (Vector3 vec in Map.bounds)
        {
            lowerX = Mathf.Min(lowerX, vec.x);
            upperX = Mathf.Max(upperX, vec.x);
            lowerY = Mathf.Min(lowerY, vec.y);
            upperY = Mathf.Max(upperY, vec.y);
        }
    }

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
            Camera.main.transform.position = origin - difference;
            float x, y;
            x = Camera.main.transform.position.x;
            y = Camera.main.transform.position.y;
            Camera.main.transform.position = new Vector3(Mathf.Clamp(x, lowerX, upperX), Mathf.Clamp(y, lowerY, upperY), -10f);
        }
    }
}
