using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    [Header("UI")]
	[SerializeField]
	private Transform lightUI;
	[SerializeField]
	private Transform darkUI;
    [SerializeField]
    private Transform uiParent;

    [Header("Version Parents")]
	[SerializeField]
	private Transform light;
	[SerializeField]
	private Transform dark;

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

    [Header("Fading")]
    [SerializeField]
    private Transform fadeOutImage;
    private bool fadeOut = false;
    [SerializeField]
    private float fadeSpeed = 1f;

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

    // Use this for initialization
    void Start () 
	{
		StartCoroutine(KeyTimer());
        glitchTarget.enabled = false;
    }
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.anyKey && !isActivated) 
		{
			lightUI.gameObject.SetActive(true);
			lightUI.GetComponentInChildren<PlayableDirector>().Play();
			StartCoroutine(DonePlaying((float)lightUI.transform.GetChild(0).GetComponent<PlayableDirector>().duration));
			pressAnyKey.gameObject.SetActive(false);
			isActivated = true;
		}

		if(isDonePlaying) 
		{
            if (Input.GetKeyDown(KeyCode.P) && isActivated)
                useDark = !useDark;

            if (isActivated && useDark) 
			{
				dark.gameObject.SetActive(true);
				light.gameObject.SetActive(false);
			}
			else if(isActivated) 
			{
				dark.gameObject.SetActive(false);
				light.gameObject.SetActive(true);
			}

            darkUI.transform.position = lightUI.transform.position;

            // Holding A to fill the progress bars
            if (Input.GetAxis("Jump_C1") > 0f)
                player1A += Time.deltaTime * fillRate;
            else
                player1A -= Time.deltaTime * emptyRate;

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
            radial3.SetAmount(player1A);
            radial4.SetAmount(player2A);

            // If both are filled, start fading out
            if (player1A >= 0.99f && player2A >= 0.99f)
                fadeOut = true;

            if(fadeOut)
            {
                // Make sure the progress bars stay full
                radial1.SetAmount(1f);
                radial2.SetAmount(1f);
                radial3.SetAmount(1f);
                radial4.SetAmount(1f);

                // Fade in a black image
                if (fadeOutImage.GetComponent<CanvasGroup>().alpha < 1f)
                    fadeOutImage.GetComponent<CanvasGroup>().alpha += Time.deltaTime * fadeSpeed;
                else
                    SceneManager.LoadScene("Main"); // When finished, load the main scene
            }

            /*
            if(Input.GetAxis("Jump_C1") > 0f)
                player1A -= Time.deltaTime / 3f;
            else
                player1A += Time.deltaTime / 9f;

            if(Input.GetAxis("Jump_C2") > 0f)
                player2A -= Time.deltaTime / 3f;
            else
                player2A += Time.deltaTime / 9f;

            player1A = Mathf.Clamp(player1A, 0.025f, 0.5f);
            player2A = Mathf.Clamp(player2A, 0.025f, 0.5f);

            radial1.SetAmount(player1A);
            radial2.SetAmount(player2A);
            */

            if(doGlitch && !isSwitching)
            {
                glitchTarget.enabled = true;
                if (spriteIndex < spriteSequence.Length)
                {
                    glitchTarget.sprite = spriteSequence[spriteIndex];
                    spriteIndex++;
                }
                else
                {
                    doGlitch = false;
                    spriteIndex = 0;
                    StartCoroutine(GlitchTimer());
                    glitchTarget.enabled = false;
                }
            }

            if(isSwitching)
            {
                SpriteArray[] spriteArrays = glitchesSwitch.GetComponents<SpriteArray>();

                int index = 0;
                if (useDark)
                    index = 1;

                spriteSequence = spriteArrays[index].frames;

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
                    if(!doGlitch)
                        glitchTarget.enabled = false;
                }
            }
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
		if(!isActivated)
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
