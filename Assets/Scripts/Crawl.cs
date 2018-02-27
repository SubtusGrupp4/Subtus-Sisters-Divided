using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawl : MonoBehaviour
{


    private string inputString = "CrawlInput";
    private bool axisInUse = false;

    private bool crawling = false;


    PlayerController playerCont;


    // Use this for initialization
    void Start()
    {
        playerCont = GetComponent<PlayerController>();

        if (playerCont.Player == Controller.Player1)
        {
            inputString += "_C1";
        }
        else
        {
            inputString += "_C2";
        }
    }

    // Update is called once per frame
    void Update()
    {
        ToggleCrawl();
    }

    void ToggleCrawl()
    {
        if (Input.GetAxisRaw(inputString) > 0 && !axisInUse)
        {
            // BUTTON DOWN LETS START THIS SHIT
            crawling ^= true;
            
            playerCont.bodyAnim.Crawl(crawling);
            playerCont.bodyAnim.Crawl(crawling);

            // Disable ATTACK, Jump?

        }
        else if (Input.GetAxisRaw(inputString) == 0)
        {
            // axisInUse is used to make sure that you dont trigger it alot when you hold down the button.
            axisInUse = false;
        }
    }
}
