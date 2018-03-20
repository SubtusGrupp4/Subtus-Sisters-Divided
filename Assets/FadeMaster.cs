using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;




public class FadeMaster : MonoBehaviour
{
    public static FadeMaster fadeMaster;

    [Header("Fading Screen")]
    public GameObject fadeScreen;
    public Image fadeImage;


    void Awake()
    {
        if (fadeMaster != null)
            GameObject.Destroy(fadeMaster);
        else
            fadeMaster = this;
    }

    public void Fade(int dir, float time, bool deActivateFade)
    {
        StartCoroutine(FadeCoroutine(dir, time, deActivateFade));
    }

    private IEnumerator FadeCoroutine(int dir, float time, bool deActivateFade)
    {
        // Fading values
        // DO WE WANT TO PAUSE THE GAME ??????????
        float internalTimer = 0;
        Time.timeScale = 1;

        Color ogC = fadeImage.color;
        Color c = fadeImage.color;
        float fade = 1 - dir;
        fade = Mathf.Clamp01(fade);
        fadeScreen.SetActive(true);
        // Fading starts
        do
        {
            internalTimer += Time.unscaledDeltaTime / time;
            c.a = fade;
            fadeImage.color = c;

            fade += Time.unscaledDeltaTime / time * dir;
            fade = Mathf.Clamp01(fade);
            Time.timeScale = 1 - fade;
            yield return null;

        } while (internalTimer <= 1.0f);
        // Fading done
        if (deActivateFade)
            fadeScreen.SetActive(false);

        Time.timeScale = 1;
    }



}
