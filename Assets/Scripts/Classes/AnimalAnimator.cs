using UnityEngine;

public class AnimalAnimator : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;
    
    private Animal animal;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animal = GetComponent<Animal>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        float angle = Mathf.Atan2(animal.Facing.y, animal.Facing.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        angle = (angle + 22.5f) % 360;
        int sliceIndex = Mathf.FloorToInt(angle / 45f);
        spriteRenderer.sprite = sprites[sliceIndex];
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < 8; i++)
        {
            float startAngle = (i * 45f) - 22.5f;
            float endAngle = ((i + 1) * 45f) - 22.5f;

            Vector3 start = Quaternion.Euler(0, 0, startAngle) * Vector3.right;
            Vector3 end = Quaternion.Euler(0, 0, endAngle) * Vector3.right;

            Gizmos.DrawLine(transform.position, transform.position + start);
            Gizmos.DrawLine(transform.position, transform.position + end);
        }

        float angleStep = 360f / 32;
        Vector3 previousPoint = transform.position + Vector3.right;

        for (int i = 1; i <= 32; i++)
        {
            float angle = angleStep * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 nextPoint = transform.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, animal.Facing);
    }
}
