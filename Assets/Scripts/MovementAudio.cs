using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAudio : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string eventName;
    public FMOD.Studio.EventInstance eventInstance;
    public FMOD.Studio.ParameterInstance parameterInstance;

    //[SerializeField]
    //private AudioClip[] footsteps;
    //private AudioSource audioSource;

    private void Start()
    {
        //audioSource = GetComponent<AudioSource>();
    }

    public void Footstep()
    {
        /* Unity solution
        int rng = Random.Range(0, footsteps.Length);
        audioSource.clip = footsteps[rng];
        audioSource.Play();
        */

        //Debug.Log("Play Footsteps");
        //FMODUnity.RuntimeManager.PlayOneShot(eventPath);
        //FMODUnity.RuntimeManager.PlayOneShot(eventPath);
        //Helloooooo

        eventInstance = FMODUnity.RuntimeManager.CreateInstance(eventName);
        eventInstance.getParameter("Grass", out parameterInstance);
        eventInstance.start();
        parameterInstance.setValue(1f);
    }
}
