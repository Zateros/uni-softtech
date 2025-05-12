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
            Vector3 entrance = new Vector3(GameManager.Instance.Entrance.x, GameManager.Instance.Entrance.y, -10);
            
            int x = pos.x;
            int y = pos.y;
            float distance = Vector3.Distance(entrance, mousePos);

            if (GameManager.Instance.GameTable.IsInBounds(x, y) && GameManager.Instance.GameTable.gameMap[x, y] != Terrain.POND)
            {
                Debug.Log(distance);
                Debug.Log(GameManager.Instance.Entrance);
                Debug.Log(mousePos);
                if (gameObject.name == "Jeep" && GameManager.Instance.GameTable.gameMap[x, y] != Terrain.ENTRANCE && distance > 2f)
                {
                    Notifier.Instance.Notify("You can only place Jeeps near the entrance!");
                    return;
                }

                enabled = false;

                if (GetComponent<Animal>() != null) GameManager.Instance.Minimap.AddBlip(gameObject);
                GameManager.Instance.Buy(gameObject);
                if (GetComponent<DepthSorting>() != null || GetComponentInChildren<DepthSorting>() != null)
                {
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
