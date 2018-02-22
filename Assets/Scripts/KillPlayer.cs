using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{

    BasicAnimator bAnim;
    AIMovement AImove;

    // Use this for initialization
    void Start()
    {
        if (GetComponent<BasicAnimator>())
            bAnim = GetComponent<BasicAnimator>();

        if (GetComponent<AIMovement>())
            AImove = GetComponent<AIMovement>();


    }

    // Update is called once per frame
    void Update()
    {
        if (bAnim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(BasicAnimator.attackAnimName) && bAnim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !bAnim.GetComponent<Animator>().IsInTransition(0))
        {
            if (bAnim.GetComponent<Animator>().GetBool(BasicAnimator.animAttack) == false)
            {
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
            obj.transform.GetComponent<PlayerController>().Die();

            bAnim.Attack();

            AImove.Freeze(false, 0);
        }
    }
}
