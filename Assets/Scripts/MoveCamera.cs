using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private Vector3 difference;
    private Vector3 origin;

    private bool drag = false;

    void LateUpdate()
    {
        if(Input.GetMouseButton(0)) {
            difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
            if(drag == false) {
                drag = true;
                origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }else { drag = false; }

        if(drag) { Camera.main.transform.position = origin - difference; }
    }
}
