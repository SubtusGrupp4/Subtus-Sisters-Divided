using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideWithDialogue : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "DialogueTrigger")
        {
            Dialogue[] dialogues = collision.GetComponent<DialogueTrigger>().dialogues;
            DialogueManager.instance.FetchDialogue(dialogues);
        }
    }
}
