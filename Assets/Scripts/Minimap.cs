using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [SerializeField]
    private Map map;

    private RawImage image;
    private Texture2D texture;
    private Color[] textureColors;
    private Color[] colors;

    void Start()
    {
        image = GetComponent<RawImage>();
        texture = new Texture2D(map.Size.x, map.Size.y);
        texture.filterMode = FilterMode.Point;
        image.texture = texture;
        textureColors = new Color[map.Size.x * map.Size.y];
        colors = new Color[]{
            new Color(162f/255f, 169f/255f, 71f/255f),
            new Color(251f/255f, 185f/255f, 84f/255f),
            new Color(143f/255f, 211f/255f, 255f/255f),
            new Color(62f/255f, 53f/255f, 70f/255f),
            new Color(103f/255f, 102f/255f, 51f/255f),
            new Color(158f/255f, 69f/255f, 57f/255f),
            new Color(143f/255f, 149f/255f, 59f/255f),
            new Color(144f/255f, 144f/255f, 144f/255f),
        };

        Refresh();
        Map.onMapGenerated += Refresh;
    }

    void Refresh()
    {
        for (int y = 0; y < map.Size.y; y++)
        {
            for (int x = 0; x < map.Size.x; x++)
            {
                textureColors[x + y*texture.height] = colors[GetColorOfCell(map.gameMap[x, y])];
            }
        }
        texture.SetPixels(textureColors);
        texture.Apply();
    }

    private int GetColorOfCell(Terrain cell)
    {
        return cell switch
        {
            Terrain.GRASSY => 0,
            Terrain.SANDY => 1,
            Terrain.RIVER => 2,
            Terrain.POND => 2,
            Terrain.ROAD => 3,
            Terrain.GRASS => 4,
            Terrain.BUSH => 4,
            Terrain.TREE => 5,
            Terrain.HILL => 6,
            _ => 7,
        };
    }
}
