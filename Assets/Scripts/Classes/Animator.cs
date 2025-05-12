using UnityEngine;

public class Animator : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    private IEntity entity;
    private Animal animal;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        entity = GetComponent<IEntity>() ?? GetComponentInParent<IEntity>();
        animal = GetComponent<Animal>() ?? GetComponentInParent<Animal>() ?? null;
        if (!TryGetComponent<SpriteRenderer>(out spriteRenderer)) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (animal == null)
        {
            ChangeSprite();
            return;
        }
        if (animal != null && !GameManager.Instance.IsNight)
        {
            ChangeSprite();
        }
        else if (animal != null && !animal.HasChip && GameManager.Instance.IsNight)
        {
            spriteRenderer.enabled = false;
        }
    }

    void ChangeSprite()
    {
        spriteRenderer.enabled = true;
        float angle = Mathf.Atan2(entity.Facing.y, entity.Facing.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        angle = (angle + 22.5f) % 360;
        int sliceIndex = Mathf.FloorToInt(angle / 45f);
        spriteRenderer.sprite = sprites[sliceIndex];
    }
}
