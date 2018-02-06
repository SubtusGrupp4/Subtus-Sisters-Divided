using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimator : MonoBehaviour
{
    protected const string animWalk = "IsRunning";
    protected const string animAttack = "IsAttacking";
    protected const string animFall = "IsFalling";

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

    public virtual void Attack()
    {
        anim.SetBool(animAttack, true);
        anim.SetBool(animAttack, false);
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

    public virtual void Walking(Vector2 direction)
    {
        SetAnimation(direction);
    }
    protected virtual void SetAnimation(Vector2 dir)
    {
        x = dir.x;
        if (x != 0)
        {
            x = dir.x > 0 ? 1 : -1;

            anim.SetBool(animWalk, true);
        }
        else
            anim.SetBool(animWalk, false);//Walking animation is activated

        Flip();
    }
}