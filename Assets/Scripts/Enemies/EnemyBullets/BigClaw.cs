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
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject damageCol,ground;
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
        animator=GetComponentInChildren<Animator>();
    }
    new void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Suelo")){
            base.speed=0;
            damageCol.SetActive(false);
            ground.SetActive(true);
            rigid.bodyType = RigidbodyType2D.Static;
            Invoke("Break",livingTime/2);
        }
    }
    void Break(){
        animator.SetTrigger("Disappear");
        Invoke("Explode",livingTime/2);
    }
    void Explode(){
        ground.SetActive(false);
        Instantiate(explosion,transform.position,Quaternion.identity,null);
        rigid.bodyType = RigidbodyType2D.Kinematic;
        speed=curSpeed;
        damageCol.SetActive(true);
        animator.Rebind();
        OnDisable?.Invoke();
    }
}