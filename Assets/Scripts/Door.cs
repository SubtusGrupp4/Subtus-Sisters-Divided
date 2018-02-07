using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool startOpen;
    private bool open = false;

    [Header("Images")]
    public Sprite openImage;
    public Sprite closedImage;
    [Header("Sounds")]
    public AudioClip openDoorSound;
    public AudioClip closeDoorSound;

    private SpriteRenderer sRender;
    private BoxCollider2D collider;
    private AudioSource myAudio;

    // Use this for initialization
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
        collider = GetComponent<BoxCollider2D>();
        sRender = GetComponent<SpriteRenderer>();

        if (startOpen)
            Open();
        else
            Close();
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
        sRender.sprite = openImage;
        collider.enabled = false;
        myAudio.PlayOneShot(openDoorSound);
        open = true;
    }

    public void Close()
    {
        sRender.sprite = closedImage;
        collider.enabled = true;
        myAudio.PlayOneShot(closeDoorSound);
        open = false;
    }
}
