using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Weapons;
public class BlackHole : Weapon
{
    [SerializeField] CircleCollider2D circleCollider;
    private Animator anim;
    private PlayerController pContr;
    private bool collided;
    new void Start()
    {
        base.Start();
    }
    protected override void OnEnable() {
        Invoke("Anim",5f);
    }
    protected override void OnBecameInvisible()
    {
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
            GameEvents.damagePlayer.Invoke(damage, transform.position.x);
            pContr=collision.GetComponentInParent<PlayerController>();
            pContr.slow2Forces = 1.5f;
            Anim();
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            pContr.slow2Forces = 1f;
        }
    }
    public void Destroy(){
        Destroy(gameObject);
    }
    void Anim(){
        collided = true;
        anim.SetBool("OnGround", true);
    }
}
