using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    private Vector2Int size;

    public Vector3Int WorldToCell(Vector3 pos) => baseTilemap.WorldToCell(pos);

    public Vector3 CellToWorld(Vector3Int pos) => baseTilemap.CellToWorld(pos);
    public Vector3 GetCellCenterWorld(Vector3Int pos) => baseTilemap.GetCellCenterWorld(pos);

    public Vector2Int Size { get { return size; } set { if (value != size) size = value; } }
    public static Bounds LocalBounds { get; private set; }
    public Vector3Int Origin { get { return baseTilemap.origin; } }

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

    [SerializeField]
    private int maxIterationForEntranceGeneration = 10;

    [SerializeField, Range(0f, 1f)]
    private float foliageChance = .7f;

    [SerializeField]
    private int foliageInset = 1;
    [SerializeField]
    private GameObject bushPrefab;
    [SerializeField, Range(0f, 1f)]
    private float bushChance = 0.32f;
    [SerializeField]
    private GameObject grassPrefab;
    [SerializeField, Range(0f, 1f)]
    private float grassChance = 0.79f;
    [SerializeField]
    private GameObject treePrefab;


    public delegate void OnMapGenerated();
    public delegate void OnMapChanged();
    public static OnMapGenerated onMapGenerated;
    public static OnMapChanged onMapChanged;

    // Can be serialized and be used for the minimap
    public Terrain[,] gameMap { get; private set; }


    private Tilemap baseTilemap;
    private Tilemap waterTilemap;
    private Tilemap obstaclesTilemap;
    private GenerationTools genTools;
    private Color[] map;

    private float sandyDifficultyModif;
    private float waterDifficultyModif;
    private float obstacleDifficultyModif;
    private float foliageDifficultyModif;

    void Awake()
    {
        baseTilemap = GameObject.FindWithTag("Base").GetComponent<Tilemap>();
        waterTilemap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();
        obstaclesTilemap = GameObject.FindWithTag("Obstacles").GetComponent<Tilemap>();

        genTools = new();
        GenerateMap();
    }

    private void Init()
    {
        sandyDifficultyModif = 0.075f * (float)GameManager.Instance.Difficulty;
        waterDifficultyModif = 0.08f * (float)GameManager.Instance.Difficulty;
        obstacleDifficultyModif = 0.15f * (float)GameManager.Instance.Difficulty;
        foliageDifficultyModif = 0.1f * (float)GameManager.Instance.Difficulty;

        Debug.Log($"sandyDifficultyModif: {sandyDifficultyModif}");
        Debug.Log($"waterDifficultyModif: {waterDifficultyModif}");
        Debug.Log($"obstacleDifficultyModif: {obstacleDifficultyModif}");
        Debug.Log($"foliageDifficultyModif: {foliageDifficultyModif}");
        Debug.Log("--------------");
        Debug.Log($"sandyThreshold: {sandyThreshold}");
        Debug.Log($"waterThreshold: {waterThreshold}");
        Debug.Log($"obstacleThreshold: {obstacleThreshold}");
        Debug.Log($"foliageChance: {foliageChance}");
        Debug.Log("--------------");
        Debug.Log($"sandyThreshold - sandyDifficultyModif: {sandyThreshold - sandyDifficultyModif}");
        Debug.Log($"waterThreshold + waterDifficultyModif: {waterThreshold + waterDifficultyModif}");
        Debug.Log($"obstacleThreshold - obstacleDifficultyModif: {obstacleThreshold - obstacleDifficultyModif}");
        Debug.Log($"foliageChance + foliageDifficultyModif: {foliageChance + foliageDifficultyModif}");

        gameMap = new Terrain[size.x, size.y];
        map = new Color[size.x * size.y];
    }

    private Color GetMapColor(int x, int y) => map[x * size.x + y];
    private void SetMapColor(Color newColor, int x, int y) { map[x * size.x + y] = newColor; }

    private void GenerateEntranceExitPair()
    {
        if (entrance == null || exit == null) return;
        int rotated = Random.Range(1, 3); // 1 - no = entry on the bottom, exit on the top, 2 - vice versa
        int iteration = 0;

        // This is ugly, need to find a better way to generate them
        // without spawning in front of an obstacle, basically softlocking the session

        // Guarantee spawned entrance-exit pair
        while ((spawnedEntrance == null || spawnedExit == null) && iteration < maxIterationForEntranceGeneration)
        {
            int axis = Random.Range(1, 3); // 1 - along x axis, 2 - along y axis
            int minMove = Random.Range(2, (axis == 1 ? Size.x : Size.y) - 4);
            for (int i = 0; i < (axis == 1 ? Size.x : Size.y); i++)
            {
                Vector3Int pos = Vector3Int.zero;
                if (axis == 1) pos.x = i;
                else pos.y = i;
                List<Terrain> cell = new();

                // Collecting radius 1 neighbouring cells
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
            iteration++;
        }
    }

    public void SetCell(Terrain terrain, int x, int y)
    {
        if (gameMap[x, y] == Terrain.BUSH || gameMap[x, y] == Terrain.GRASS || gameMap[x, y] == Terrain.TREE)
        {
            Destroy(GameManager.Instance.Plants[x, y].gameObject);
            GameManager.Instance.Plants[x, y] = null;
        }
        gameMap[x, y] = terrain;
        onMapChanged?.Invoke();
    }

    public bool IsInBounds(int x, int y) => IsInBounds(x, y, 0);
    public bool IsInBounds(int x, int y, int inset) => (0 + inset <= x && x < Size.x - inset) && (0 + inset <= y && y < Size.y - inset);

    public void GenerateMap()
    {
        baseTilemap.ClearAllTiles();
        waterTilemap.ClearAllTiles();
        obstaclesTilemap.ClearAllTiles();
        if (spawnedEntrance != null) Destroy(spawnedEntrance);
        if (spawnedExit != null) Destroy(spawnedExit);
        spawnedEntrance = null;
        spawnedExit = null;

        Init();
        genTools.Reseed();

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
                bool inSandyRange = current.r > (sandyThreshold - sandyDifficultyModif);
                baseTilemap.SetTile(tilePos, inSandyRange ? tiles[1] : tiles[0]);
                gameMap[x, y] = inSandyRange ? Terrain.SANDY : Terrain.GRASSY;

                // Obstacles
                if (current.g > (obstacleThreshold - obstacleDifficultyModif))
                {
                    tilePos.z = -4;
                    obstaclesTilemap.SetTile(tilePos, tiles[5]);
                    gameMap[x, y] = Terrain.HILL;
                }

                // Water
                //TODO: Rivers
                if (current.b > (waterThreshold + waterDifficultyModif) && !(gameMap[x, y] == Terrain.HILL))
                {
                    tilePos.z = -1;
                    waterTilemap.SetTile(tilePos, tiles[2]);
                    gameMap[x, y] = Terrain.POND;
                }

                // Foliage
                if (!inSandyRange && IsInBounds(x, y, foliageInset) &&
                !(gameMap[x, y] == Terrain.SANDY || gameMap[x, y] == Terrain.POND || gameMap[x, y] == Terrain.RIVER || gameMap[x, y] == Terrain.HILL)
                )
                {
                    if (Random.Range(0f, 1f) > (foliageChance + foliageDifficultyModif))
                    {
                        float foliageChance = Random.Range(0f, 1f);
                        GameObject foliage;
                        GameObject foliagePrefab;

                        if (foliageChance <= bushChance)
                        {
                            foliagePrefab = bushPrefab;
                            gameMap[x, y] = Terrain.BUSH;
                        }
                        else if (foliageChance > bushChance && foliageChance <= grassChance)
                        {
                            foliagePrefab = grassPrefab;
                            gameMap[x, y] = Terrain.GRASS;
                        }
                        else
                        {
                            foliagePrefab = treePrefab;
                            gameMap[x, y] = Terrain.TREE;
                        }
                        Vector3 insideRandomCircle = Random.insideUnitCircle / 4f;
                        insideRandomCircle.z = 0f;
                        Vector3 foliagePosition = GetCellCenterWorld(new Vector3Int(x, y)) + insideRandomCircle;
                        foliage = Instantiate(foliagePrefab, foliagePosition, Quaternion.identity);
                        foliage.GetComponent<FollowMouse>().enabled = false;
                        GameManager.Instance.Plants[x, y] = foliage.GetComponent<Plant>();
                    }
                }

            }
        }

        GenerateEntranceExitPair();

        LocalBounds = baseTilemap.localBounds;

        onMapGenerated?.Invoke();
    }
}
