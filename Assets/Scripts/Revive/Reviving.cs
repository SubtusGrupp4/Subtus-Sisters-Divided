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

    private bool yIsPressedC1 = false;
    private bool yIsPressedC2 = false;

    private Transform reviveTransform;

    private void Start()
    {
        pc = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (pc.player == Controller.Player1 && reviveTransform != null)
        {
            if (Input.GetAxisRaw("Y_C1") > 0.1f && !yIsPressedC1)
            {
                Revive();
                yIsPressedC1 = true;
            }
            else if (yIsPressedC1)
                yIsPressedC1 = false;
        }
        else if (reviveTransform != null)
        {
            if (Input.GetAxisRaw("Y_C2") > 0.1f && !yIsPressedC2)
            {
                Revive();
                yIsPressedC1 = true;
            }
            else if (yIsPressedC2)
                yIsPressedC2 = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Portal")
            Die();
        if (collision.transform.tag == "Revive")
            reviveTransform = collision.transform;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Revive")
            reviveTransform = null;
    }

    public void Die()
    {
        if (pc.isActive)
        {
            Camera.main.GetComponent<CameraClamp>().SetClamp(false);
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
                revivePlacer.GetComponent<RevivePlacer>().Initialize(pc.player);
                Camera.main.GetComponent<CameraController>().SetCameraState(CameraState.FollowingOne, transform);
                GameManager.instance.onePlayerDead = true;
            }
        }
    }

    private void Revive()
    {
        // Destroy ReviveSpot
        Destroy(reviveTransform.gameObject);

        Transform player;

        if (pc.player == Controller.Player1)
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
        Camera.main.GetComponent<CameraClamp>().SetClamp(false);

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

        SafepointManager.instance.SetSafepointsAsCheckpoints();
    }
}
