using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouse : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 mousePos;


    void Start()
    {
        mainCamera = Camera.main;
    }


    void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            enabled = false;
            GameManager.Instance.Minimap.AddBlip(gameObject);
            GameManager.Instance.Buy(gameObject);
        }
        else
        {
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(gameObject);
            enabled = false;
        }

    }
}
