using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    [SerializeField]
    private Vector2Int size = new(100, 100);

    public Vector2Int Size { get { return size; } set { if (value != size) value = size; } }

    [SerializeField]
    private TileBase[] tiles;

#nullable enable
    [SerializeField]
    private GameObject? entrance = null;

    [SerializeField]
    private GameObject? exit = null;

    private GameObject? spawnedEntrance, spawnedExit = null;
#nullable disable

    [SerializeField]
    private int blurKernel = 3;

    [SerializeField]
    private float sigma = 1.2f;

    [SerializeField]
    private float scale = 1f / 3f;

    [SerializeField]
    private float waterScale = 1f / 2f;

    [SerializeField]
    private float obstacleScale = 1f / 2f;

    [SerializeField]
    private float amp = 1f / 6f;

    [SerializeField]
    private float waterAmp = 1f / 3f;

    [SerializeField]
    private float obstacleAmp = 1f / 1.5f;

    [SerializeField]
    private int octave = 6;

    [SerializeField, Range(0f, 1f)]
    private float sandyThreshold = .4f;

    [SerializeField, Range(0f, 1f)]
    private float waterThreshold = .7f;

    [SerializeField, Range(0f, 1f)]
    private float obstacleThreshold = .7f;

    [SerializeField, Range(0f, 1f)]
    private float foliageChance = .7f;

    [SerializeField, Range(1f, 4f)]
    private float foliagePadding = 1.15f;

    // Can be serialized and be used for the minimap
    public Terrain[,] gameMap { get; private set; }

    private Tilemap baseTilemap;
    private Tilemap waterTilemap;
    private Tilemap foliageTilemap;
    private Tilemap obstaclesTilemap;
    private GenerationTools genTools;
    private Color[] map;
    private Vector2Int pastSize;

    void Start()
    {
        baseTilemap = GameObject.FindWithTag("Base").GetComponent<Tilemap>();
        waterTilemap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();
        foliageTilemap = GameObject.FindWithTag("Foliage").GetComponent<Tilemap>();
        obstaclesTilemap = GameObject.FindWithTag("Obstacles").GetComponent<Tilemap>();
        genTools = new();
        gameMap = new Terrain[size.x, size.y];
        map = new Color[size.x * size.y];
        pastSize = size;
        GenerateMap();
    }

    public Color GetMapColor(int x, int y) => map[x * size.x + y];

    public void SetMapColor(Color newColor, int x, int y) { map[x * size.x + y] = newColor; }

    private void GenerateEntranceExitPair()
    {
        if (entrance == null || exit == null) return;
        int axis = Random.Range(1, 3); // 1 - along x axis, 2 - along y axis
        int rotated = Random.Range(1, 3); // 1 - no = entry on the bottom, exit on the top, 2 - vice versa
        int minMove = Random.Range(2, (axis == 1 ? Size.x : Size.y) - 4);
        for (int i = 0; i < (axis == 1 ? Size.x : Size.y); i++)
        {
            Vector3Int pos = Vector3Int.zero;
            if (axis == 1) pos.x = i;
            else pos.y = i;
            List<Terrain> cell = new();
            for (int y = pos.y + 1; y < pos.y - 1; --y)
            {
                for (int x = pos.x + 1; x < pos.x - 1; --x)
                {
                    try
                    {
                        cell.Add(gameMap[x, y]);
                    }
                    catch { }
                }
            }
            if (!(cell.Contains(Terrain.POND) || cell.Contains(Terrain.RIVER) || cell.Contains(Terrain.HILL)) && i >= minMove)
            {
                GameObject gate = Instantiate(rotated == 1 ? entrance : exit, baseTilemap.CellToWorld(pos), Quaternion.identity);
                if (rotated == 1) spawnedEntrance = gate;
                else spawnedExit = gate;
                gameMap[pos.x, pos.y] = rotated == 1 ? Terrain.ENTRANCE : Terrain.EXIT;
                break;
            }
        }
        for (int i = 0; i < (axis == 1 ? Size.x : Size.y); i++)
        {
            Vector3Int pos = new(axis == 1 ? 0 : Size.x - 1, axis == 2 ? 0 : Size.y - 1);
            if (axis == 1) pos.x = i;
            else pos.y = i;
            List<Terrain> cell = new();
            for (int y = pos.y + 1; y < pos.y - 1; --y)
            {
                for (int x = pos.x + 1; x < pos.x - 1; --x)
                {
                    try
                    {
                        cell.Add(gameMap[x, y]);
                    }
                    catch { }
                }
            }
            if (!(cell.Contains(Terrain.POND) || cell.Contains(Terrain.RIVER) || cell.Contains(Terrain.HILL)) && i >= minMove)
            {
                GameObject gate = Instantiate(rotated == 1 ? exit : entrance, baseTilemap.CellToWorld(pos), Quaternion.identity);
                if (rotated == 2) spawnedEntrance = gate;
                else spawnedExit = gate;
                gameMap[pos.x, pos.y] = rotated == 1 ? Terrain.EXIT : Terrain.ENTRANCE;
                break;
            }
        }
    }

    public void GenerateMap()
    {
        baseTilemap.ClearAllTiles();
        waterTilemap.ClearAllTiles();
        foliageTilemap.ClearAllTiles();
        obstaclesTilemap.ClearAllTiles();
        if (spawnedEntrance != null) Destroy(spawnedEntrance);
        if (spawnedExit != null) Destroy(spawnedExit);
        spawnedEntrance = null;
        spawnedExit = null;

        if (pastSize != size)
        {
            map = new Color[size.x * size.y];
            pastSize = size;
        }

        // Generate Perlin Noise for map construction
        for (float y = 0f; y < size.y; y++)
        {
            for (float x = 0f; x < size.x; x++)
            {
                float X = x / size.x;
                float Y = y / size.y;
                int xx = (int)x;
                int yy = (int)y;

                Color temp = new(
                    genTools.Fbm(X * scale, Y * scale, octave) * amp, //Red - Base map
                    genTools.Fbm(2343f + X * obstacleScale, 233f + Y * obstacleScale, octave) * obstacleAmp, //Green - Obstacles
                    -genTools.Fbm(545f + X * waterScale, 33f + Y * waterScale, octave) * waterAmp //Blue - Waters //TODO: Rivers
                );
                temp.a = temp.r * foliagePadding; //Alpha - Foliage (Base map with padding around sandy regions)
                SetMapColor(temp, xx, yy);
            }
        }

        // Gaussian blur to remove "artifacts"
        map = GenerationTools.GaussianBlur(map, size.x, size.y, blurKernel, sigma).GetPixels();

        // Construct the map into tilemaps from the final generated texture
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);

                Color current = GetMapColor(x, y);

                // Base
                bool inSandyRange = current.r > sandyThreshold;
                baseTilemap.SetTile(tilePos, inSandyRange ? tiles[1] : tiles[0]);
                gameMap[x, y] = inSandyRange ? Terrain.SANDY : Terrain.GRASSY;

                // Obstacles
                if (current.g > obstacleThreshold)
                {
                    obstaclesTilemap.SetTile(tilePos, tiles[5]);
                    gameMap[x, y] = Terrain.HILL;
                }

                // Water
                //TODO: Rivers
                if (current.b > waterThreshold && !(gameMap[x, y] == Terrain.HILL))
                {
                    waterTilemap.SetTile(tilePos, tiles[2]);
                    gameMap[x, y] = Terrain.POND;
                }

                // Foliage
                if (current.a <= sandyThreshold && !(gameMap[x, y] == Terrain.POND || gameMap[x, y] == Terrain.RIVER || gameMap[x, y] == Terrain.HILL))
                {
                    if (Random.Range(0f, 1f) > foliageChance)
                    {
                        int foliageType = Random.Range(3, 5);
                        foliageTilemap.SetTile(tilePos, tiles[foliageType]);
                        gameMap[x, y] = foliageType == 3 ? Terrain.GRASS : Terrain.BUSH;
                    }
                }

            }
        }

        GenerateEntranceExitPair();
    }
}
