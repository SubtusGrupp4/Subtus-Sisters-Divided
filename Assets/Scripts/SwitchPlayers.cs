using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchPlayers : MonoBehaviour
{
    public static SwitchPlayers instance;

    [Header("Animation")]
    [SerializeField]
    private Image switchImage;
    [SerializeField]
    private Sprite[] spriteSequence;
    private int spriteIndex = 0;
    private bool doAnimation = false;

    [Header("Audio")]
    [SerializeField]
    private AudioClip staticClip;
    private AudioSource source;

    [Header("Dialogue")]
    [SerializeField]
    private float startTimer = 2f;
    [SerializeField]
    private float waitTime = 2f;
    private Dialogue[] dialogues;

    private bool hasSwitched = false;

    private void Start()
    {
        CreateSingleton();
        source = GetComponent<AudioSource>();
        source.clip = staticClip;
        dialogues = GetComponents<Dialogue>();
    }

    public void StartSwitch()
    {
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
        if (!hasSwitched)
        {
            yield return new WaitForSeconds(startTimer);
            switchImage.gameObject.SetActive(true);
            doAnimation = true;
            source.Play();
        }
    }

    private void FixedUpdate()
    {
        if (doAnimation && !hasSwitched)
        {
            if (spriteIndex < spriteSequence.Length)
            {
                switchImage.sprite = spriteSequence[spriteIndex];
                spriteIndex++;
            }
            else
            {
                switchImage.sprite = null;
                source.clip = null;
                switchImage.gameObject.SetActive(false);
                DoSwitch();
                doAnimation = false;
                spriteIndex = 0;
                StartCoroutine(DialogueTimer());
                hasSwitched = true;
            }
        }
    }

    private void DoSwitch()
    {
        Transform top = GameManager.instance.playerTop;
        Transform bot = GameManager.instance.playerBot;

        Vector3 temp = top.position;
        top.position = bot.position;
        bot.position = temp + new Vector3(0f, 1f);

        Transform[] players = { top, bot };

        foreach(Transform player in players)
        {
            player.GetComponent<PlayerController>().Flip();
            player.GetComponent<PullBoxes>().Flip();
            player.GetComponent<PlayerController>().PreventInput(false);
        }
    }

    private void CreateSingleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private IEnumerator DialogueTimer()
    {
        yield return new WaitForSeconds(waitTime);
        if(dialogues.Length != 0)
            DialogueManager.instance.FetchDialogue(dialogues);  // Send the dialogue to the DialogueManager when triggered
    }
}
