using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Bought items follow mouse till it's clicked.
/// </summary>
public class FollowMouse : MonoBehaviour
{
    private Camera _mainCamera;
    private Vector3 _mousePos;


    void Start()
    {
        _mainCamera = Camera.main;
    }

    /// <summary>
    /// Checks if player puts down items on allowed tiles, if yes, then puts down the item.
    /// </summary>
    void Update()
    {

        _mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3Int pos = GameManager.Instance.GameTable.WorldToCell(_mousePos);
            Vector3 entrance = new Vector3(GameManager.Instance.Entrance.x, GameManager.Instance.Entrance.y, -10);
            
            int x = pos.x;
            int y = pos.y;
            float distance = Vector3.Distance(entrance, _mousePos);

            if (GameManager.Instance.GameTable.IsInBounds(x, y) && GameManager.Instance.GameTable.gameMap[x, y] != Terrain.POND)
            {
                if (gameObject.name == "Jeep" && GameManager.Instance.GameTable.gameMap[x, y] != Terrain.ENTRANCE && distance > 2f)
                {
                    Notifier.Instance.Notify("You can only place Jeeps near the entrance!");
                    return;
                }

                enabled = false;

                if (GetComponent<Animal>() != null) GameManager.Instance.Minimap.AddBlip(gameObject);
                GameManager.Instance.Buy(gameObject, null);
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
            transform.position = new Vector3(_mousePos.x, _mousePos.y, 0);
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(gameObject);
            enabled = false;
        }

    }
}
