using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public enum PlacerMode { LINE, SQUARE }
public enum PlacerType { ROAD, WATER }

public class Placer : MonoBehaviour
{

    [SerializeField]
    private TileBase roadTile;
    [SerializeField]
    private TileBase waterTile;

    [SerializeField]
    private Tilemap roadTilemap;
    [SerializeField]
    private Tilemap waterTilemap;

    [SerializeField]
    private Color validColor = new(0.6078f, 0.9411f, 0.9098f, 0.8f);

    [SerializeField]
    private Color invalidColor = new(1f, 0.1921f, 0f, 0.8f);

    public delegate void OnPlaced(int price);
    public delegate void OnInvalidPlacement();
    public static event OnPlaced onPlaced;
    public static event OnInvalidPlacement onInvalidPlacement;

    private PlacerType type;
    private PlacerMode mode;

    private Map map;
    private Camera cam;
    private Vector3 mouseWorldPos;
    private Vector3 lastMouseWorldPos;
    private Vector3Int startPos;
    private IEnumerable<Vector3Int> inProgress = new List<Vector3Int>();
    private bool validPlacement = true;
    private bool placing = false;
    private Color previewColor;
    private Vector3Int currentCellPos;
    private Vector3Int prevCellPos;
    private Vector3Int? singleGhostPos = null;
    private int defaultSortOrder;
    private TileBase activeTile;
    private Tilemap activeTilemap;
    private Terrain activeCheckingTerrain;
    private IPurchasable activePurchasable;

    void Awake()
    {
        roadTilemap = GameObject.FindWithTag("UserBought").GetComponent<Tilemap>();
        waterTilemap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();
        SetMode(PlacerMode.LINE, PlacerType.ROAD);
    }

    void OnEnable()
    {
        map = GameManager.Instance.GameTable;
        cam = Camera.main;
        activeTilemap.transform.position = new Vector3(activeTilemap.transform.position.x, activeTilemap.transform.position.y, -3);
        activeTilemap.gameObject.GetComponent<TilemapRenderer>().sortingOrder = 0;
    }


    void OnDisable()
    {
        RemoveGhostShape();
        RemoveSingleGhost();
        activeTilemap.transform.position = new Vector3(activeTilemap.transform.position.x, activeTilemap.transform.position.y, 0);
        activeTilemap.gameObject.GetComponent<TilemapRenderer>().sortingOrder = defaultSortOrder;
    }

    void Update()
    {
        mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        currentCellPos = activeTilemap.WorldToCell(mouseWorldPos);
        currentCellPos.z = type == PlacerType.ROAD ? -3 : -1;

        if (map.IsInBounds(currentCellPos.x, currentCellPos.y) && !EventSystem.current.IsPointerOverGameObject())
        {
            prevCellPos = activeTilemap.WorldToCell(lastMouseWorldPos);
            prevCellPos.z = type == PlacerType.ROAD ? -3 : -1;
            int cxx = Mathf.Clamp(currentCellPos.x, 0, map.Size.x);
            int cyy = Mathf.Clamp(currentCellPos.y, 0, map.Size.y);

            int pxx = Mathf.Clamp(prevCellPos.x, 0, map.Size.x);
            int pyy = Mathf.Clamp(prevCellPos.y, 0, map.Size.y);

            if (Input.GetMouseButton(0))
            {
                if (!placing)
                {
                    placing = true;
                    startPos = currentCellPos;
                    if (map.gameMap[currentCellPos.x, currentCellPos.y] != activeCheckingTerrain && !IsPermanentWater(currentCellPos)) activeTilemap.SetTile(currentCellPos, null);
                }
                else
                {
                    HandleGhostShape();
                    lastMouseWorldPos = mouseWorldPos;
                    return;
                }
            }
            else
            {
                if (!validPlacement && placing)
                {
                    RemoveGhostShape();
                    placing = false;
                    lastMouseWorldPos = mouseWorldPos;
                    onInvalidPlacement?.Invoke();
                    return;
                }
                Place();
            }

            if (!placing)
            {
                HandleSingleGhost(cxx, cyy, pxx, pyy);
            }
        }
        else
        {
            RemoveGhostShape();
            RemoveSingleGhost();
        }

        lastMouseWorldPos = mouseWorldPos;
    }

    private void RemoveSingleGhost()
    {
        if (singleGhostPos.HasValue && map.IsInBounds(singleGhostPos.Value.x, singleGhostPos.Value.y))
        {
            if (map.gameMap[singleGhostPos.Value.x, singleGhostPos.Value.y] != activeCheckingTerrain &&
                !IsPermanentWater(singleGhostPos.Value))
            {
                activeTilemap.SetTile(singleGhostPos.Value, null);
            }
        }
        singleGhostPos = null;
    }

    private bool IsPermanentWater(Vector3Int pos)
    {
        var terrain = map.gameMap[pos.x, pos.y];
        return activeCheckingTerrain == Terrain.POND && (terrain == Terrain.RIVER || terrain == Terrain.POND);
    }


    private void RemoveGhostShape()
    {
        if (inProgress != null)
        {
            foreach (var pos in inProgress)
            {
                if (map.IsInBounds(pos.x, pos.y) &&
                    map.gameMap[pos.x, pos.y] != activeCheckingTerrain &&
                    !IsPermanentWater(pos))
                {
                    activeTilemap.SetTile(pos, null);
                }
            }
        }
    }

    private void HandleGhostShape()
    {
        // Remove previously placed ghost line
        if (mouseWorldPos != lastMouseWorldPos)
            RemoveGhostShape();

        // Calculate new
        inProgress = mode == PlacerMode.LINE ? GetContinuousLine(startPos, currentCellPos, currentCellPos.z) : GetFilledSquare(startPos, currentCellPos, currentCellPos.z);
        previewColor = validPlacement ? validColor : invalidColor;

        // Place
        foreach (Vector3Int pos in inProgress)
        {
            if (map.gameMap[pos.x, pos.y] == activeCheckingTerrain || IsPermanentWater(pos)) continue;

            TileChangeData newTile = new(pos, activeTile, previewColor, Matrix4x4.identity);
            activeTilemap.SetTile(newTile, false);
        }
    }

    private void Place()
    {
        if (placing && validPlacement)
        {
            foreach (Vector3Int pos in inProgress)
            {
                activeTilemap.SetTile(pos, null);
                activeTilemap.SetTile(pos, activeTile);
                int price = activePurchasable.Price;

                Terrain terrainOnPos = map.gameMap[pos.x, pos.y];
                if (terrainOnPos == Terrain.HILL) price += Hill.DestroyExtraCost;
                else if ((terrainOnPos == Terrain.RIVER || terrainOnPos == Terrain.POND) && (activeCheckingTerrain != Terrain.POND)) price += Water.DestroyExtraCost;

                onPlaced?.Invoke(price);
                map.SetCell(activeCheckingTerrain, pos.x, pos.y);
            }
            placing = false;
        }
    }

    private void HandleSingleGhost(int currentX, int currentY, int previousX, int previousY)
    {

        // Remove out-of-date ghosts
        RemoveSingleGhost();

        previewColor = validPlacement ? validColor : invalidColor;

        // Place the new one if the selected cell is empty
        if (map.gameMap[currentX, currentY] != activeCheckingTerrain && !IsPermanentWater(new Vector3Int(currentX, currentY)) && !placing)
        {
            TileChangeData newTile = new(currentCellPos, activeTile, previewColor, Matrix4x4.identity);
            activeTilemap.SetTile(newTile, false);
            singleGhostPos = currentCellPos;
        }
    }

    private IEnumerable<Vector3Int> GetContinuousLine(Vector3Int start, Vector3Int end, int z)
    {
        int startX = start.x;
        int startY = start.y;

        int endX = end.x;
        int endY = end.y;

        int dx = Mathf.Abs(endX - startX);
        int dy = Mathf.Abs(endY - startY);

        int sx = startX < endX ? 1 : -1;
        int sy = startY < endY ? 1 : -1;
        int err = dx - dy;
        while (true)
        {
            yield return new Vector3Int(startX, startY, z);

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                if (e2 < dx)
                {
                    yield return new Vector3Int(startX, startY + sy, z);
                }
                err -= dy;
                startX += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                startY += sy;
            }
            if (startX == endX && startY == endY) break;
        }
    }

    public IEnumerable<Vector3Int> GetFilledSquare(Vector3Int start, Vector3Int end, int z)
    {
        int startX = Mathf.Min(start.x, end.x);
        int startY = Mathf.Min(start.y, end.y);
        int endX = Mathf.Max(start.x, end.x);
        int endY = Mathf.Max(start.y, end.y);

        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                yield return new Vector3Int(x, y, z);
            }
        }
    }

    public void SetMode(PlacerMode mode, PlacerType type)
    {
        RemoveSingleGhost();
        RemoveGhostShape();

        if (activeTilemap != null)
        {
            activeTilemap.transform.position = new Vector3(activeTilemap.transform.position.x, activeTilemap.transform.position.y, 0);
            activeTilemap.gameObject.GetComponent<TilemapRenderer>().sortingOrder = defaultSortOrder;
        }

        this.mode = mode;
        this.type = type;

        activeTilemap = type == PlacerType.ROAD ? roadTilemap : waterTilemap;
        activeTile = type == PlacerType.ROAD ? roadTile : waterTile;
        activeCheckingTerrain = type == PlacerType.ROAD ? Terrain.ROAD : Terrain.POND;
        activePurchasable = type == PlacerType.ROAD ? new Road() : new Water();

        defaultSortOrder = activeTilemap.gameObject.GetComponent<TilemapRenderer>().sortingOrder;

        activeTilemap.transform.position = new Vector3(activeTilemap.transform.position.x, activeTilemap.transform.position.y, -3);
        activeTilemap.gameObject.GetComponent<TilemapRenderer>().sortingOrder = 0;
    }
}
