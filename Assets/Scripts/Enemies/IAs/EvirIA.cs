using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using Enemy.Weapon;
public class EvirIA : EnemyBase
{
    [SerializeField]Transform shootPoint;
    [SerializeField]Pool bulletsPool;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float groundAware;
    private float currentSpeed;
    private int dir=1;
    private new void OnEnable() {
        base.OnEnable();
        pDetect.OnDetection+=Attack;
        pDetect.OnOut+=OnPlayerExit;
    }
    private void OnDisable() {
        pDetect.OnDetection -= Attack;
        pDetect.OnOut -= OnPlayerExit;
    }
    void Start()=>currentSpeed = speed;
    void Update()
    {
        if (pDetect.detected)
        {
            if (pDetect.player.transform.position.x < transform.position.x && transform.localScale.x>0)
            {
                transform.localScale=new Vector2(-1f,1f);
            }else if(pDetect.player.transform.position.x > transform.position.x && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(1f, 1f);
            }
            currentSpeed=0;
        }else currentSpeed=speed;
        if(Physics2D.Raycast(transform.position,Vector2.up*dir,groundAware,layerMask)){
            dir*=-1;
        }
    }
    void FixedUpdate()=>rigid.velocity = new Vector2(0f, currentSpeed*dir) * Time.deltaTime;
    private void Attack(){
        anim.SetBool("Attack", true);
    } 
    private void OnPlayerExit(){
        anim.SetBool("Attack", false);
        currentSpeed = speed;
    }
    public void Shoot(){
        bulletsPool.ActiveNextPoolObject();
    }
}
