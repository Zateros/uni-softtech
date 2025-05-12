using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.TestTools;
using System.Collections;

[TestFixture]
public class MapGenerationTests
{
    private GameObject gameManager;
    private GameObject mapGameObject;
    private Map map;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        Tile testTile = ScriptableObject.CreateInstance<Tile>();

        testTile.name = "TestTile";
        testTile.colliderType = Tile.ColliderType.Grid;

        gameManager = new GameObject();
        mapGameObject = new GameObject
        {
            tag = "Map"
        };

        gameManager.SetActive(true);
        mapGameObject.SetActive(true);

        Grid grid = mapGameObject.AddComponent<Grid>();
        grid.cellLayout = GridLayout.CellLayout.Isometric;

        var baseTilemapGO = new GameObject("BaseTilemap")
        {
            tag = "Base"
        };
        var waterTilemapGO = new GameObject("WaterTilemap")
        {
            tag = "Water"
        };
        var obstaclesTilemapGO = new GameObject("ObstaclesTilemap")
        {
            tag = "Obstacles"
        };
        var roadsTilemapGO = new GameObject("RoadsTilemap")
        {
            tag = "UserBought"
        };

        baseTilemapGO.transform.parent = mapGameObject.transform;
        waterTilemapGO.transform.parent = mapGameObject.transform;
        obstaclesTilemapGO.transform.parent = mapGameObject.transform;
        roadsTilemapGO.transform.parent = mapGameObject.transform;

        baseTilemapGO.AddComponent<Tilemap>();
        waterTilemapGO.AddComponent<Tilemap>();
        obstaclesTilemapGO.AddComponent<Tilemap>();
        roadsTilemapGO.AddComponent<Tilemap>();

        map = mapGameObject.AddComponent<Map>();
        map.enabled = false;
        map.bushPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        map.grassPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        map.treePrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        map.lightPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);

        gameManager.AddComponent<GameManager>();
        GameManager.Instance.turist = GameObject.CreatePrimitive(PrimitiveType.Cube);
        map.tiles = new TileBase[]{testTile, testTile, testTile, testTile};
        yield return new WaitForSeconds(0.2f);
        map.enabled = true;

        yield return null;
    }

    [UnityTest]
    public IEnumerator Map_Generation_Contains_Required_Terrain_Types()
    {
        yield return new WaitForSeconds(0.1f);

        map.GenerateMap();

        yield return new WaitForSeconds(0.3f);

        bool hasWater = false;
        bool hasFoliage = false;
        bool hasSand = false;

        for (int x = 0; x < map.Size.x; x++)
        {
            for (int y = 0; y < map.Size.y; y++)
            {
                switch (map.gameMap[x, y])
                {
                    case Terrain.POND:
                    case Terrain.RIVER:
                        hasWater = true;
                        break;
                    case Terrain.BUSH:
                    case Terrain.GRASS:
                    case Terrain.TREE:
                        hasFoliage = true;
                        break;
                    case Terrain.SANDY:
                        hasSand = true;
                        break;
                }
            }
        }

        Assert.IsTrue(hasWater, "Map should contain at least one water tile.");
        Assert.IsTrue(hasFoliage, "Map should contain at least one foliage tile.");
        Assert.IsTrue(hasSand, "Map should contain at least one sandy tile.");
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        Object.DestroyImmediate(mapGameObject);
        Object.DestroyImmediate(gameManager);
        yield return null;
    }
}
