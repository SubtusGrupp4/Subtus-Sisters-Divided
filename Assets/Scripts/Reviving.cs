using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This might need a better name
public class Reviving : MonoBehaviour {

    private PlayerController pc;

    [SerializeField]
    private GameObject revivePlacerPrefab;
    private GameObject revivePlacer;
    [SerializeField]
    private GameObject dyingAnimationGO;

    private void Start()
    {
        pc = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Portal")
        {
            Die();
        }
        else if (collision.transform.tag == "Revive")
        {
            Revive();
            Destroy(collision.gameObject);
        }
    }

    public void Die()
    {
        if (pc.isActive)
        {
            pc.isActive = false;
            gameObject.SetActive(false);

            // If any of the players is dead and the last dies, call BothDead() instead.
            if (GameManager.instance.onePlayerDead)
            {
                BothDead();
            }
            else
            {
                GameObject dead;
                dead = Instantiate(dyingAnimationGO, transform.position, transform.rotation);
                dead.transform.localScale = transform.localScale;

                Destroy(dead, dead.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

                Vector2 spawnPos = new Vector2(pc.lastSafe.x, 0f);
                revivePlacer = Instantiate(revivePlacerPrefab, spawnPos, Quaternion.identity);
                revivePlacer.GetComponent<RevivePlacer>().Initialize(pc.Player);
                Camera.main.GetComponent<CameraController>().SetCameraState(CameraState.FollowingOne, transform);
                GameManager.instance.onePlayerDead = true;
            }
        }
    }

    private void Revive()
    {
        Transform player;

        if (pc.Player == Controller.Player1)
            player = GameManager.instance.playerBot;
        else
            player = GameManager.instance.playerTop;

        SafepointManager.instance.PlacePlayerOnCheckpoint(player);
        player.gameObject.SetActive(true);
        player.GetComponent<PlayerController>().isActive = true;

        Camera.main.GetComponent<CameraController>().SetCameraState(CameraState.FollowingBoth);

        GameManager.instance.onePlayerDead = false;
    }

    private void BothDead()
    {
        // Delete all revive objects, if there are any
        GameObject[] revives = GameObject.FindGameObjectsWithTag("Revive");
        foreach (GameObject revive in revives)
            Destroy(revive);

        // Get the player transforms
        Transform playerTop = GameManager.instance.playerTop;
        Transform playerBot = GameManager.instance.playerBot;

        // Activate them
        playerTop.gameObject.SetActive(true);
        playerBot.gameObject.SetActive(true);

        // Place them on the current safepoints
        playerTop.transform.position = SafepointManager.instance.currentTopSafepoint.position;
        playerBot.transform.position = SafepointManager.instance.currentBotSafepoint.position;

        // Set the camera to follow both
        Camera.main.GetComponent<CameraController>().SetCameraState(CameraState.FollowingBoth, transform);

        GameManager.instance.onePlayerDead = false;

        playerTop.GetComponent<PlayerController>().isActive = true;
        playerBot.GetComponent<PlayerController>().isActive = true;
    }
}
