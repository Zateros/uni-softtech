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
        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3Int pos = GameManager.Instance.GameTable.WorldToCell(mousePos);
            
            int x = pos.x;
            int y = pos.y;

            if (GameManager.Instance.GameTable.IsInBounds(x,y) && GameManager.Instance.GameTable.gameMap[x, y] != Terrain.POND && GameManager.Instance.GameTable.gameMap[x, y] != Terrain.RIVER)
            {
                enabled = false;

                if (GetComponent<Animal>() != null) GameManager.Instance.Minimap.AddBlip(gameObject);
                GameManager.Instance.Buy(gameObject);
                if (GetComponent<DepthSorting>() != null || GetComponentInChildren<DepthSorting>() != null) {
                    DepthSorting sorting = GetComponent<DepthSorting>() ?? GetComponentInChildren<DepthSorting>();
                    sorting.UpdateSorting();
                }
            }
            else
            {
                Notifier.Instance.Notify("You can't place items there!");
            }
        }
        else
        {
            transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(gameObject);
            enabled = false;
        }

    }
}
