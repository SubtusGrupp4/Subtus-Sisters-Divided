using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafepointManager : MonoBehaviour {

    // Singleton instance
	public static SafepointManager instance;

    // Used for respawning both the players. Stays activated
    public Transform currentTopSafepoint;
    public Transform currentBotSafepoint;

    // Got from Safepoints being triggered. Will deactivate if one is empty or not a matching pair.
    public Transform topSafepoint;
    public Transform botSafepoint;

    // Used as the spawning point if only one player dies but is revived
    public Transform topCheckpoint;
    public Transform botCheckpoint;

    // References to the players. Fetched from GameManager
    private Transform playerTop;
    private Transform playerBot;

    private void Awake()
	{
        CreateSingleton();
    }

	private void Start()
	{
        // Set the initial respawn points to be the starting position of the players.
        currentTopSafepoint = new GameObject("Initial Top Safepoint").transform;
        currentTopSafepoint.position = GameManager.instance.playerTop.position;

        currentBotSafepoint = new GameObject("Initial Bot Safepoint").transform;
        currentBotSafepoint.position = GameManager.instance.playerTop.position;
	}

    private void CreateSingleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void SetAsActivated(Transform safepoint) 
    {
        // Set the appropriate transform, depending on Y pos (Top/Bot)
        if (safepoint.position.y > 0f)
            topSafepoint = safepoint;
        else
            botSafepoint = safepoint;

        // If both the top and bot safepoint are assigned and the bot is a child of the top (Meaning a matching pair)
        if (topSafepoint != null && botSafepoint != null && botSafepoint == topSafepoint.GetChild(0))
        {
            // If the top safepoint has the script (if it hasn't, either it is the initial spawn point or an error)
            if(currentTopSafepoint.GetComponent<SafepointParent>() != null)
                currentTopSafepoint.GetComponent<SafepointParent>().ResetSafepoint();

            // Assign the new current safepoints
            currentTopSafepoint = topSafepoint;
            currentBotSafepoint = botSafepoint;

            // Tell the parent safepoint to make them current
            currentTopSafepoint.GetComponent<SafepointParent>().MakeCurrent();

            // Null these to enable checking more of them.
            topSafepoint = null;
            botSafepoint = null;
        }
    }
    
    public void SetAsDeactivated(Transform safepoint) 
    {
        // Null the activated transforms if they match
        if(topSafepoint == safepoint)
            topSafepoint = null;
        else if(botSafepoint == safepoint)
            botSafepoint = null;
    }
}
