using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerManager : MonoBehaviour
{
    [SerializeField]
    private VideoClip videoToPlay;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Transform loadingText;
    [SerializeField]
    private AudioSource creditsSource;

    private VideoPlayer videoPlayer;
    private VideoSource videoSource;
    private AudioSource audioSource;

    void Start()
    {
        Application.runInBackground = true;
        videoPlayer = GetComponent<VideoPlayer>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(playVideo());
    }

    IEnumerator playVideo()
    {
        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;

        videoPlayer.source = VideoSource.VideoClip;

        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        videoPlayer.clip = videoToPlay;
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
            yield return null;

        loadingText.gameObject.SetActive(false);
        videoPlayer.Play();
        audioSource.Play();
        creditsSource.Play();

        while (videoPlayer.isPlaying)
            yield return null;

        animator.SetBool("Scroll", true);
    }
}
