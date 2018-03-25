using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Loading Screen")]
    public GameObject loadingScreen;
    //public Slider slider;
    //public Text progressText;

    [SerializeField]
    private bool fadeOnStart = true;
    public GameObject fadeScreen;
    public Image fadeImage;
    public float fadeTime = 2f;

    private void Awake()
    {
        CreateSingleton();
    }

    private void CreateSingleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        if(fadeOnStart)
            StartCoroutine(StartWait());
    }

    public void ResetLevel()
    {
        ChangeLevel(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator StartWait()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(Fade(-1, fadeTime, true));
    }

    public IEnumerator Fade(int dir, float time, bool deActivateFade)
    {
        // Fading values
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
            Time.timeScale = 1f - fade;
            yield return null;

        } while (internalTimer <= 1.0f);
        // Fading done
        if (deActivateFade)
            fadeScreen.SetActive(false);

        Time.timeScale = 1;
    }

    IEnumerator WaitForFade(float time, int level)
    {
        yield return new WaitForSeconds(time);
        ChangeLevelLoadingScreen(level);
    }

    public void ChangeLevel(int id)
    {
        StartCoroutine(Fade(1, fadeTime, false));
        StartCoroutine(WaitForFade(fadeTime, id));
    }

    private void ChangeLevelLoadingScreen(int id)
    {
        GameObject[] gos = FindObjectsOfType<GameObject>();
        foreach (GameObject go in gos)
        {
            FMODEmitter[] emitters = go.GetComponents<FMODEmitter>();
            foreach (FMODEmitter emitter in emitters)
                emitter.Stop();
        }

        StartCoroutine(LevelLoadAsynchronous(id));
    }

    IEnumerator LevelLoadAsynchronous(int id)
    {
        
        //float progress;
        AsyncOperation operation = SceneManager.LoadSceneAsync(id);

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            //progress = operation.progress / 0.9f;
            //Debug.Log("Progress: " + progress);

            //slider.value = progress;
            //progressText.text = Mathf.RoundToInt(progress * 100f) + "%";

            yield return null;
        }
        
    }
}
