using System.Collections.Generic;
using System.Linq;
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

    [SerializeField]
    private float riverSwingMagnitude = 2.0f;
    [SerializeField]
    private int minRiverWidth = 1;
    [SerializeField]
    private int maxRiverWidth = 3;


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
    public GameObject bushPrefab;
    [SerializeField, Range(0f, 1f)]
    private float bushChance = 0.397f;
    [SerializeField]
    public GameObject grassPrefab;
    [SerializeField, Range(0f, 1f)]
    private float grassChance = 0.968f;
    [SerializeField]
    public GameObject treePrefab;
    [SerializeField]
    public GameObject lightPrefab;

    public delegate void OnMapGenerated();
    public delegate void OnMapChanged();
    public static OnMapGenerated onMapGenerated;
    public static OnMapChanged onMapChanged;

    // Can be serialized and be used for the minimap
    public Terrain[,] gameMap { get; private set; }
    public GameObject[,] lights { get; private set; }


    private Tilemap baseTilemap;
    private Tilemap waterTilemap;
    private Tilemap obstaclesTilemap;
    private Tilemap roadsTilemap;
    private GenerationTools genTools;
    private Color[] map;
    private HashSet<Vector2> riverVisitedPositions = new HashSet<Vector2>();

    private float sandyDifficultyThresholdModif;
    private float waterDifficultyThresholdModif;
    private float obstacleDifficultyThresholdModif;
    private float foliageDifficultyThresholdModif;

    private float waterDifficultyScaleModif;

    private float riverDifficultySwingMagnitudeModif;

    void Awake()
    {
        baseTilemap = GameObject.FindWithTag("Base").GetComponent<Tilemap>();
        waterTilemap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();
        obstaclesTilemap = GameObject.FindWithTag("Obstacles").GetComponent<Tilemap>();
        roadsTilemap = GameObject.FindWithTag("UserBought").GetComponent<Tilemap>();

        genTools = new();
    }

    void Start()
    {
        lights = new GameObject[size.x, size.y];
        if (Application.isPlaying)
        {
            GenerateMap();
        }
    }

    private void Init()
    {
        float difficulty = (float)GameManager.Instance.Difficulty;

        sandyDifficultyThresholdModif = 0.075f * difficulty;
        waterDifficultyThresholdModif = 0.08f * difficulty;
        obstacleDifficultyThresholdModif = 0.15f * difficulty;
        foliageDifficultyThresholdModif = 0.1f * difficulty;

        waterDifficultyScaleModif = 1.15f * (1f - difficulty);

        riverDifficultySwingMagnitudeModif = 0.25f * difficulty;

        gameMap = new Terrain[size.x, size.y];
        map = new Color[size.x * size.y];
    }

    private Color GetMapColor(int x, int y) => map[x * size.x + y];
    private void SetMapColor(Color newColor, int x, int y) { map[x * size.x + y] = newColor; }

    private void GenerateEntranceExitPair()
    {
        if (entrance == null || exit == null) return;

        int rotatedChance = Random.Range(0, 50); // even - entrance on the bottom, exit on the top; odd - vice versa
        int iteration = 0;

        while ((spawnedEntrance == null || spawnedExit == null) && iteration < maxIterationForEntranceGeneration)
        {
            var (point1, point2) = GenerationTools.PointsAlongOppositeSquareSides(Mathf.Min(Size.x, Size.y), 2f);

            Vector3Int entrancePos = new Vector3Int(Mathf.RoundToInt(point1.x), Mathf.RoundToInt(point1.y), 0);
            Vector3Int exitPos = new Vector3Int(Mathf.RoundToInt(point2.x), Mathf.RoundToInt(point2.y), 0);

            if (IsValidTerrain(entrancePos) && IsValidTerrain(exitPos))
            {
                bool rotated = rotatedChance % 2 == 0;

                GameObject gate1 = Instantiate(rotated ? entrance : exit, baseTilemap.CellToWorld(entrancePos), Quaternion.identity);
                GameObject gate2 = Instantiate(rotated ? exit : entrance, baseTilemap.CellToWorld(exitPos), Quaternion.identity);

                if (rotated)
                {
                    spawnedEntrance = gate1;
                    spawnedExit = gate2;
                    gameMap[entrancePos.x, entrancePos.y] = Terrain.ENTRANCE;
                    gameMap[exitPos.x, exitPos.y] = Terrain.EXIT;
                }
                else
                {
                    spawnedEntrance = gate2;
                    spawnedExit = gate1;
                    gameMap[entrancePos.x, entrancePos.y] = Terrain.EXIT;
                    gameMap[exitPos.x, exitPos.y] = Terrain.ENTRANCE;
                }
            }

            iteration++;
        }
    }

    private bool IsValidTerrain(Vector3Int pos)
    {
        if (!IsInBounds(pos.x, pos.y)) return false;

        List<Terrain> cell = new();
        for (int y = pos.y - 1; y <= pos.y + 1; y++)
        {
            for (int x = pos.x - 1; x <= pos.x + 1; x++)
            {
                if (IsInBounds(x, y))
                {
                    cell.Add(gameMap[x, y]);
                }
            }
        }
        return !(cell.Contains(Terrain.POND) || cell.Contains(Terrain.RIVER) || cell.Contains(Terrain.HILL));
    }

    private Color[] GenerateRivers()
    {
        Color[] riverMap = Enumerable.Repeat(Color.black, size.x * size.y).ToArray();

        int pointAttempts = 0;
        Vector2 p1 = Vector2.zero;
        Vector2 p2 = Vector2.zero;

        while (pointAttempts < 500)
        {
            (p1, p2) = GenerationTools.PointsAlongOppositeSquareSides(size.x, 10f);

            if (!isHill(Mathf.RoundToInt(p1.x), Mathf.RoundToInt(p1.y)) &&
                !isHill(Mathf.RoundToInt(p2.x), Mathf.RoundToInt(p2.y)))
            {
                break;
            }

            pointAttempts++;
        }

        if (pointAttempts == 500)
        {
            return riverMap;
        }

        Vector2 pos = p1;
        Vector2 prevPos = Vector2.zero;

        float swingSlice = Random.Range(0f, 10000f);
        float widthSlice = Random.Range(0f, 10000f);

        genTools.Reseed();

        int step = 0;

        while (true)
        {
            if (step > 1000)
            {
                break;
            }

            int x = Mathf.RoundToInt(pos.x);
            int y = Mathf.RoundToInt(pos.y);

            if (!IsInBounds(x, y))
                break;

            float widthNoise = genTools.PerlinNoise(widthSlice, step * 0.05f);
            int riverWidth = Mathf.Clamp(Mathf.RoundToInt(widthNoise * maxRiverWidth), minRiverWidth, maxRiverWidth);

            List<Vector2> brushStrokes = VariableWidthStroke(pos, riverWidth);

            foreach (var strokePos in brushStrokes)
            {
                int sx = Mathf.RoundToInt(strokePos.x);
                int sy = Mathf.RoundToInt(strokePos.y);

                if (sx >= 0 && sx < size.x && sy >= 0 && sy < size.y)
                {
                    riverMap[sx * size.x + sy] = Color.white;
                }
            }

            riverVisitedPositions.Add(pos);

            if (x == Mathf.RoundToInt(p2.x) && y == Mathf.RoundToInt(p2.y))
                break;

            // Random swinging
            float noiseValue = genTools.PerlinNoise(swingSlice, step * 0.1f) - 0.5f;
            Vector2 direction = (p2 - pos).normalized;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            Vector2 swing = perpendicular * noiseValue * (riverSwingMagnitude + riverDifficultySwingMagnitudeModif);

            // Forward check for hills
            Vector2 nextStep = direction + swing;
            Vector2 proposedPosition = pos + nextStep.normalized;

            if (isHill(Mathf.RoundToInt(proposedPosition.x), Mathf.RoundToInt(proposedPosition.y)) || riverVisitedPositions.Contains(proposedPosition))
            {
                proposedPosition = FloodFillEscape(pos, prevPos);

                if (proposedPosition == pos)
                {
                    break;
                }
            }

            prevPos = pos;
            pos = proposedPosition;
            step++;
        }

        riverVisitedPositions.Clear();
        return riverMap;
    }

    private bool isHill(int x, int y)
    {
        if (x < 0 || x >= size.x || y < 0 || y >= size.y)
            return true;

        return gameMap[x, y] == Terrain.HILL;
    }

    private Vector2 FloodFillEscape(Vector2 startPosition, Vector2 previousPosition)
    {
        Queue<Vector2> queue = new Queue<Vector2>();
        HashSet<Vector2> visited = new HashSet<Vector2>();

        queue.Enqueue(startPosition);
        visited.Add(startPosition);

        Vector2[] directions = {
            new Vector2(1, 0), new Vector2(-1, 0),
            new Vector2(0, 1), new Vector2(0, -1)
        };

        int floodFillSteps = 0;

        while (queue.Count > 0)
        {
            if (floodFillSteps > 100000)
            {
                break;
            }

            Vector2 current = queue.Dequeue();
            floodFillSteps++;

            Vector2 prevStepDir = (startPosition - previousPosition).normalized;

            foreach (var dir in directions)
            {
                if (dir == -prevStepDir)
                {
                    continue;
                }

                Vector2 neighbor = current + dir;
                int nx = Mathf.RoundToInt(neighbor.x);
                int ny = Mathf.RoundToInt(neighbor.y);

                if (!isHill(nx, ny) && !visited.Contains(neighbor) && !riverVisitedPositions.Contains(neighbor))
                {
                    return neighbor;
                }

                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return startPosition;
    }

    private List<Vector2> VariableWidthStroke(Vector2 center, int radius)
    {
        List<Vector2> points = new List<Vector2>();

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (isHill(x, y)) continue;

                if (x * x + y * y <= radius * radius)
                {
                    points.Add(new Vector2(center.x + x, center.y + y));
                }
            }
        }

        return points;
    }

    public void SetCell(Terrain terrain, int x, int y)
    {
        if (gameMap[x, y] == Terrain.BUSH || gameMap[x, y] == Terrain.GRASS || gameMap[x, y] == Terrain.TREE)
        {
            Destroy(GameManager.Instance.Plants[x, y].gameObject);
            GameManager.Instance.Plants[x, y] = null;
        }

        Vector3Int pos = new Vector3Int(x, y);
        if (terrain != Terrain.ROAD && gameMap[x, y] == Terrain.ROAD)
        {
            pos.z = -3;
            roadsTilemap.SetTile(pos, null);
            Destroy(lights[x, y]);
            lights[x, y] = null;
        }
        if (terrain != Terrain.RIVER && terrain != Terrain.POND && (gameMap[x, y] == Terrain.RIVER || gameMap[x, y] == Terrain.POND))
        {
            pos.z = -1;
            waterTilemap.SetTile(pos, null);
        }
        if (gameMap[x, y] == Terrain.HILL)
        {
            pos.z = -4;
            obstaclesTilemap.SetTile(pos, null);
        }
        if (terrain == Terrain.ROAD)
        {
            lights[x, y] = Instantiate(lightPrefab, CellToWorld(pos), Quaternion.identity);
            lights[x, y].GetComponent<NightLight>().isRoad = true;
        }

        gameMap[x, y] = terrain;
        onMapChanged?.Invoke();
    }

    public bool IsInBounds(int x, int y) => IsInBounds(x, y, 0);
    public bool IsInBounds(int x, int y, int inset) => 0 + inset <= x && x < Size.x - inset && 0 + inset <= y && y < Size.y - inset;

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
                    -genTools.Fbm(545f + X * (waterScale - waterDifficultyScaleModif), 33f + Y * (waterScale - waterDifficultyScaleModif), octave) * waterAmp //Blue - Waters
                );
                SetMapColor(temp, xx, yy);
            }
        }

        // Gaussian blur to remove "artifacts"
        map = GenerationTools.GaussianBlur(map, size.x, size.y, blurKernel, sigma).GetPixels();

        bool[,] sandyRanges = new bool[size.x, size.y];

        // Construct the map and the obstacles into tilemaps from the final generated texture
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);

                Color current = GetMapColor(x, y);

                // Base
                sandyRanges[x, y] = current.r > (sandyThreshold - sandyDifficultyThresholdModif);
                baseTilemap.SetTile(tilePos, sandyRanges[x, y] ? tiles[1] : tiles[0]);
                gameMap[x, y] = sandyRanges[x, y] ? Terrain.SANDY : Terrain.GRASSY;

                // Obstacles
                if (current.g > (obstacleThreshold - obstacleDifficultyThresholdModif))
                {
                    tilePos.z = -4;
                    obstaclesTilemap.SetTile(tilePos, tiles[3]);
                    gameMap[x, y] = Terrain.HILL;
                }
            }
        }
        // Generate the rivers
        for (int i = 0; i < Random.Range(1, 4); i++)
        {
            int colorIndex = 0;
            Color[] river = GenerateRivers();
            map = map.Zip(river, (first, second) =>
            {
                if (second.b == 1f)
                {
                    int x = colorIndex / Size.x;
                    int y = colorIndex % Size.x;
                    if (!isHill(x, y)) gameMap[x, y] = Terrain.RIVER;
                    colorIndex++;
                    return new Color(first.r, first.g, second.b);
                }
                else
                {
                    colorIndex++;
                    return first;
                }
            }).ToArray();
        }

        // Generate the rest of the map
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                Color current = GetMapColor(x, y);

                // Water
                if (current.b > (waterThreshold + waterDifficultyThresholdModif) && !(gameMap[x, y] == Terrain.HILL))
                {
                    tilePos.z = -1;
                    waterTilemap.SetTile(tilePos, tiles[2]);
                    if (gameMap[x, y] != Terrain.RIVER) gameMap[x, y] = Terrain.POND;
                }

                // Foliage
                if (!sandyRanges[x, y] && IsInBounds(x, y, foliageInset) &&
                !(gameMap[x, y] == Terrain.SANDY || gameMap[x, y] == Terrain.POND || gameMap[x, y] == Terrain.RIVER || gameMap[x, y] == Terrain.HILL)
                )
                {
                    if (Random.Range(0f, 1f) > (foliageChance + foliageDifficultyThresholdModif))
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
                        Vector3 insideRandomCircle = Random.insideUnitCircle / 10f;
                        insideRandomCircle.z = 0f;
                        Vector3 foliagePosition = GetCellCenterWorld(new Vector3Int(x, y)) + insideRandomCircle;
                        foliage = Instantiate(foliagePrefab, foliagePosition, Quaternion.identity);
                        FollowMouse mouse = foliage.GetComponent<FollowMouse>();
                        if (mouse != null) mouse.enabled = false;
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
