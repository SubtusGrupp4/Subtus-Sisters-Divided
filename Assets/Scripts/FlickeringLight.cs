using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [Header("Interval")]
    [SerializeField]
    private float minInterval;
    [SerializeField]
    private float maxInterval;

    [Header("Size")]
    [SerializeField]
    private float minSize;
    [SerializeField]
    private float maxSize;
    private float prevSize;
    private float newSize = 1f;

    [Header("Opacity")]
    [SerializeField]
    private float minOpacity;
    [SerializeField]
    private float maxOpacity;
    private float prevOpacity;
    private float newOpacity = 1f;

    private float lerpTimer = 1f;
    private float lerpInterval;

    [Header("Switching sprites")]
    [SerializeField]
    private bool useSwitchingSprites;
    [SerializeField]
    private Sprite[] sprites;

    private SpriteRenderer sr;

    // Use this for initialization
    void Start () {
        sr = GetComponent<SpriteRenderer>();
        if (useSwitchingSprites)
            if (transform.position.y < 0f)
                sr.sprite = sprites[1];
        StartCoroutine(FlickerTimer(Random.Range(minInterval, maxInterval)));
    }

    private void Update()
    {
        if (lerpTimer < 1f) {
            lerpTimer += Time.deltaTime / lerpInterval;
            float size = Mathf.Lerp(prevSize, newSize, lerpTimer);
            transform.localScale = new Vector2(size, size);

            float opacity = Mathf.Lerp(prevOpacity, newOpacity, lerpTimer);
            sr.color = new Color(1f, 1f, 1f, opacity);
        }
    }

    private IEnumerator FlickerTimer(float interval)
    {
        prevSize = newSize;
        prevOpacity = newOpacity;
        lerpTimer = 0f;
        lerpInterval = interval;
        newSize = Random.Range(minSize, maxSize);
        newOpacity = Random.Range(minOpacity, maxOpacity);

        yield return new WaitForSeconds(interval);
        StartCoroutine(FlickerTimer(Random.Range(minInterval, maxInterval)));
    }
}
