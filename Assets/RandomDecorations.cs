using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDecorations : MonoBehaviour {

    [SerializeField]
    private int amount = 1;
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private float margin = 1.5f;
    [SerializeField]
    private float maxSize = 1.1f;
    [SerializeField]
    private float minSize = 0.9f;

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
            float x = sr.bounds.size.x / margin;
            float y = sr.bounds.size.y / margin;
            decoration.transform.localPosition = new Vector2(Random.Range(-x, x), Random.Range(-y, y));
            decoration.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            float scale = Random.Range(minSize, maxSize);
            decoration.transform.localScale = new Vector2(scale, scale);

            decoration.isStatic = true;
        }
	}
}
