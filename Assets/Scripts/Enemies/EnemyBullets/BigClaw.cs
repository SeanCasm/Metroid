using System.Collections;
using System.Collections.Generic;
using Enemy.Weapons;
using System;
using UnityEngine;
/// <summary>
/// Kraid big claw projectil
/// </summary>
public class BigClaw : Weapon
{
    [SerializeField] Collider2D floorCol;
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject oneSide,damageCol;
    Animator animator;
    public event Action OnDisable;
    private float curSpeed;
    private new void OnEnable() {
        
    }
    private new void OnBecameInvisible() {
        
    }
    new void Start() {
        base.Start();
        curSpeed=speed;
        animator=GetComponent<Animator>();
    }
    new void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Suelo") && other.IsTouching(floorCol)){
            base.speed=0;
            damageCol.SetActive(false);
            oneSide.SetActive(true);
            Invoke("Break",livingTime/2);
        }
    }
    void Break(){
        animator.SetTrigger("Disappear");
        Invoke("Explode",livingTime/2);
    }
    void Explode(){
        Instantiate(explosion,transform.position,Quaternion.identity,null);
        speed=curSpeed;
        damageCol.SetActive(true);
        oneSide.SetActive(false);
        animator.Rebind();
        OnDisable?.Invoke();
    }
}