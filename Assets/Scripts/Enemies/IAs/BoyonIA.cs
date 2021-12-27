using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using Player.Weapon;
public class BoyonIA : EnemyBase
{
    [SerializeField] float idleTime;
    [SerializeField] Transform target;
    private Animator animator;
    private Vector2 spawn,curTarget;
    private float currentSpeed,curIdleTime;
    private void Start()
    {
        target.SetParent(null);
        currentSpeed = speed;
        curTarget = target.position;
        curIdleTime=idleTime;
        spawn=transform.position;
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if(Vector2.Distance(transform.position,curTarget)>0.005f){
            transform.position=Vector2.MoveTowards(transform.position,curTarget,currentSpeed*Time.deltaTime);
            curIdleTime=idleTime;
        }else{
            curIdleTime-=Time.deltaTime;
            if(curIdleTime<=0){
                curTarget = Vector2.Distance(transform.position,spawn)<0.01f ? (Vector2)target.position : spawn;
            }
        }
    }
    
}
