using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    [SerializeField]
    [Tooltip("Prevent the script from overriding the tag with \"DialogueTrigger\"")]
    private bool customTag = false;
    [HideInInspector]
    public Dialogue[] dialogues;
    [SerializeField]
    private bool onlyTriggerOnce = false;
    private bool isTriggered;

    private void Start()
    {
        if(!customTag)
            transform.tag = "DialogueTrigger";  // Default tag

        dialogues = GetComponents<Dialogue>();  // Fetch the dialogue scripts
    }

    public void TriggerDialogue()
    {
        if(!onlyTriggerOnce || !isTriggered)
            DialogueManager.instance.FetchDialogue(dialogues);  // Send the dialogue to the DialogueManager when triggered

        if (onlyTriggerOnce)
            isTriggered = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.44f, 0.18f, 0.21f);
        if(GetComponent<BoxCollider2D>() != null)
            Gizmos.DrawWireCube(transform.position + new Vector3(GetComponent<BoxCollider2D>().offset.x, GetComponent<BoxCollider2D>().offset.y), GetComponent<BoxCollider2D>().size);
    }
}
