using UnityEngine;

public class CombinedTile : MonoBehaviour {
    public Sprite[] lightSprites;
    public Sprite[] darkSprites;
    [HideInInspector]
    public bool lightTile = false;
    private bool isPreview = false;

    public void SetTileLight(bool light) 
    {
        lightTile = light;
        SwitchSprites();
    }

    private void SetType() 
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (!lightTile)
        {
            if (lightSprites.Length > 1 && !isPreview)
                sr.sprite = lightSprites[Random.Range(0, lightSprites.Length - 1)];
            else
                sr.sprite = lightSprites[0];

            FlipY(false);
        }
        else
        {
            if (darkSprites.Length > 1 && !isPreview)
                sr.sprite = darkSprites[Random.Range(0, darkSprites.Length - 1)];
            else
                sr.sprite = darkSprites[0];

            FlipY(true);
        }

        if (GetComponent<Rigidbody2D>() != null)
            SetGravity();
    }

    public void FlipY()
    {
        transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y);
    }

    public void FlipY(bool doFlip)
    {
        if(doFlip)
            transform.localScale = new Vector2(transform.localScale.x, -Mathf.Abs(transform.localScale.y));
        else
            transform.localScale = new Vector2(transform.localScale.x, Mathf.Abs(transform.localScale.y));
    }

    private void SetGravity() 
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if(lightTile)
            rb.gravityScale = Mathf.Abs(rb.gravityScale);
        else
            rb.gravityScale = Mathf.Abs(rb.gravityScale) * -1f;
    }

    public void SwitchSprites() 
    {
        lightTile = !lightTile;
        SetType();
    }

    public void SetSameAs(CombinedTile ct) 
    {
        lightTile = ct.lightTile;
        lightSprites = ct.lightSprites;
        darkSprites = ct.darkSprites;
        isPreview = true;

        SetType();
    }
}
