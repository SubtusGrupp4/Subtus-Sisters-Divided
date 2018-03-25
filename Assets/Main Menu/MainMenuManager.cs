using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    [Header("UI")]
    [SerializeField]
    private Transform lightUI;
    [SerializeField]
    private Transform darkUI;

    [Header("Version Parents")]
    [SerializeField]
    private Transform lightParent;
    [SerializeField]
    private Transform darkParent;

    [Header("Press Any Key notification")]
    [SerializeField]
    private Transform pressAnyKey;

    private bool useDark = false;
    private bool isActivated = false;
    private bool isDonePlaying = false;

    private float player1A = 0.4f;
    private float player2A = 0.4f;

    [Header("Radial Progress bar")]
    [SerializeField]
    private float fillRate = 1f;
    [SerializeField]
    private float emptyRate = 0.4f;

    [SerializeField]
    private RadialProgressBar radial1;
    [SerializeField]
    private RadialProgressBar radial2;

    [SerializeField]
    private RadialProgressBar radial3;
    [SerializeField]
    private RadialProgressBar radial4;

    // Fading
    private bool fadeOut = false;
    private bool playedOnce = false;

    [Header("Glitches")]
    [SerializeField]
    private Transform glitchesLight;
    [SerializeField]
    private Transform glitchesDark;
    [SerializeField]
    private Transform glitchesSwitch;
    [SerializeField]
    private Image glitchTarget;

    [SerializeField]
    private float minWait = 2f;
    [SerializeField]
    private float maxWait = 7f;

    private Sprite[] spriteSequence;
    private bool doGlitch = false;
    int spriteIndex = 0;

    [Header("Switching")]
    [SerializeField]
    private float minWaitSwitch = 10f;
    [SerializeField]
    private float maxWaitSwitch = 20f;
    private bool isSwitching = false;
    private int spriteSwitchIndex = 0;

    [Header("Audio")]
    [SerializeField]
    private AudioClip[] staticClips;
    private AudioSource[] audioSources;

    [SerializeField]
    [Range(0f, 1f)]
    private float topMaxVolume = 0.6f;
    [SerializeField]
    [Range(0f, 1f)]
    private float botMaxVolume = 0.6f;

    [Header("Loading")]
    [SerializeField]
    private string sceneName;

    void Start()
    {
        StartCoroutine(KeyTimer());     // Coroutine that displays the "Press any key" prompt
        glitchTarget.enabled = false;   // Do not show the glitch sprite
        audioSources = GetComponents<AudioSource>();
    }

    void Update()
    {
        if (Input.anyKey && !isActivated)
            StartAnimation();

        if (isDonePlaying)
        {
            SwitchLightDark();  // Chooses what version to display

            darkUI.transform.position = lightUI.transform.position; // Keep both versions of the UI on the same position

            ProgressBar();  // Progress bar input and processing
        }
    }

    private void FixedUpdate()
    {
        if (doGlitch && !isSwitching)
            GlitchAnimation();  // Cycle through glitch sprites

        if (isSwitching)
            SwitchAnimation();  // Cycle through switching sprites
    }

    private void StartAnimation()
    {
        lightUI.gameObject.SetActive(true);
        lightUI.GetComponentInChildren<PlayableDirector>().Play();  // Start playing all of their animations
        StartCoroutine(DonePlaying((float)lightUI.transform.GetChild(0).GetComponent<PlayableDirector>().duration));    // Set a timer the length of the animations
        pressAnyKey.gameObject.SetActive(false);    // Hide the "Press any key" prompt
        isActivated = true;
    }

    private void SwitchLightDark()
    {
        if (isActivated && useDark)
        {
            darkParent.gameObject.SetActive(true);
            lightParent.gameObject.SetActive(false);

            audioSources[1].volume = 0f;
            audioSources[2].volume = botMaxVolume;
        }
        else if (isActivated)
        {
            darkParent.gameObject.SetActive(false);
            lightParent.gameObject.SetActive(true);

            audioSources[1].volume = topMaxVolume;
            audioSources[2].volume = 0f;
        }
    }

    private void ProgressBar()
    {
        // Holding A to fill the progress bars
        if (Input.GetAxis("Jump_C1") > 0f)
            player1A += Time.deltaTime * fillRate;
        else
            player1A -= Time.deltaTime * emptyRate; // Empty if not holding A

        if (Input.GetAxis("Jump_C2") > 0f)
            player2A += Time.deltaTime * fillRate;
        else
            player2A -= Time.deltaTime * emptyRate;

        // Clamping to appropriate numbers
        player1A = Mathf.Clamp(player1A, 0.4f, 1f);
        player2A = Mathf.Clamp(player2A, 0.4f, 1f);

        // Applying the amount to each radial progress bar
        radial1.SetAmount(player1A);
        radial2.SetAmount(player2A);
        // 3 and 4 are the dark versions
        radial3.SetAmount(player1A);
        radial4.SetAmount(player2A);

        // If both are filled, start fading out
        if (player1A >= 0.99f && player2A >= 0.99f)
            fadeOut = true;

        if (fadeOut)
        {
            // Make sure the progress bars stay full
            radial1.SetAmount(1f);
            radial2.SetAmount(1f);
            radial3.SetAmount(1f);
            radial4.SetAmount(1f);

            if(!playedOnce)
            {
                playedOnce = true;
                LevelManager.instance.ChangeLevel(sceneName);
            }
        }
    }

    private void GlitchAnimation()
    {
        glitchTarget.enabled = true;    // Display the image
        int rng = Random.Range(0, staticClips.Length);
        audioSources[0].PlayOneShot(staticClips[rng]);

        if (spriteIndex < spriteSequence.Length)    // If there are still sprites to show
        {
            glitchTarget.sprite = spriteSequence[spriteIndex];  // Switch to the next one
            spriteIndex++;
        }
        else    // Stop the animation and reset
        {
            doGlitch = false;
            spriteIndex = 0;
            StartCoroutine(GlitchTimer());  // Start a timer for when next glitch should occur
            glitchTarget.enabled = false;
        }
    }

    private void SwitchAnimation()
    {
        int rng = Random.Range(0, staticClips.Length);
        audioSources[0].PlayOneShot(staticClips[rng]);
        SpriteArray[] spriteArrays = glitchesSwitch.GetComponents<SpriteArray>();   // Fetch the sprite array

        int index = 0;
        if (useDark)
            index = 1;

        spriteSequence = spriteArrays[index].frames;    // Get either the switch to dark or light, depending on current version

        glitchTarget.enabled = true;
        if (spriteSwitchIndex < spriteSequence.Length)
        {
            glitchTarget.sprite = spriteSequence[spriteSwitchIndex];
            spriteSwitchIndex++;
        }
        else
        {
            isSwitching = false;
            spriteSwitchIndex = 0;
            StartCoroutine(SwitchTimer());
            useDark = !useDark;
            if (!doGlitch)
                glitchTarget.enabled = false;
        }
    }

    IEnumerator GlitchTimer()
    {
        float time = Random.Range(minWait, maxWait);
        yield return new WaitForSeconds(time);
        SpriteArray[] spriteArrays = glitchesLight.GetComponents<SpriteArray>();

        if (useDark)
            spriteArrays = glitchesDark.GetComponents<SpriteArray>();

        int index = Random.Range(0, spriteArrays.Length - 1);
        spriteSequence = spriteArrays[index].frames;
        doGlitch = true;
    }

    IEnumerator SwitchTimer()
    {
        float time = Random.Range(minWaitSwitch, maxWaitSwitch);
        yield return new WaitForSeconds(time);
        isSwitching = true;
    }

    IEnumerator KeyTimer()
    {
        yield return new WaitForSeconds(5f);
        if (!isActivated)
            pressAnyKey.gameObject.SetActive(true);

    }

    IEnumerator DonePlaying(float animationLength)
    {
        yield return new WaitForSeconds(animationLength);
        isDonePlaying = true;
        StartCoroutine(GlitchTimer());
        StartCoroutine(SwitchTimer());
    }
}
