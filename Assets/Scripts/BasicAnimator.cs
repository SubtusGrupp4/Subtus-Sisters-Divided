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
    public const string animCrawl = "IsCrawling";
    public const string animPull = "IsPulling";
    public const string animPush = "IsPushing";
 

    private bool walkRunBool = false;

    public const string attackAnimName = "Attack";
    public const string crawlEndName = "EndCrawl";

    private bool attacking;
    private bool jumping;
    private bool landing;
    private bool crawling;

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
        if (anim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(animLand) && anim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !anim.GetComponent<Animator>().IsInTransition(0))
        {       
            landing = false;
            anim.SetBool(animLand, false);         
        }
        else if(!anim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(animLand) && !anim.GetComponent<Animator>().IsInTransition(0))
        {
            landing = false;
            anim.SetBool(animLand, false);
        }
        if (anim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(crawlEndName) && anim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !anim.GetComponent<Animator>().IsInTransition(0) )
        {
            crawling = false;
            anim.SetBool(crawlEndName, true);
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
    public virtual bool GetJumpState()
    {
        return jumping;
    }

    public virtual bool GetCrawlState()
    {
        return crawling;
    }
    
    public virtual bool GetLandState()
    {
        return landing;
    }

    public virtual void Jump()
    {
        anim.SetBool(animJump, true);
        jumping = true;
    }

    public virtual void Crawl(bool state)
    {
        anim.SetBool(animCrawl, state);

        if (state)
        {
            crawling = true;
            anim.SetBool(crawlEndName, false);
        }
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
        anim.SetBool(animWalk, state);
    }

    public virtual void Falling(bool state)
    {
        anim.SetBool(animFall, state);
    }

    public virtual void Pull(bool state)
    {
        anim.SetBool(animPull,state);
    }

    public virtual void Push(bool state)
    {
        anim.SetBool(animPush, state);
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