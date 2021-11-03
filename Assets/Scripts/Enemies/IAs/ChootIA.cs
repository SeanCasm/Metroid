using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
public class ChootIA : EnemyBase
{
    #region Properties
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] LayerMask groundLayer;
    private bool canJump,isGrounded,onJump,resting;

    private Transform shootpoint;
    float jumpTime = 2f;
    float currentJumpTime;
    #endregion
    #region Unity Methods
    new void Awake()
    {
        base.Awake();
        shootpoint = transform.Find("Shootpoint");
        currentJumpTime=jumpTime;
    }
    void Update()
    {
        if (pDetect.detected && !canJump && !resting)
        {
            StartCoroutine(Jump());
            canJump = true;isGrounded=false;
        }
    }
    
    IEnumerator Jump(){
        while(currentJumpTime>0){
            rigid.velocity = new Vector2(0, speed);
            currentJumpTime -= 0.2f;
            yield return new WaitForSeconds(0.1f);
        }
        currentJumpTime = jumpTime;
        StartCoroutine(Descend());
    }
    IEnumerator Descend(){
        StartCoroutine(Shoot());
        while (currentJumpTime > 0)
        {
            rigid.velocity = new Vector2(0, -speed/2);
            currentJumpTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        resting=isGrounded=true;canJump=false;
        currentJumpTime = jumpTime;
        StopAllCoroutines();//just in case...
        Invoke("StopRest",2f);
    }
    void StopRest(){
        resting=false;
    }
    void LateUpdate()
    {
        anim.SetBool("Jump", canJump);
        anim.SetBool("Grounded", isGrounded);
    }
    #endregion
    IEnumerator Shoot()
    {
        while (!isGrounded)
        {
            Instantiate(bulletPrefab, shootpoint.position, Quaternion.identity);
            yield return new WaitForSeconds(jumpTime /2);
        }
    } 
}
