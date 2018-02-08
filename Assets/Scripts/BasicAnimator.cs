using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimator : MonoBehaviour
{
    public const string animWalk = "IsRunning";
    public const string animAttack = "IsAttacking";
    public const string animFall = "IsFalling";

    public GameObject animGameObject;
    [Range(-1,1)]
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
        anim.SetBool(animWalk, false);//Walking animation is deactivated
        anim.SetBool(animAttack, false);//Attacking animation is deactivated
        anim.SetBool(animFall, false);//Falling animation is deactivated

        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {       
        if (anim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.GetComponent<Animator>().IsInTransition(0))
        {
            anim.SetBool(animAttack, false);
        } 
    }

    public virtual void Attack()
    {
        anim.SetBool(animAttack, true);
    }
    protected virtual void Flip()
    {
        if (x != savedX && x != 0)
        {
            savedX = x;
            // FLIP ?
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
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

            anim.SetBool(animWalk, true);
        }
        else
            anim.SetBool(animWalk, false);//Walking animation is activated

        if(flip)
        Flip();
    }
}