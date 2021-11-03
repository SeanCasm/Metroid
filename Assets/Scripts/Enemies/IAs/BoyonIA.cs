using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using Player.Weapon;
public class BoyonIA : EnemyBase
{
    [Tooltip("Distance between player and this enemy, to go towards the player")]
    [SerializeField] float distance;
    [SerializeField] float damageDelay,stunnedTime;
    private Animator animator;
    private bool onPlayer,isStunned;
    private PlayerHealth pHealth;
    private float currentSpeed;
    private EnemyHealth eHealth;
    private void Start()
    {
        currentSpeed=speed;
        animator=GetComponent<Animator>();
        eHealth=GetComponentInChildren<EnemyHealth>();
    }
    private void Update()
    {
        if (Vector2.Distance(transform.position, pDetect.GetPlayerTransformCenter()) <= distance && !onPlayer)
        {
            transform.position = Vector2.MoveTowards(transform.position, pDetect.GetPlayerTransformCenter(), speed * Time.deltaTime);
        }else if(onPlayer){
            transform.position=pDetect.GetPlayerTransformCenter();
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            pHealth=other.transform.parent.parent.GetComponent<PlayerHealth>();
            onPlayer=true;
            InvokeRepeating("Damage",damageDelay,damageDelay);
            animator.SetFloat("Anim speed", 3f);
        }
        if(other.GetComponent<Projectil>()){
            CancelInvoke("OutStunt");
            isStunned=true;
            speed=0;
            animator.SetFloat("Anim speed",4f);
            Invoke("OutStunt",stunnedTime);
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")){
            CancelInvoke("Damage");
            onPlayer=false;
            animator.SetFloat("Anim speed",1f);
        }
    }
    void OutStunt(){
        speed=currentSpeed;
        onPlayer=isStunned=false;
        animator.SetFloat("Anim speed", 1);
        CancelInvoke("Damage");
    }
    void Damage(){
        pHealth.ConstantDamage(eHealth.collideDamage);
    }
}
