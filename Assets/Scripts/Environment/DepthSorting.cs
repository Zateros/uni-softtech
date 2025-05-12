using UnityEngine;

public class DepthSorting : MonoBehaviour
{
    [SerializeField]
    private bool isStatic = false;
    [SerializeField]
    private int addition = 0;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        UpdateSorting();
    }

    void LateUpdate()
    {
        if (!isStatic) UpdateSorting();
    }

    public void UpdateSorting()
    {
        spriteRenderer.sortingOrder = (int)(transform.position.y * -10) - (int)transform.position.x + addition;
    }
}
