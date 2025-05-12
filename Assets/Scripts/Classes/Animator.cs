using UnityEngine;

public class Animator : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;
    
    private IEntity entity;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        entity = GetComponent<IEntity>() ?? GetComponentInParent<IEntity>();
        if (!TryGetComponent<SpriteRenderer>(out spriteRenderer)) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        float angle = Mathf.Atan2(entity.Facing.y, entity.Facing.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        angle = (angle + 22.5f) % 360;
        int sliceIndex = Mathf.FloorToInt(angle / 45f);
        spriteRenderer.sprite = sprites[sliceIndex];
    }
}
