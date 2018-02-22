using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAudio : MonoBehaviour
{
    /*
    [FMODUnity.EventRef]
    public string eventPath;
    */

    [SerializeField]
    private AudioClip[] footsteps;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Footstep()
    {
        int rng = Random.Range(0, footsteps.Length);
        audioSource.clip = footsteps[rng];
        audioSource.Play();
        /*
        Debug.Log("Play Footsteps");
        //FMODUnity.RuntimeManager.PlayOneShot(eventPath);
        FMODUnity.RuntimeManager.PlayOneShotAttached(eventPath, gameObject);
        */
    }
}
