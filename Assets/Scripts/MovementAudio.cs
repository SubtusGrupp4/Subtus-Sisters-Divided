using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAudio : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string eventPath;

    public void Footstep()
    {
        Debug.Log("Play Footsteps");
        //FMODUnity.RuntimeManager.PlayOneShot(eventPath);
        FMODUnity.RuntimeManager.PlayOneShotAttached(eventPath, gameObject);
    }
}
