using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Weapons;
public class BlackHole : LookPlayerFirst
{
    [SerializeField] CircleCollider2D circleCollider;
    private Animator anim;
    private PlayerController pContr;
    private bool collided;
    protected new void OnEnable() {
        playerPosition = PlayerController.current.TransformCenter();
        Invoke("Anim",5f);
    }
    void Awake()
    {
        anim = GetComponent<Animator>();
        transform.SetParent(null);
    }
    private void Update() {
        if(!collided)SetDirection();
        else speed=0;
    }
    
    private new void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Suelo") && collision.IsTouching(circleCollider))
        {
            Anim();
        }else if(collision.GetComponent<Player.Weapon.Projectil>()!=null){
            var b=collision.GetComponent<Player.Weapon.Projectil>();
            b.FloorCollision();
        }else if(collision.CompareTag("Player")){
            PlayerKnockBack playerKnockBack = collision.GetComponent<PlayerKnockBack>();
            playerKnockBack.HitPlayer(damage,transform.position.x);
            pContr=collision.GetComponentInParent<PlayerController>();
            pContr.slow = .5f;
            Anim();
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) pContr.slow = 1f;
    }
    public void Destroy(){
        Destroy(gameObject);
    }
    void Anim(){
        collided = true;
        anim.SetBool("OnGround", true);
    }
}
