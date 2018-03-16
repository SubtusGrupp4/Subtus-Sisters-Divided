using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchPlayers : MonoBehaviour {

    [SerializeField]
    private Image switchImage;
    [SerializeField]
    private Sprite[] spriteSequence;
    private int spriteIndex = 0;
    private bool doAnimation = false;

    public void StartSwitch()
    {
        switchImage.gameObject.SetActive(true); 
        doAnimation = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            DoSwitch();
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
}
