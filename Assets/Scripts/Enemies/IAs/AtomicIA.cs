using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
public class AtomicIA : EnemyBase
{
    [Tooltip("Distance between player and this enemy, to go towards the player")]
    [SerializeField] float distance;
    [SerializeField] bool horizontalPattern;
    [SerializeField] LayerMask ground;
    private bool isPatrolling, movingAway;
    private (Vector3 dir, float time) curMove;
    private (Vector3 dir, float time)[] movement = new (Vector3 dir, float time)[10];
    private new void Awake()
    {
        base.Awake();
        for (int i = 0; i < movement.Length; i++)
        {
            if (horizontalPattern) movement[i].dir = new Vector3(Random.Range(-2.5f, 2.5f), Random.Range(-.25f, .25f));
            else movement[i].dir = new Vector3(Random.Range(-.5f, .5f), Random.Range(-2.5f, 2.5f));
            movement[i].time = Random.Range(2, 5);
        }
        curMove = movement[Random.Range(0, movement.Length - 1)];
        InvokeRepeating(nameof(NewPatrolMove), curMove.time, curMove.time);
    }

    private void Update()
    {
        if (pDetect.detected)
        {
            if (Vector2.Distance(transform.position, pDetect.GetPlayerTransformCenter()) <= distance)
            {
                transform.position = Vector2.MoveTowards(transform.position, pDetect.GetPlayerTransformCenter(), speed * Time.deltaTime);
            }
            isPatrolling = false;
        }
        else isPatrolling = true;

        if (isPatrolling)
        {
            RaycastHit2D raycastHit2D = Physics2D.CircleCast(transform.position, .2f, Vector2.zero, 0, ground);
            if (!raycastHit2D && !movingAway)
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + curMove.dir, speed * Time.deltaTime);
            }
            else
            {
                movingAway = true;
                if(!IsInvoking(nameof(StopMovingAway)))Invoke(nameof(StopMovingAway),1.5f);
                transform.position = Vector2.MoveTowards(transform.position, transform.position + curMove.dir * -1, speed * Time.deltaTime);
            }
        }
    }
    private void StopMovingAway(){
        movingAway=false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, .2f);
    }
    void NewPatrolMove()
    {
        curMove = movement[Random.Range(0, movement.Length - 1)];
    }
}
