using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Placed on objects that should trigger dialogue triggers
public class TriggerDialogue : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "DialogueTrigger")
            collision.GetComponent<DialogueTrigger>().TriggerDialogue();
    }
}
