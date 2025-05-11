using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
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
    [SerializeField]
    private RectTransform cameraView;
    [SerializeField]
    private float cameraViewScale = 1f;

    private Map map;
    private Camera cam;
    private RawImage image;
    private Texture2D texture;
    private Color[] textureColors;
    private RectTransform rect;

    void Start()
    {
        map = GameManager.Instance.GameTable;
        cam = Camera.main;
        image = GetComponent<RawImage>();
        rect = GetComponent<RectTransform>();

        Refresh();
        Map.onMapGenerated += Refresh;
        Map.onMapChanged += Refresh;
    }

    void LateUpdate()
    {
        Bounds bounds = GetCameraWorldBounds();

        Vector2 size = bounds.size * cameraViewScale;
        Vector2 center = WorldToMinimap(bounds.center);

        cameraView.anchoredPosition = center;
        cameraView.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
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

    private Bounds GetCameraWorldBounds()
    {
        float z = Mathf.Abs(cam.transform.position.z);

        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, z));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, z));

        return new Bounds { min = bottomLeft, max = topRight };
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

    public void OnPointerDown(PointerEventData pointer)
    {
        Vector2 sTM = ScreenToMinimap(pointer.position);
        Vector3 pos = map.GetCellCenterWorld(new Vector3Int((int)sTM.x, (int)sTM.y, -10));
        Camera.main.transform.position = pos;
    }

    public void OnDrag(PointerEventData pointer)
    {
        Vector2 sTM = ScreenToMinimap(pointer.position);
        Vector3 target = map.CellToWorld(new Vector3Int((int)sTM.x, (int)sTM.y, -10));
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, target, 65f * Time.deltaTime);
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

    public void AddBlip(GameObject gameObject)
    {
        GameObject newBlip = Instantiate(blipPrefab, blipsMask.transform);
        Blip blip = newBlip.GetComponent<Blip>();
        blip.minimap = this;
        blip.SetMimic(ref gameObject);
    }

    private void OnDisable()
    {
        Map.onMapGenerated -= Refresh;
        Map.onMapChanged -= Refresh;
    }
}
