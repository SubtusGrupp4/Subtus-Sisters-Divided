using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAudio : MonoBehaviour
{
    [SerializeField]
    private string eventPath;

    public void Footstep()
    {
        Debug.Log("Play Footsteps");
        //FMODUnity.RuntimeManager.PlayOneShot(eventPath);
        FMODUnity.RuntimeManager.PlayOneShotAttached(eventPath, gameObject);
    }
}
