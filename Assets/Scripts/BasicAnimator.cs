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
    public float Facing;

    protected float X;
    protected float savedX;
    protected float Y;
    Animator anim;
    private new Rigidbody2D rigidbody2D;
   




    protected virtual void Start()
    {
        savedX = Facing;
        savedX = savedX > 0 ? 1 : -1;
        anim = animGameObject.GetComponent<Animator>();
        anim.SetBool(animWalk, false);//Walking animation is deactivated
        anim.SetBool(animAttack, false);//Attacking animation is deactivated
        anim.SetBool(animFall, false);//Falling animation is deactivated

        rigidbody2D = GetComponent<Rigidbody2D>();
    }
    protected virtual void Update()
    {

    }

    public virtual void Attack()
    {
        anim.SetBool(animAttack, true);
        anim.SetBool(animAttack, false);
    }
    protected virtual void Flip()
    {

        if (X != savedX && X != 0)
        {
            savedX = X;
            // FLIP ?
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
        /*
        if (faceright == true && X > 0 || faceright == false && X < 0)
        {
            savedX = X;
            faceright = !faceright;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        } */
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
        X = dir.x;
        if (X != 0)
        {
            X = dir.x > 0 ? 1 : -1;

            anim.SetBool(animWalk, true);
        }
        else
            anim.SetBool(animWalk, false);//Walking animation is activated


        Flip();
    }
}