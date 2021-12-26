using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
public class AtomicIA : EnemyBase
{
    [SerializeField] LayerMask ground;
    private bool movingAway;
    private (Vector3 dir, float time) curMove;
    private (Vector3 dir, float time)[] movement = new (Vector3 dir, float time)[10];
    private new void Awake()
    {
        base.Awake();
        for (int i = 0; i < movement.Length; i++)
        {
            movement[i].dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            movement[i].time = Random.Range(2, 5);
        }
        curMove = movement[Random.Range(0, movement.Length - 1)];
        InvokeRepeating(nameof(NewPatrolMove), curMove.time, curMove.time);
    }

    private void Update()
    {
        RaycastHit2D raycastHit2D = Physics2D.CircleCast(transform.position, .2f, Vector2.zero, 0, ground);
        if (raycastHit2D && !movingAway)
        {
            Invoke(nameof(StopMovingAway), .25f);
            curMove.dir *= -1;
            movingAway = true;
        }
        transform.position = Vector2.MoveTowards(transform.position, transform.position + curMove.dir, speed * Time.deltaTime);
    }
    private void StopMovingAway()
    {
        movingAway = false;
    }
    /*private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distance);
        Gizmos.DrawWireSphere(transform.position, .2f);
    }*/
    void NewPatrolMove()
    {
        curMove = movement[Random.Range(0, movement.Length - 1)];
    }
}
