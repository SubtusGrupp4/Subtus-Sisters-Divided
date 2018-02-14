using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementEnum
{
    Patrolling, Stalking, OneDirBounce, Idle
}

public class AIMovement : MonoBehaviour
{
    protected const float patrollGrace = 0.1f;

    [Header("Interactions")]

    [GiveTag]
    public string[] engageOn = new string[] { };
    [GiveTag]
    public string[] bounceOn = new string[] { };
    [GiveTag]
    public string[] walkOn = new string[] { };


    [Header("States")]

    public bool flipped;

    public MovementEnum startState;
    public MovementEnum engagedState;
    protected MovementEnum currentState;
    protected MovementEnum savedState;

    [Header("Stats")]

    public float speed;
    public float stepRange;
    public float climbRange;
    public float maxSlope;
    public float turnRate;
    protected float minSlope = 0.1f; // Save some perfomance.

    public float aggroRange;

    protected bool isDead = false;
    protected float distanceGraceForFalling = 0.2f;


    [Header("Check Points")]

    public List<GameObject> checkPoints = new List<GameObject>();
    protected List<Vector2> checkPointPos = new List<Vector2>();
    protected List<bool> checkPointCheck = new List<bool>();
    protected bool reverse = false;
    protected int checkPointIndex = 0;

    protected bool isFalling = false;
    protected bool engaged = false;

    protected GameObject target;

    protected bool bounce;
    protected Vector2 directionMultiplier; // flip the X value to make the char go the opesite direction
    protected int flipValue = 1; // used to flip the Gravity

    protected new Rigidbody2D rigidbody2D;
    protected List<GameObject> allTargets = new List<GameObject>();

    protected RaycastHit2D[] objHit;
    protected float rayDistanceHypot;
    protected float rayDistanceX;
    protected float rayOffSetX;
    protected float rayOffSetY;
    protected float slopeRayOffset;
    protected float boxOffSetY;

    Vector2 desiredDir;
    //
    protected BasicAnimator bAnim;
    //
    protected virtual void Awake()
    {
        if (GetComponent<BasicAnimator>())
            bAnim = GetComponent<BasicAnimator>();

        currentState = startState;

        if (GetComponent<Rigidbody2D>())
            rigidbody2D = GetComponent<Rigidbody2D>();

        for (int i = 0; i < checkPoints.Count; i++)
        {
            checkPointPos.Add(checkPoints[i].transform.position);
            checkPointCheck.Add(false);
        }

        //Maxslope
        maxSlope = Mathf.Sin((maxSlope * Mathf.PI) / 180);



        directionMultiplier = new Vector2(1, 0);

        // Raydistance
        rayDistanceHypot = Mathf.Pow((GetComponent<BoxCollider2D>().size.y * GetComponent<Transform>().localScale.y), 2) + // X^2
           Mathf.Pow((GetComponent<BoxCollider2D>().size.x * GetComponent<Transform>().localScale.x), 2);             // Y^2 

        rayDistanceHypot = Mathf.Sqrt(rayDistanceHypot);
        rayDistanceHypot += 0.0f; // offSet.

        rayDistanceHypot = 0.7f;



        // SlopeRay
        rayOffSetX = GetComponent<BoxCollider2D>().size.x * transform.localScale.x / 2;
        rayOffSetY = GetComponent<BoxCollider2D>().size.y * transform.localScale.y / 2;
        boxOffSetY = GetComponent<BoxCollider2D>().offset.y;

        slopeRayOffset = rayOffSetX;

        //Xray
        rayDistanceX = (GetComponent<BoxCollider2D>().size.x * transform.localScale.x / 2) + 1f;

        if (flipped)
            Flip();
    }




    protected virtual void Update()
    {
        if (!isDead)
        {
            // if (EngageOn != null)
          //  if (engaged == false || target == null)
            {
                CheckEngagement();
            }

            CheckFalling();
            //  CheckFalling();

        }
    }

    protected virtual void FixedUpdate()
    {
        if (!isDead) //&& isFalling)
        {
            Move();
        }
    }

    public void Freeze()
    {
        savedState = currentState;
        currentState = MovementEnum.Idle;
    }

    public void UnFreeze()
    {
        currentState = savedState;
    }

    public void Die()
    {
        isDead = true;
    }

    protected void CheckFalling()
    {
        isFalling = true;
        //
        if (bAnim != null)
            bAnim.Falling(true);
        //
        for (int l = -1; l < 2; l += 2)
        {
            objHit = Physics2D.RaycastAll(transform.position + new Vector3((rayOffSetX - 0.1f) * l, boxOffSetY * flipValue, 0),
                -Vector2.up * flipValue,
                rayOffSetY + distanceGraceForFalling);

            Debug.DrawRay(transform.position + new Vector3((rayOffSetX - 0.1f) * l, boxOffSetY * flipValue, 0), -Vector2.up * flipValue, Color.grey);

            for (int i = 0; i < walkOn.Length; i++)
            {
                for (int j = 0; j < objHit.Length; j++)
                {
                    if (objHit[j].transform.tag == walkOn[i])
                    {
                        if (Mathf.Abs(objHit[j].normal.x) < maxSlope) // So we cant jump on walls.
                        {
                            isFalling = false;
                            //
                            if (bAnim != null)
                                bAnim.Falling(false);
                            //
                            break;
                        }
                    }
                }
            }
        }
    }

    protected virtual void CheckEngagement()
    {
        allTargets.Clear(); // clear the target list, incase another target is added or removed etc...
        if (engageOn.Length >= 1)
            for (int i = 0; i < engageOn.Length; i++)
                allTargets.AddRange(GameObject.FindGameObjectsWithTag(engageOn[i]));

        float tempDistance = aggroRange;
        GameObject tempTarget = null;

        for (int i = 0; i < allTargets.Count; i++)
        {
            float distance = Vector2.Distance(transform.position, allTargets[i].transform.position);

            if (distance > aggroRange)
                continue;

            if (distance <= tempDistance)
            {
                tempDistance = distance;
                tempTarget = allTargets[i];
            }
        }
        target = tempTarget;
        if (target != null)
        {
            engaged = true;
            currentState = engagedState;
        }
    }

    private void Move()
    {
        //
        //
        //

        if (currentState == MovementEnum.Stalking)
        {
            StalkingMove();

        }

        else if (currentState == MovementEnum.OneDirBounce)
        {

            OneDirmove();
        }

        else if (currentState == MovementEnum.Patrolling)
        {
            PatrollingMove();
        }

        else if (currentState == MovementEnum.Idle)
        {
            IdleMove();
        }

    }

    protected void StalkingMove()
    {
        //
        if (bAnim != null)
            bAnim.Walking(directionMultiplier, true);
        //


        float xDistance = target.transform.position.x - transform.position.x; // positive value = right, negative = left

        if (xDistance > 0)
            directionMultiplier = new Vector2(1, rigidbody2D.velocity.y);

        if (xDistance < 0)
            directionMultiplier = new Vector2(-1, rigidbody2D.velocity.y);

       

        if (CheckWall(transform.position))
        {
            if (CheckJump())
            {
                // DOING JUMP 
                isFalling = true;
            }
        }
        NormalizeSlope();

        rigidbody2D.velocity = new Vector2(directionMultiplier.x * speed, rigidbody2D.velocity.y);

    }

    protected void OneDirmove()
    {
        //
        if (bAnim != null)
            bAnim.Walking(directionMultiplier, true);
        //

        // PRO HACKER 
        if (rigidbody2D.velocity == Vector2.zero)
        {
            /*
            Debug.Log("got stuck" + rigidbody2D.velocity);

            Debug.Log("Direction" + directionMultiplier);

            Debug.Log("Falling" + isFalling);

            Debug.Log("Wall" + CheckWall());

            Debug.Log("Slope" + CheckSlope());

            Debug.Log("Ledge" + CheckLedge()); */
            rigidbody2D.AddForce(new Vector2(10, 50));
        }

        rigidbody2D.velocity = new Vector2(speed * directionMultiplier.x, rigidbody2D.velocity.y);


        if (!isFalling)
        {

            if (CheckWall(transform.position))
            {
                if (CheckJump())
                {
                    // DOING JUMP 
                    isFalling = true;
                }
                else
                    Bounce();
            }
            else if (CheckSlope() == false)
            {
                if (CheckLedge())
                    Bounce();

            }

            NormalizeSlope();

        }
    }

    protected void PatrollingMove()
    {
        //
        if (bAnim != null)
            bAnim.Walking(directionMultiplier, true);
        //

        if (Vector2.Distance(transform.position, checkPointPos[checkPointIndex]) <= patrollGrace)
        {
            if (reverse)
                checkPointIndex--;

            else if (!reverse)
                checkPointIndex++;
        }

        if (checkPointIndex == checkPoints.Count - 1)
        {
            reverse = true;
        }
        else if (checkPointIndex == 0)
        {
            reverse = false;
        }

        desiredDir = (checkPointPos[checkPointIndex] - (Vector2)transform.position).normalized;
        rigidbody2D.velocity = desiredDir * speed;
        // transform.Translate(new Vector3(desiredDir.x, desiredDir.y, 0) * Speed * Time.deltaTime);
    }

    protected void IdleMove()
    {
        if (bAnim != null)
            bAnim.Walking(Vector2.zero, false);

        //  if (rigidbody2D.velocity.x != 0)
        //     rigidbody2D.velocity = Vector2.zero;
    }

    public Vector2 GoingWhere()
    {
        return desiredDir;
    }

    public Vector2 Facing()
    {
        return directionMultiplier;
    }

    protected bool CheckWall(Vector3 pos)
    {
        bool walls = false;

        objHit = Physics2D.RaycastAll(pos + new Vector3(0,boxOffSetY * flipValue,0), new Vector2(directionMultiplier.x, 0), rayDistanceX);

        Debug.DrawRay(pos + new Vector3(0, boxOffSetY * flipValue, 0), new Vector2(directionMultiplier.x, 0), Color.green);

        for (int i = 0; i < objHit.Length; i++)
        {
            for (int j = 0; j < bounceOn.Length; j++)
            {
                if (objHit[i].transform.tag == bounceOn[j])
                {

                    if ((Mathf.Abs(objHit[i].normal.x) >= maxSlope))
                    {
                        walls = true;
                    }
                }
            }
        }
        return walls;
    }

    protected bool CheckSlope()
    {
        bool slopes = false;

        objHit = Physics2D.RaycastAll(transform.position + new Vector3((rayOffSetX + 1.1f) * directionMultiplier.x, (-slopeRayOffset * flipValue) - (0.1f * flipValue) + (boxOffSetY * flipValue), 0),
            -Vector2.right * directionMultiplier.x,
            stepRange * 3);

        Debug.DrawRay(transform.position + new Vector3((rayOffSetX + 1.1f) * directionMultiplier.x, (-slopeRayOffset * flipValue) - (0.1f * flipValue) + (boxOffSetY * flipValue), 0), -Vector2.right * directionMultiplier.x, Color.blue);

        for (int i = 0; i < objHit.Length; i++)
        {
            for (int j = 0; j < walkOn.Length; j++)
            {
                if (objHit[i].transform.tag == walkOn[j])
                {
                    if (Mathf.Abs(objHit[i].normal.x) > minSlope && (Mathf.Abs(objHit[i].normal.x) < maxSlope))
                    {
                        slopes = true;
                    }
                }
            }
        }


        return slopes;
    }

    protected bool CheckLedge()
    {
        bool floors = true;
        // (transform.position + new Vector3((rayOffset + 0.1f)
        // (transform.position + new Vector3((rayOffSetX - 0.1f) * directionMultiplier.x, 0, 0)
        objHit = Physics2D.RaycastAll(transform.position + new Vector3((rayOffSetX - 0.1f) * directionMultiplier.x, boxOffSetY * flipValue, 0)
            , -Vector2.up * flipValue,
            stepRange);

        Debug.DrawRay(transform.position + new Vector3((rayOffSetX - 0.1f) * directionMultiplier.x, boxOffSetY * flipValue, 0), -Vector2.up * flipValue, Color.red);

        for (int i = 0; i < objHit.Length; i++)
        {
            for (int j = 0; j < walkOn.Length; j++)
            {
                if (objHit[i].transform.tag == walkOn[j])
                {
                    floors = false;
                }
            }
        }

        return floors;
    }

    protected bool CheckJump()
    {
        float height;
        bool jump = false;

        for (int i = 1; i <= climbRange; i++)
        {
            if (!CheckWall(new Vector3(transform.position.x, // X
                  transform.position.y + (i * flipValue),                 // Y
                  transform.position.z)))                   // Z
            {
                height = i;
                CalculateJump(height);
                jump = true;
                break;
            }
        }

        // Loopa från min höjd, uppåt, mer smooth hop??

        return jump;
    }
    protected void CalculateJump(float height)
    {
        bAnim.Jump();
        //   height;
        float offSetValue;
        offSetValue = rayOffSetY  + 0.5f;
        // U^2 = V^2 - 2as
        float jumpVelocity;
        float gravity;
        gravity = -9.81f; // a
                          // height = s
                          // V = end velocity = 0
                                                // OffSetMultiplier
        jumpVelocity = 0 - (2 * (gravity * (height + offSetValue)));
        jumpVelocity = Mathf.Sqrt(jumpVelocity);

        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpVelocity * flipValue);

    }

    protected void Bounce()
    {
        directionMultiplier *= -1;
    }
    protected void Flip()
    {
        flipValue *= -1;
        rigidbody2D.gravityScale *= -1;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * -1, transform.localScale.x);
    }


    void NormalizeSlope()
    {
        float slopeFriction = 0.11f;

        // Small optimization (if first ray hit we dont raycast a second time).
        bool slope = false;

        // Raycasting twice, meanwhile using L as direction because we want L to be between -1 and 1 so we raycast left and right...
        for (int l = -1; l < 2; l += 2)
        {
            // Attempt vertical normalization
            for (int i = 0; i < walkOn.Length; i++)
            {
                objHit = Physics2D.RaycastAll(transform.position, new Vector2(l, -flipValue),
                 rayDistanceHypot + distanceGraceForFalling);

                for (int j = 0; j < objHit.Length; j++)
                {
                    if (objHit[j].transform.tag != walkOn[i])
                        continue;

                    if (objHit[j].collider != null && Mathf.Abs(objHit[j].normal.x) > 0.1f && Mathf.Abs(objHit[j].normal.x) < maxSlope && isFalling == false)
                    {


                        slopeFriction *= Mathf.Abs(objHit[j].normal.x);

                        Rigidbody2D body = GetComponent<Rigidbody2D>();
                        // Apply the opposite force against the slope force 
                        body.velocity = new Vector2(body.velocity.x - (objHit[j].normal.x * slopeFriction), body.velocity.y);

                        //Move Player up or down to compensate for the slope below them
                        Vector3 pos = transform.position;
                        float offSet = 0;
                        //              "-1"          normalen     *       hastigheten          *       deltatime     *  hastigheten - (1 / -1)
                        offSet += (flipValue * -1) * objHit[j].normal.x * Mathf.Abs(body.velocity.x) * Time.deltaTime * (body.velocity.x - objHit[j].normal.x > 0 ? 1f : -1f);

                        if (offSet * flipValue > 0)
                            offSet *= 0.5f;
                        else
                            offSet *= 2;

                        pos.y += offSet;
                        transform.position = pos;

                        slope = true;

                    }
                }
            }
            if (slope)
                break;
        }
    }

}
