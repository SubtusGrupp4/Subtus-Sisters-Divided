using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroundType
{
    Grass, Gravel, Wood,
    GrassGross
}

public class MovementAudio : MonoBehaviour
{
    public GroundType groundType = GroundType.Grass;

    [FMODUnity.EventRef]
    public string grassEvent;
    [FMODUnity.EventRef]
    public string gravelEvent;
    [FMODUnity.EventRef]
    public string woodEvent;
    [FMODUnity.EventRef]
    public string grassGrossEvent;

    public FMOD.Studio.EventInstance eventInstance;
    public FMOD.Studio.ParameterInstance parameterInstance;
    [SerializeField]
    FMODUnity.StudioEventEmitter emitter;

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

        switch(groundType)
        {
            case GroundType.Grass:
                Debug.Log("Grass");
                FMODUnity.RuntimeManager.PlayOneShot(grassEvent, transform.position);
                break;
            case GroundType.Gravel:
                Debug.Log("Gravel");
                FMODUnity.RuntimeManager.PlayOneShot(gravelEvent, transform.position);
                break;
            case GroundType.Wood:
                Debug.Log("Wood");
                FMODUnity.RuntimeManager.PlayOneShot(woodEvent, transform.position);
                break;
            case GroundType.GrassGross:
                Debug.Log("Grass Gross");
                FMODUnity.RuntimeManager.PlayOneShot(grassGrossEvent, transform.position);
                break;
        }

        //eventInstance = FMODUnity.RuntimeManager.CreateInstance(eventName);
        //eventInstance.getParameter("Grass", out parameterInstance);
        //eventInstance.start();
        //parameterInstance.setValue(1f);

        //emitter.Play();
        //emitter.SetParameter("Grass", 1f);
    }
}
