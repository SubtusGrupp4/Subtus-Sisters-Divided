using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour {

    public bool rightAligned = false;
    public string npcName;
    public Sprite npcSprite;

    [TextArea(3, 10)]
    public string[] sentences;
    public AudioClip[] audioClips;
    public float waitTime = 0f;
}
