using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
public class Owtch : EnemyBase
{
    [Tooltip("This multiply the current speed when the player is detected around this enemy.")]
    [SerializeField] float speedMultiplier;
    [SerializeField] Collider2D hurt;
    [Tooltip("Time to hide after hits the player")]
    [SerializeField] float hideTime;
    [Tooltip("Time it takes to hide underground")]
    [SerializeField,Range(0.35f,2f)] float hidingVel;
    [Tooltip("Time it takes to get out underground. Half of this time is corresponding the time it's hiding")]
    [SerializeField] float timeToOut;
    [SerializeField] Collider2D coll;
    [SerializeField] float spriteHeight;
    private GroundChecker efd;
    private float currentSpeed,hideAmount;
    private bool hiding;
    private void Start() {
         
        efd=GetComponent<GroundChecker>();
    }
    private new void OnEnable() {
        base.OnEnable();
        pDetect.OnDetection+=OnDetected;
        pDetect.OnOut+=OnOut;
    }
    private void OnDisable() {
        pDetect.OnDetection -= OnDetected;
        pDetect.OnOut -= OnOut;
    }
    private void Awake() {
        base.Awake();
        currentSpeed=speed;
    }
    private void FixedUpdate() {
        if (!hiding) efd.SetOnGroundVelocity(currentSpeed);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player") && other.IsTouching(hurt)){
            if(!hiding)StartCoroutine(HideMe());
        }
    }
    private void OnDetected(){
        currentSpeed *= speedMultiplier;
    }
    private void OnOut(){
        currentSpeed = speed;
    }
    private IEnumerator HideMe(){
        float vel=spriteHeight/hidingVel;
        hiding = true;
        rigid.velocity=Vector2.zero;
        yield return new WaitForSeconds(hideTime);
        efd.enabled=false;
        coll.isTrigger = true;
        hideAmount = 0;
        while(hideAmount<spriteHeight){
            hideAmount+=(vel);
            transform.position=new Vector2(transform.position.x,transform.position.y-vel);
            yield return new WaitForSeconds(vel);
        }
        hideAmount=0;
        yield return new WaitForSeconds(timeToOut);
        while(hideAmount<spriteHeight){
            hideAmount+=(vel);
            transform.position=new Vector2(transform.position.x, transform.position.y + vel);
            yield return new WaitForSeconds(vel);
        }
        coll.isTrigger = hiding = false;
        efd.enabled=true;
    }
}