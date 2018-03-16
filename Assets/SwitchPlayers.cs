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
    private float waitTime = 2f;
    private Dialogue[] dialogues;

    private void Start()
    {
        CreateSingleton();
        source = GetComponent<AudioSource>();
        source.clip = staticClip;
        dialogues = GetComponents<Dialogue>();
    }

    public void StartSwitch()
    {
        switchImage.gameObject.SetActive(true); 
        doAnimation = true;
        source.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            StartSwitch();
    }

    private void FixedUpdate()
    {
        if (doAnimation)
        {
            if (spriteIndex < spriteSequence.Length)
            {
                switchImage.sprite = spriteSequence[spriteIndex];
                spriteIndex++;
            }
            else
            {
                switchImage.gameObject.SetActive(false);
                DoSwitch();
                doAnimation = false;
                spriteIndex = 0;
                StartCoroutine(DialogueTimer());
            }
        }
    }

    private void DoSwitch()
    {
        Transform top = GameManager.instance.playerTop;
        Transform bot = GameManager.instance.playerBot;

        Transform[] players = { top, bot };

        foreach(Transform player in players)
        {
            player.GetComponent<PlayerController>().Flip();
            player.GetComponent<PullBoxes>().Flip();
        }
        Vector3 temp = Vector3.zero;
        temp = top.position;
        top.position = bot.position;
        bot.position = temp + new Vector3(0f, 1f);
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
        DialogueManager.instance.FetchDialogue(dialogues);  // Send the dialogue to the DialogueManager when triggered
    }
}
