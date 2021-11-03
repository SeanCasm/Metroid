using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
public class AtomicIA : EnemyBase
{
    [Tooltip("Distance between player and this enemy, to go towards the player")]
    [SerializeField] float distance;
    private new void Awake()=>base.Awake();
    private void Update() {
        if(pDetect.detected){
            if (Vector2.Distance(transform.position, pDetect.GetPlayerTransformCenter()) <= distance)
            {
                transform.position = Vector2.MoveTowards(transform.position, pDetect.GetPlayerTransformCenter(), speed * Time.deltaTime);
            }
        }
    }
}
