using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLevelTrigger : MonoBehaviour
{
    bool topHasEntered = false;
    bool botHasEntered = false;
    [SerializeField]
    private int levelId = 0;
    [SerializeField]
    private bool bothHaveToStay = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (collision.transform == GameManager.instance.playerTop)
                topHasEntered = true;
            if (collision.transform == GameManager.instance.playerBot)
                botHasEntered = true;

            if (topHasEntered && botHasEntered)
                GameManager.instance.ChangeLevel(levelId);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (bothHaveToStay)
        {
            if (collision.tag == "Player")
            {
                if (collision.transform == GameManager.instance.playerTop)
                    topHasEntered = false;
                if (collision.transform == GameManager.instance.playerBot)
                    botHasEntered = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider2D>().size);
    }
}
