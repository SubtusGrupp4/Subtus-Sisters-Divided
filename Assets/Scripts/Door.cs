using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool startOpen;

    [GiveTag]
    public string[] OpenOnTouchWith = new string[] { };
    public bool autoClose;
    [SerializeField]
    private float timeUntilClose;

    [SerializeField]
    private bool useItemIndex;
    public int itemIndex;

    private bool open = false;
    private bool currentlyAtDoor;
    
    private float timer;

    [Header("Images")]
    public Sprite openImage;
    public Sprite closedImage;
    [Header("Sounds")]
    [SerializeField]
    [FMODUnity.EventRef]
    private string deActivationEvent;
    [SerializeField]
    [FMODUnity.EventRef]
    private string activationEvent;

    private SpriteRenderer sRender;
    private BoxCollider2D col;

    // Use this for initialization
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        sRender = GetComponent<SpriteRenderer>();


        if (startOpen)
            Open();
        else
            Close();
    }

    private void Update()
    {
        if (autoClose && currentlyAtDoor == false)
        {
            if (open)
                Close();
        }

        if (currentlyAtDoor)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
                currentlyAtDoor = false;
        }
    }

    public void Toggle()
    {
        if (open)
            Close();
        else
            Open();
    }

    public void Open()
    {
        if (!open)
        {
            sRender.sprite = openImage;
            col.enabled = false;
            FMODUnity.RuntimeManager.PlayOneShot(activationEvent, transform.position);
            open = true;

            currentlyAtDoor = true;
            timer = timeUntilClose;
        }
    }

    public void Close()
    {
        if (open)
        {
            sRender.sprite = closedImage;
            col.enabled = true;
            FMODUnity.RuntimeManager.PlayOneShot(deActivationEvent, transform.position);
            open = false;
        }
    }

    private void OnCollisionStay2D(Collision2D obj)
    {
        for (int i = 0; i < OpenOnTouchWith.Length; i++)
        {
            if (obj.transform.tag == OpenOnTouchWith[i])
            {
                if (obj.transform.GetComponent<ItemIndex>())
                {
                    if (useItemIndex)
                    {
                        if (obj.transform.GetComponent<ItemIndex>().Index != itemIndex)
                            break;
                    }

                    if ((obj.transform.GetComponent<ItemIndex>().DestroyOnUse))
                        Destroy(obj.transform.gameObject);
                }

                currentlyAtDoor = true;
                timer = timeUntilClose;
                Open();
            }
        }
    }
}
