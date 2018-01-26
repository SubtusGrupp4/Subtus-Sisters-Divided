using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDecorations : MonoBehaviour {

    [SerializeField]
    private int amount = 1;
    [SerializeField]
    private Sprite[] sprites;

    [Header("Margin")]
    [SerializeField]
    private float margin = 1.5f;
    [SerializeField]
    private bool marginLeft = false;
    [SerializeField]
    private bool marginRight = false;
    [SerializeField]
    private bool marginTop = true;
    [SerializeField]
    private bool marginBot = true;

    [Header("Transform")]
    [SerializeField]
    private bool randomSize = true;
    [SerializeField]
    private float maxSize = 1.1f;
    [SerializeField]
    private float minSize = 0.9f;
    [SerializeField]
    private bool randomRotation = true;
    [SerializeField]
    private bool staticDecorations = true;

    // Use this for initialization
    void Start ()
    {
		for(int i = 0; i < amount; i++)
        {
            int rng = Random.Range(0, sprites.Length - 1);
            GameObject decoration = new GameObject(sprites[rng].name);

            decoration.AddComponent<SpriteRenderer>();
            SpriteRenderer sr = decoration.GetComponent<SpriteRenderer>();
            sr.sprite = sprites[rng];

            decoration.transform.parent = transform;
            float xl = -sr.bounds.size.x;
            float xr = sr.bounds.size.x;

            float yt = sr.bounds.size.y;
            float yb = -sr.bounds.size.y;

            if (marginLeft)
                xl = -sr.bounds.size.x / margin;
            if (marginRight)
                xr = sr.bounds.size.x / margin;

            if (marginTop)
                yt = sr.bounds.size.y / margin;
            if (marginBot)
                yb = -sr.bounds.size.y / margin;

            decoration.transform.localPosition = new Vector2(Random.Range(xl, xr), Random.Range(yt, yb));

            if(randomRotation)
                decoration.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            if (randomSize)
            {
                float scale = Random.Range(minSize, maxSize);
                decoration.transform.localScale = new Vector2(scale, scale);
            }

            if(staticDecorations)
                decoration.isStatic = true;

            this.enabled = false;
        }
	}
}
