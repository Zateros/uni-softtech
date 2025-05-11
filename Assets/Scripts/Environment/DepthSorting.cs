using UnityEngine;

public class DepthSorting : MonoBehaviour
{
    [SerializeField]
    private bool isStatic = false;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        UpdateSorting();
    }

    void LateUpdate()
    {
        if(!isStatic) UpdateSorting();
    }

    public void UpdateSorting() {
        spriteRenderer.sortingOrder = -(int)(transform.position.y * 10) + 1;
    }
}
