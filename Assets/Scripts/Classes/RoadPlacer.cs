using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoadPlacer : MonoBehaviour
{
    [SerializeField]
    private Map map;

    [SerializeField]
    private TileBase roadTile;

    [SerializeField]
    private string userBoughtTag = "UserBought";

    [SerializeField]
    private Color validColor = new(0.6078f, 0.9411f, 0.9098f, 0.8f);

    [SerializeField]
    private Color invalidColor = new(1f, 0.1921f, 0f, 0.8f);

    public delegate void OnRoadPlaced(int count);
    public delegate void OnInvalidRoadPlacement();
    public static event OnRoadPlaced onRoadPlaced;
    public static event OnInvalidRoadPlacement onInvalidRoadPlacement;

    private Tilemap roadTilemap;
    private Camera cam;
    private Vector3 mouseWorldPos;
    private Vector3 lastMouseWorldPos;
    private Vector3Int lineStart;
    private IEnumerable<Vector3Int> inProgressLine;
    private bool validPlacement = true;
    private bool placing = false;
    private Color previewColor;
    private Vector3Int currentCellPos;
    private Vector3Int prevCellPos;

    void OnEnable()
    {
        roadTilemap = GameObject.FindWithTag(userBoughtTag).GetComponent<Tilemap>();
        cam = Camera.main;
        roadTilemap.transform.position = new Vector3(roadTilemap.transform.position.x, roadTilemap.transform.position.y, -3);
    }


    void OnDisable()
    {
        if (map.gameMap[currentCellPos.x, currentCellPos.y] != Terrain.ROAD && map.IsInBounds(currentCellPos.x, currentCellPos.y)) roadTilemap.SetTile(currentCellPos, null);
        if (map.gameMap[prevCellPos.x, prevCellPos.y] != Terrain.ROAD && map.IsInBounds(prevCellPos.x, prevCellPos.y)) roadTilemap.SetTile(prevCellPos, null);
        roadTilemap.transform.position = new Vector3(roadTilemap.transform.position.x, roadTilemap.transform.position.y, 0);
    }

    void Update()
    {
        mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        currentCellPos = roadTilemap.WorldToCell(mouseWorldPos);
        currentCellPos.z = -3;

        if (map.IsInBounds(currentCellPos.x, currentCellPos.y))
        {
            prevCellPos = roadTilemap.WorldToCell(lastMouseWorldPos);
            prevCellPos.z = -3;
            int cxx = Mathf.Clamp(currentCellPos.x, 0, map.Size.x);
            int cyy = Mathf.Clamp(currentCellPos.y, 0, map.Size.y);

            int pxx = Mathf.Clamp(prevCellPos.x, 0, map.Size.x);
            int pyy = Mathf.Clamp(prevCellPos.y, 0, map.Size.y);

            if (Input.GetMouseButton(0))
            {
                if (!placing)
                {
                    placing = true;
                    lineStart = currentCellPos;
                    roadTilemap.SetTile(currentCellPos, null);
                }
                else
                {
                    HandleGhostLine();
                    lastMouseWorldPos = mouseWorldPos;
                    return;
                }
            }
            else
            {
                if (!validPlacement && placing)
                {
                    RemoveGhostLine();
                    placing = false;
                    lastMouseWorldPos = mouseWorldPos;
                    return;
                }
                PlaceRoads();
            }

            if (!placing)
            {
                HandleSingleGhost(cxx, cyy, pxx, pyy);
            }
        }

        lastMouseWorldPos = mouseWorldPos;
    }

    private bool ValidateLine()
    {
        foreach (Vector3Int pos in inProgressLine)
        {
            if (!ValidateSingle(pos.x, pos.y)) return false;
        }
        return true;
    }

    private bool ValidateSingle(int x, int y) => !(map.gameMap[x, y] == Terrain.RIVER || map.gameMap[x, y] == Terrain.POND || map.gameMap[x, y] == Terrain.HILL);

    private void RemoveGhostLine()
    {
        foreach (Vector3Int pos in inProgressLine)
        {
            if (map.gameMap[pos.x, pos.y] != Terrain.ROAD && map.IsInBounds(pos.x, pos.y)) roadTilemap.SetTile(pos, null);
        }
    }

    private void HandleGhostLine()
    {
        // Remove previously placed ghost line
        if (mouseWorldPos != lastMouseWorldPos)
            RemoveGhostLine();

        // Calculate new
        inProgressLine = GetContinuousLine(lineStart, currentCellPos, currentCellPos.z);
        validPlacement = ValidateLine();
        previewColor = validPlacement ? validColor : invalidColor;

        // Place
        foreach (Vector3Int pos in inProgressLine)
        {
            if (map.gameMap[pos.x, pos.y] == Terrain.ROAD) continue;

            TileChangeData newTile = new(pos, roadTile, previewColor, Matrix4x4.identity);
            roadTilemap.SetTile(newTile, false);
        }
    }

    private void PlaceRoads()
    {
        if (placing && validPlacement)
        {
            int count = 0;
            foreach (Vector3Int pos in inProgressLine)
            {
                roadTilemap.SetTile(pos, null);
                roadTilemap.SetTile(pos, roadTile);
                map.SetCell(Terrain.ROAD, pos.x, pos.y);
                count++;
            }
            placing = false;
            onRoadPlaced?.Invoke(count);
        }
    }

    private void HandleSingleGhost(int currentX, int currentY, int previousX, int previousY)
    {

        // Remove out-of-date ghosts
        if (mouseWorldPos != lastMouseWorldPos)
        {
            if (map.IsInBounds(previousX, previousY) && map.gameMap[previousX, previousY] != Terrain.ROAD) roadTilemap.SetTile(prevCellPos, null);
        }

        validPlacement = ValidateSingle(currentX, currentY);
        previewColor = validPlacement ? validColor : invalidColor;

        // Place the new one if the selected cell is empty
        if (map.gameMap[currentX, currentY] != Terrain.ROAD && !placing)
        {
            TileChangeData newTile = new(currentCellPos, roadTile, previewColor, Matrix4x4.identity);
            roadTilemap.SetTile(newTile, false);
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
}
