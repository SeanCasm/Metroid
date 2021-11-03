using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Weapon;
public class BouncingBomb : Bomb,IPooleable
{
    [SerializeField] float speed;
    [SerializeField] bool pooleable;
    private Rigidbody2D rigid;
    private bool poolRemoved,isExploding;
    bool IPooleable.pooleable { get => this.pooleable; set => this.pooleable=value; }

    private float currentSpeed;
    new void Start() {
        base.Start();
        rigid=GetComponent<Rigidbody2D>();
    }
    new void OnEnable(){
        base.OnEnable();
        Pool.OnPoolChanged+=PoolChanged;
        currentSpeed = speed;
        OnExplosion+=BackToGun;
        Invoke("Explode",timeToExplode);
        direction = parent.right;
        transform.eulerAngles = parent.eulerAngles;
    }
    private void OnDisable() {
        Pool.OnPoolChanged -= PoolChanged;
        OnExplosion -= BackToGun;
    }
    void FixedUpdate()
    {
        rigid.velocity = direction.normalized * currentSpeed;
    }
    new void OnTriggerEnter2D(Collider2D collision)
    {
        if(isExploding)base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("Suelo")||collision.CompareTag("Crumble")) currentSpeed = 0;
    }
    void Explode(){
        animator.SetTrigger("Explode");
        isExploding=true;
        currentSpeed=0;
    }
    private void PoolChanged()=>poolRemoved = true;
    private void BackToGun()
    {
        if (!pooleable || poolRemoved)Destroy(gameObject);
        else audioPlayer.clip = null;

        isExploding=false;
    }
}