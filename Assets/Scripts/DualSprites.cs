using UnityEngine;

public class DualSprites : MonoBehaviour {
    public Sprite[] sprites;

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (rb != null && sr.sprite == sprites[1] && rb.gravityScale > 0f)
            rb.gravityScale = -rb.gravityScale;
    }
}
