using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimator : MonoBehaviour
{
    public const string animRun = "IsRunning";
    public const string animAttack = "IsAttacking";
    public const string animFall = "IsFalling";
    public const string animWalk = "IsWalking";
    public const string animJump = "IsJumping";
    public const string animLand = "IsLanding";
    public const string animTurning = "IsTurning";

    private bool walkRunBool = false;

    public const string attackAnimName = "Attack";

    private bool attacking;
    private bool jumping;
    private bool landing;

    public GameObject animGameObject;
    [Range(-1, 1)]
    public float facing;

    protected float x;
    protected float savedX;
    protected float y;
    Animator anim;
    private new Rigidbody2D rigidbody2D;

    protected virtual void Start()
    {
        savedX = facing;
        savedX = savedX > 0 ? 1 : -1;
        anim = animGameObject.GetComponent<Animator>();
        anim.SetBool(animRun, false);//Walking animation is deactivated
        anim.SetBool(animAttack, false);//Attacking animation is deactivated
        anim.SetBool(animFall, false);//Falling animation is deactivated
        anim.SetBool(animWalk, true);

        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        // toggle of attack when attack is finished
        if (anim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(attackAnimName) && anim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.GetComponent<Animator>().IsInTransition(0) && attacking == true)
        {
            anim.SetBool(animAttack, false);
            attacking = false;
        }
        if (anim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(animJump) && anim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.1 && !anim.GetComponent<Animator>().IsInTransition(0) && jumping == true)
        {
            anim.SetBool(animJump, false);
            jumping = false;
        }
        if (anim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(animLand) && anim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.GetComponent<Animator>().IsInTransition(0) && landing == true)
        {
            anim.SetBool(animLand, false);
            landing = false;
        }

    }
    public virtual void Turning(bool state)
    {
        anim.SetBool(animTurning, state);
    }

    public virtual void Attack()
    {
        anim.SetBool(animAttack, true);
        attacking = true;
    }

    public virtual void Jump()
    {
        anim.SetBool(animJump, true);
        jumping = true;
    }

    public virtual void Land()
    {
        anim.SetBool(animLand, true);
        landing = true;
    }
    public virtual void Flip()
    {
        // FLIP ?
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

    }

    public virtual void ToggleWalk(bool state)
    {
      //  walkRunBool ^= true;


        anim.SetBool(animWalk, state);
    }

    public virtual void Falling(bool state)
    {
        anim.SetBool(animFall, state);
    }

    public virtual void Walking(Vector2 direction, bool flip)
    {
        SetAnimation(direction, flip);
    }
    protected virtual void SetAnimation(Vector2 dir, bool flip)
    {
        x = dir.x;
        if (x != 0)
        {
            x = dir.x > 0 ? 1 : -1;

            anim.SetBool(animRun, true);
        }
        else
            anim.SetBool(animRun, false);//Walking animation is activated

        if (flip)
            if (x != savedX && x != 0)
            {
                savedX = x;
                Flip();
            }

    }
}