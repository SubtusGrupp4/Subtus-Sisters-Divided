using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{

    BasicAnimator bAnim;
    AIMovement AImove;
    private bool attacking;

    private EnemyAudio enemyAudio;

    // Use this for initialization
    void Start()
    {
        if (GetComponent<BasicAnimator>())
            bAnim = GetComponent<BasicAnimator>();

        if (GetComponent<AIMovement>())
            AImove = GetComponent<AIMovement>();

        enemyAudio = GetComponent<EnemyAudio>();
    }

    // Update is called once per frame
    void Update()
    {
        {
            if (bAnim.GetComponent<Animator>().GetBool(BasicAnimator.animAttack) == false && attacking)
            {
                attacking = false;
                Debug.Log("Animation Doneish");
                AImove.UnFreeze();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //   Kill(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Kill(collision.gameObject);
    }

    void Kill(GameObject obj)
    {
        // If collide with player, kill the player
        if (obj.transform.tag == "Player")
        {
            obj.transform.GetComponent<Reviving>().Die();
           // obj.transform.GetComponent<Reviving>().die
            bAnim.Attack();
            attacking = true;
            Debug.Log("KILL PLAYER");
            AImove.Freeze(false, 0);
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;


        }
    }
}
