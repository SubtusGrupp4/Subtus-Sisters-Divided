using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This might need a better name
public class Reviving : MonoBehaviour
{

    private PlayerController pc;

    [SerializeField]
    private GameObject revivePlacerPrefab;
    private GameObject revivePlacer;
    [SerializeField]
    private GameObject dyingAnimationGO;

    [HideInInspector]
    public GameObject deathAnimationObject;

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
        {
            reviveTransform = collision.transform;
            reviveTransform.GetComponent<ReviveSpot>().SetParticleEmission(1f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Revive")
        {
            reviveTransform.GetComponent<ReviveSpot>().SetParticleEmission(0f);
            reviveTransform = null;
        }
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

                deathAnimationObject = Instantiate(dyingAnimationGO, transform.position, transform.rotation);
                deathAnimationObject.transform.localScale = transform.localScale;

                // Destroy(deathAnimationObject, deathAnimationObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

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
        reviveTransform.GetComponent<ReviveSpot>().DestroySelf();

        Transform player;

        if (pc.player == Controller.Player1)
            player = GameManager.instance.playerBot;
        else
        {
            player = GameManager.instance.playerTop;
            player.GetComponent<PlayerController>().crawling = false;
        }


        SafepointManager.instance.PlacePlayerOnCheckpoint(player);
        player.gameObject.SetActive(true);
        player.GetComponent<PlayerController>().isActive = true;
        // Destroy animation
        Destroy(player.GetComponent<Reviving>().deathAnimationObject);

        Camera.main.GetComponent<CameraController>().SetCameraState(CameraState.FollowingBoth);

        GameManager.instance.onePlayerDead = false;

        SafepointManager.instance.DecreaseTimer();
    }

    public void BothDead()
    {
        Camera.main.GetComponent<CameraClamp>().SetClamp(false);

        // Delete all revive objects, if there are any
        GameObject[] revives = GameObject.FindGameObjectsWithTag("Revive");
        foreach (GameObject revive in revives)
            Destroy(revive);

        GameManager.instance.playerTop.GetComponent<PlayerController>().crawling = false;
        // Set the camera to transition towards the safepoints
        SafepointManager.instance.RespawnTransition();
        SafepointManager.instance.DecreaseTimer();


        Transform player;

        GameManager.instance.playerTop.gameObject.SetActive(false);
        GameManager.instance.playerBot.gameObject.SetActive(false);

        if (pc.player == Controller.Player1)
            player = GameManager.instance.playerBot;
        else
            player = GameManager.instance.playerTop;

        Destroy(player.GetComponent<Reviving>().deathAnimationObject);

        Destroy(deathAnimationObject);

        // Destroy both animations ?
    }
}
