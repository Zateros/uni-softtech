using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Minimap : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Color[] mapColors = new Color[]{
            new Color(162f/255f, 169f/255f, 71f/255f),
            new Color(251f/255f, 185f/255f, 84f/255f),
            new Color(143f/255f, 211f/255f, 255f/255f),
            new Color(158/255f, 69/255f, 57/255f),
            new Color(103f/255f, 102f/255f, 51f/255f),
            new Color(158f/255f, 69f/255f, 57f/255f),
            new Color(143f/255f, 149f/255f, 59f/255f),
            new Color(144f/255f, 144f/255f, 144f/255f),
        };

    [SerializeField]
    private GameObject blipPrefab;
    [SerializeField]
    private GameObject blipsMask;

    private Map map;
    private RawImage image;
    private Texture2D texture;
    private Color[] textureColors;
    private RectTransform rect;

    void Start()
    {
        map = GameManager.Instance.GameTable;
        image = GetComponent<RawImage>();
        rect = GetComponent<RectTransform>();

        Refresh();
        Map.onMapGenerated += Refresh;
        Map.onMapChanged += Refresh;
    }

    void Refresh()
    {
        texture = new(map.Size.x, map.Size.y)
        {
            filterMode = FilterMode.Point
        };
        image.texture = texture;
        textureColors = new Color[map.Size.x * map.Size.y];
        for (int y = 0; y < map.Size.y; y++)
        {
            for (int x = 0; x < map.Size.x; x++)
            {
                textureColors[x + y * texture.height] = mapColors[GetColorOfCell(map.gameMap[x, y])];
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

    public void OnPointerClick(PointerEventData pointer)
    {
        Debug.Log($"Clicked @ ({pointer.position.x},{pointer.position.y})");
        Vector2 sTM = ScreenToMinimap(pointer.position);
        Debug.Log($"(sTM) Moved to ({sTM.x},{sTM.y})");
        Vector3 pos = MinimapToWorld(sTM);
        Debug.Log($"(pos) Moved to ({pos.x},{pos.y})");

        pos.z = -10;
        Camera.main.transform.position = pos;
    }

    public Vector2 WorldToMinimap(Vector3 pos)
    {
        Vector3Int worldmapAdjusted = map.WorldToCell(pos);
        float normalizedX = Mathf.InverseLerp(0f, map.Size.x, worldmapAdjusted.x);
        float normalizedY = Mathf.InverseLerp(0f, map.Size.y, worldmapAdjusted.y);
        float minimapWidth = rect.rect.width;
        float minimapHeight = rect.rect.height;

        float iconX = (normalizedX * minimapWidth) - (minimapWidth / 2f);
        float iconY = (normalizedY * minimapHeight) - (minimapHeight / 2f);

        return new Vector2(iconX, iconY);
    }

    public Vector2 ScreenToMinimap(Vector2 pos)
    {
        Vector2 inRectPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, pos, null, out inRectPos);
        inRectPos += rect.sizeDelta / 2;

        float x = map.Size.x * (inRectPos.x / rect.rect.width);
        float y = map.Size.y * (inRectPos.y / rect.rect.height);

        return new Vector2(x, y);
    }

    public Vector3 MinimapToWorld(Vector3 pos) => WorldToMinimap(new Vector2(pos.x, pos.y));

    public Vector3 MinimapToWorld(Vector2 pos)
    {
        float minimapWidth = rect.rect.width;
        float minimapHeight = rect.rect.height;

        float x = (pos.x + (minimapWidth / 2f)) / minimapWidth;
        float y = (pos.y + (minimapHeight / 2f)) / minimapHeight;

        int normalizedX = Mathf.FloorToInt(Mathf.Lerp(0f, map.Size.x, x));
        int normalizedY = Mathf.FloorToInt(Mathf.Lerp(0f, map.Size.y, y));

        return map.CellToWorld(new Vector3Int(normalizedX, normalizedY));
    }

    public void AddBlip(GameObject gameObject)
    {
        GameObject newBlip = Instantiate(blipPrefab, blipsMask.transform);
        Blip blip = newBlip.GetComponent<Blip>();
        blip.minimap = this;
        blip.SetMimic(ref gameObject);
    }
}
