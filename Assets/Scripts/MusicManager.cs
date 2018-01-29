using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public static MusicManager instance;

    // 0 = ???
    // 1 = ???
    // 2 = ???
    // 3 = ???
    // 4 = ???
    // 5 = ???

    [SerializeField]
    private AudioSource[] musicPlayers;

    private void Awake()
    {
        CreateSingleton();
    }

    // Use this for initialization
    void Start () {
        musicPlayers = GetComponents<AudioSource>();
	}

    private void CreateSingleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// Play all musicplayers, overload for volume
    /// </summary>
    public void PlayAll()
    {
        foreach(AudioSource musicPlayer in musicPlayers)
            musicPlayer.Play();
    }

    public void PlayAll(float volume)
    {
        foreach (AudioSource musicPlayer in musicPlayers)
            musicPlayer.PlayOneShot(musicPlayer.clip, volume);
    }

    /// <summary>
    /// Change the volume of all musicplayers, or with index
    /// </summary>
    /// <param name="volume"></param>
    public void SetVolume(float volume)
    {
        foreach (AudioSource musicPlayer in musicPlayers)
            musicPlayer.volume = volume;
    }

    public void SetVolume(float volume, int index)
    {
        musicPlayers[index].volume = volume;
    }

    /// <summary>
    /// Set looping of all musicplayers
    /// </summary>
    /// <param name="loop"></param>
    public void SetLoop(bool loop)
    {
        foreach (AudioSource musicPlayer in musicPlayers)
            musicPlayer.loop = loop;
    }


    /// <summary>
    /// Change all audioclips, or use an index
    /// </summary>
    /// <param name="clips"></param>
    public void ChangeClip(AudioClip[] clips)
    {
        for(int i = 0; i < musicPlayers.Length; i++)
        {
            if(clips[i] != null)
                musicPlayers[i].clip = clips[i];
        }
    }

    public void ChangeClip(AudioClip clip, int index)
    {
        musicPlayers[index].clip = clip;
    }
}
