using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerManager : MonoBehaviour
{
    [Header("Video")]
    [SerializeField]
    private VideoClip videoToPlay;

    [Header("Loading")]
    [SerializeField]
    private bool changeScene = false;
    [SerializeField]
    private string sceneName;

    [Header("Credits")]
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

        if (videoPlayer.canSetSkipOnDrop)
            videoPlayer.skipOnDrop = true;

        videoPlayer.source = VideoSource.VideoClip;

        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        videoPlayer.clip = videoToPlay;
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
            yield return null;

        if(loadingText != null)
            loadingText.gameObject.SetActive(false);
        videoPlayer.Play();
        if(audioSource != null)
            audioSource.Play();
        if(creditsSource != null)
            creditsSource.Play();

        while (videoPlayer.isPlaying)
            yield return null;

        if(animator != null)
            animator.SetBool("Scroll", true);

        if (changeScene)
            LevelManager.instance.ChangeLevel(sceneName);
    }
}
