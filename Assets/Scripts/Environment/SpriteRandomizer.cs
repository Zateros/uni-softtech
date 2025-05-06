using System.Collections.Generic;
using UnityEngine;

public class SpriteRandomizer : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> sprites;

    private SpriteRenderer spriteRenderer;
    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        int index = Random.Range(0, sprites.Count);
        spriteRenderer.sprite = sprites[index];
    }

}
