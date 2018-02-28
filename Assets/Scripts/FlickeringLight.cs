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

    [Header("Intensity")]
    [SerializeField]
    [Range(0f, 1f)]
    private float minIntensity;
    [SerializeField]
    [Range(0f, 1f)]
    private float maxIntensity;
    private float prevIntensity;
    private float newIntensity = 1f;

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
            float size = Mathf.Lerp(prevIntensity, newIntensity, lerpTimer);
            transform.localScale = new Vector2(size, size);

            float opacity = Mathf.Lerp(prevIntensity, newIntensity, lerpTimer);
            sr.color = new Color(1f, 1f, 1f, opacity);
        }
    }

    private IEnumerator FlickerTimer(float interval)
    {
        prevIntensity = newIntensity;
        lerpTimer = 0f;
        lerpInterval = interval;
        newIntensity = Random.Range(minIntensity, maxIntensity);

        yield return new WaitForSeconds(interval);
        StartCoroutine(FlickerTimer(Random.Range(minInterval, maxInterval)));
    }
}
