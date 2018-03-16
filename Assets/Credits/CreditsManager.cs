using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    [Header("Fading")]
    [SerializeField]
    private float videoLength = 26f;
    [SerializeField]
    private CanvasGroup group;
    [SerializeField]
    private float fadeSpeed = 1f;
    private bool doFade = false;


    [Header("Scrolling")]
    [SerializeField]
    private float scrollWait = 2f;
    [SerializeField]
    private RectTransform logo;
    [SerializeField]
    private float finalY = 100f;
    [SerializeField]
    private float scrollSpeed = 1f;
    private bool doScroll = false;

    // Use this for initialization
    void Start ()
    {
        StartCoroutine(VideoTimer());
        group.alpha = 0f;
	}

    private IEnumerator VideoTimer()
    {
        yield return new WaitForSeconds(videoLength);
        doFade = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (doFade)
            FadeIn();

        if (doScroll)
            Scroll();
	}

    private void FadeIn()
    {
        if (group.alpha < 1f)
            group.alpha += Time.deltaTime * fadeSpeed;
        else
        {
            group.alpha = 1f;
            doFade = false;
            StartCoroutine(ScrollTimer());
        }
    }

    private IEnumerator ScrollTimer()
    {
        yield return new WaitForSeconds(scrollWait);
        doScroll = true;
    }

    private void Scroll()
    {
        if (logo.position.y < finalY)
            logo.position = new Vector2(logo.position.x, logo.position.y + Time.deltaTime * scrollSpeed);
        else
            EndScroll();
    }

    private void EndScroll()
    {
        SceneManager.LoadScene(0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(new Vector2(logo.position.x, finalY), 10f);
    }
}
