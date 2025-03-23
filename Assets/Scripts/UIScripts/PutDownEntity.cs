using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouse : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 mousePosition;


    void Start()
    {
        mainCamera = Camera.main;
    }


    void Update()
    {
        if(!Mouse.current.leftButton.wasPressedThisFrame)
        {
            mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
        }
        else
        {
            enabled = false;
            GameManager.Instance.Buy(gameObject);
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(gameObject);
            enabled = false;
        }

    }
}
