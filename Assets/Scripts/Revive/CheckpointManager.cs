using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour {

	public static CheckpointManager instance;

    public Transform topCheckpoint;
    public Transform botCheckpoint;

    public Transform topSafepoint;
    public Transform botSafepoint;

    private Transform PlayerTop;
    private Transform PlayerBot;

    private void Awake()
	{
        CreateSingleton();
    }

	private void Start()
	{
        topCheckpoint = GameManager.instance.playerTop;
        botCheckpoint = GameManager.instance.playerBot;
	}

    private void CreateSingleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

	public void Checkpoint(Transform top, Transform bot) 
	{
        topCheckpoint = top;
        botCheckpoint = bot;
    }

    public void Safepoint(Transform top, Transform bot)
    {
        topSafepoint = top;
        botSafepoint = bot;
    }
}
