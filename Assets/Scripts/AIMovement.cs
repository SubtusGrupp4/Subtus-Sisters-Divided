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
    public string[] EngageOn = new string[] { };
    [GiveTag]
    public string[] BounceOn = new string[] { };
    [GiveTag]
    public string[] WalkOn = new string[] { };


    [Header("States")]

    public MovementEnum StartState;
    public MovementEnum EngagedState;
    protected MovementEnum currentState;
    protected MovementEnum savedState;

    [Header("Stats")]

    public float Speed;
    public float StepRange;
    public float MaxSlope;
    public float TurnRate;
    protected float minSlope = 0.1f; // Save some perfomance.

    public float AggroRange;

    protected bool isDead = false;
    protected float distanceGraceForFalling = 0.2f;


    [Header("Check Points")]

    public List<GameObject> CheckPoints = new List<GameObject>();
    protected List<Vector2> checkPointPos = new List<Vector2>();
    protected List<bool> checkPointCheck = new List<bool>();
    protected bool reverse = false;
    protected int checkPointIndex = 0;

    protected bool isFalling;
    protected bool engaged;

    protected GameObject target;

    protected bool bounce;
    protected Vector2 directionMultiplier; // flip the X value to make the char go the opesite direction
    protected int flipValue = 1; // used to flip the Gravity

    protected new Rigidbody2D rigidbody2D;
    protected List<GameObject> allTargets = new List<GameObject>();

    protected RaycastHit2D[] objHit;
    protected float rayDistanceHypot;
    protected float rayDistanceX;
    protected float rayOffset;
    protected float slopeRayOffset;

    Vector2 desiredDir;



    void Start()
    {
        currentState = StartState;

        if (GetComponent<Rigidbody2D>())
            rigidbody2D = GetComponent<Rigidbody2D>();

        for (int i = 0; i < CheckPoints.Count; i++)
        {
            checkPointPos.Add(CheckPoints[i].transform.position);
            checkPointCheck.Add(false);
        }

        //Maxslope
        MaxSlope = Mathf.Sin((MaxSlope * Mathf.PI) / 180);



        directionMultiplier = new Vector2(1, 0);

        // Raydistance
        rayDistanceHypot = Mathf.Pow((GetComponent<BoxCollider2D>().size.y * GetComponent<Transform>().localScale.y), 2) + // X^2
           Mathf.Pow((GetComponent<BoxCollider2D>().size.x * GetComponent<Transform>().localScale.x), 2);             // Y^2 

        rayDistanceHypot = Mathf.Sqrt(rayDistanceHypot);
        rayDistanceHypot += 0.15f; // offSet.

        // SlopeRay
        rayOffset = GetComponent<BoxCollider2D>().size.x * transform.localScale.x / 2;

        slopeRayOffset = rayOffset;

        //Xray
        rayDistanceX = GetComponent<BoxCollider2D>().size.x * transform.localScale.x;
    }




   protected virtual void Update()
    {
        if (!GetComponent<Rigidbody2D>())
            isDead = true;


        if (!isDead)
        {
            // if (EngageOn != null)
            if (engaged == false || target == null)
            {
                CheckEngagement();
            }

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

    private void CheckFalling()
    {
        isFalling = true;
        for (int l = -1; l < 2; l += 2)
        {
            objHit = Physics2D.RaycastAll(transform.position, new Vector2(l, -flipValue),
                rayDistanceHypot + distanceGraceForFalling);

            Debug.DrawRay(transform.position, new Vector2(l, -flipValue), Color.grey);

            for (int i = 0; i < WalkOn.Length; i++)
            {
                for (int j = 0; j < objHit.Length; j++)
                {
                    if (objHit[j].transform.tag == WalkOn[i])
                    {
                        isFalling = false;
                    }
                }
            }
        }
    }

   protected virtual void CheckEngagement()
    {
        allTargets.Clear(); // clear the target list, incase another target is added or removed etc...
        if (EngageOn.Length >= 1)
            for (int i = 0; i < EngageOn.Length; i++)
                allTargets.AddRange(GameObject.FindGameObjectsWithTag(EngageOn[i]));

        float tempDistance = AggroRange;
        GameObject tempTarget = null;

        for (int i = 0; i < allTargets.Count; i++)
        {
            float distance = Vector2.Distance(transform.position, allTargets[i].transform.position);

            if (distance > AggroRange)
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
            currentState = EngagedState;
        }
    }

    private void Move()
    {
        if (currentState == MovementEnum.Stalking)
        {
            float xDistance = target.transform.position.x - transform.position.x; // positive value = right, negative = left

            if (xDistance > 0)
                directionMultiplier = new Vector2(1, rigidbody2D.velocity.y);

            if (xDistance < 0)
                directionMultiplier = new Vector2(-1, rigidbody2D.velocity.y);

            rigidbody2D.velocity = new Vector2(directionMultiplier.x * Speed, rigidbody2D.velocity.y);


            //    rigidbody2D.velocity = new Vector2(GetComponent<Rigidbody2D>, GetComponent<Rigidbody2D>().velocity.y);
        }

         else if (currentState == MovementEnum.OneDirBounce)
        {

            CheckFalling();



            // PRO HACKER LMAO
            if (rigidbody2D.velocity == Vector2.zero)
            {
                Debug.Log("got stuck" + rigidbody2D.velocity);

                Debug.Log("Direction" + directionMultiplier);

                Debug.Log("Falling" + isFalling);

                Debug.Log("Wall" + CheckWall());

                Debug.Log("Slope" + CheckSlope());

                Debug.Log("Ledge" + CheckLedge());
                rigidbody2D.AddForce(new Vector2(10, 10));

            }

            if (!isFalling)
            {
                rigidbody2D.velocity = new Vector2(Speed * directionMultiplier.x, rigidbody2D.velocity.y);
                Debug.Log("AM I EVEN RUNNING ?");

                if (CheckSlope() == true)
                {
                    // DO NOTHING
                    // Therefore keeping the same direction and speed as "inteded"

                }
                else if (CheckLedge())
                    Bounce();


                if (CheckWall())
                    Bounce();

                NormalizeSlope();

            }
        }

        else if (currentState == MovementEnum.Patrolling)
        {
            if (Vector2.Distance(transform.position, checkPointPos[checkPointIndex]) <= patrollGrace)
            {
                if (reverse)
                    checkPointIndex--;

                else if (!reverse)
                    checkPointIndex++;
            }

            if (checkPointIndex == CheckPoints.Count - 1)
            {
                reverse = true;
            }
            else if (checkPointIndex == 0)
            {
                reverse = false;
            }

            desiredDir = (checkPointPos[checkPointIndex] - (Vector2)transform.position).normalized;
             rigidbody2D.velocity = desiredDir * Speed;
           // transform.Translate(new Vector3(desiredDir.x, desiredDir.y, 0) * Speed * Time.deltaTime);
        }

        else if (currentState == MovementEnum.Idle)
        {
            //  if (rigidbody2D.velocity.x != 0)
            //     rigidbody2D.velocity = Vector2.zero;
        }

    }

    public Vector2 GoingWhere()
    {
        return desiredDir;
    }

    public Vector2 Facing()
    {
        return directionMultiplier;
    }

    private bool CheckWall()
    {
        bool walls = false;

        objHit = Physics2D.RaycastAll(transform.position, new Vector2(directionMultiplier.x, 0), rayDistanceX);
        Debug.DrawRay(transform.position, new Vector2(directionMultiplier.x, 0), Color.green);

        for (int i = 0; i < objHit.Length; i++)
        {
            for (int j = 0; j < BounceOn.Length; j++)
            {
                if (objHit[i].transform.tag == BounceOn[j])
                {

                    if ((Mathf.Abs(objHit[i].normal.x) >= MaxSlope))
                    {
                        walls = true;
                    }
                }
            }
        }


        return walls;
    }

    private bool CheckSlope()
    {
        bool slopes = false;

        objHit = Physics2D.RaycastAll(transform.position + new Vector3((rayOffset + 1.1f) * directionMultiplier.x, -slopeRayOffset * flipValue - 0.1f * flipValue, 0), -Vector2.right * directionMultiplier.x, StepRange * 3);

        Debug.DrawRay(transform.position + new Vector3((rayOffset + 1.1f) * directionMultiplier.x, -slopeRayOffset * flipValue - 0.1f * flipValue), -Vector2.right * directionMultiplier.x, Color.blue);

        for (int i = 0; i < objHit.Length; i++)
        {
            for (int j = 0; j < WalkOn.Length; j++)
            {
                if (objHit[i].transform.tag == WalkOn[j])
                {
                    if (Mathf.Abs(objHit[i].normal.x) > minSlope && (Mathf.Abs(objHit[i].normal.x) < MaxSlope))
                    {
                        slopes = true;
                    }
                }
            }
        }


        return slopes;
    }

    private bool CheckLedge()
    {
        bool floors = true;

        objHit = Physics2D.RaycastAll(transform.position + new Vector3((rayOffset + 0.1f) * directionMultiplier.x, 0, 0), -Vector2.up * flipValue, StepRange);

        Debug.DrawRay(transform.position + new Vector3((rayOffset + 0.1f) * directionMultiplier.x, 0, 0), -Vector2.up * flipValue, Color.red);

        for (int i = 0; i < objHit.Length; i++)
        {
            for (int j = 0; j < WalkOn.Length; j++)
            {
                if (objHit[i].transform.tag == WalkOn[j])
                {
                    floors = false;
                }
            }
        }

        /*  if (objHit.Length <= 0)
              return true;
          else
              return false; */

        return floors;








        //  else
        //   Debug.Log("objhitlengt " + objHit.Length);




        /*for(int i = 0; i < objHit.Length; i++)
        {
            if(objHit[i].transform.gameObject == this.gameObject)
            {
                if(objHit)
            }
        }*/
        /*
        for (int i = 0; i < objHit.Length; i++)
        {


            if (objHit[i].transform.tag != BounceOn[0])
            {
                Debug.Log("AI hit" + objHit[i].transform.gameObject);
                directionMultiplier *= -1;
            }
        }*/
        // Incase we cant have boxcollider
        /*
        for(int i = 0; i < BounceOn.Length; i++)
        {

            for(int j = 0; j < objHit.Length; i++)
            {

            }


        }
           */
        // if(objHit)

    }

    private void Bounce()
    {
        directionMultiplier *= -1;


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
            for (int i = 0; i < WalkOn.Length; i++)
            {
                objHit = Physics2D.RaycastAll(transform.position, new Vector2(l, -flipValue),
                 rayDistanceHypot + distanceGraceForFalling);

                for (int j = 0; j < objHit.Length; j++)
                {
                    if (objHit[j].transform.tag != WalkOn[i])
                        continue;

                    if (objHit[j].collider != null && Mathf.Abs(objHit[j].normal.x) > 0.1f && Mathf.Abs(objHit[j].normal.x) < MaxSlope && isFalling == false)
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
