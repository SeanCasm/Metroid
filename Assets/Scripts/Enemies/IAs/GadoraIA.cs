using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Weapons;
using Enemy;
public class GadoraIA : EnemyBase
{
    [SerializeField]GameObject gadoraBlast;
    [SerializeField]Transform shootPoint;
    [SerializeField] Collider2D hurtbox;
    [SerializeField] int id;
    PlayerDetector playerDetector;
    private int[] attackPattern=new int[10];
    new void Awake()
    {
        base.Awake();
        enemyHealth=GetComponentInChildren<EnemyHealth>();
        playerDetector=GetComponentInChildren<PlayerDetector>();
        for(int i=0;i<attackPattern.Length;i++){
            attackPattern[i]=Random.Range(2,3);
        }
    }
    private new void OnEnable() {
        if(GameDataContainer.instance.GadoraExist(id)){

            Destroy(gameObject);
        }
        base.OnEnable();
        enemyHealth.OnDamage+=OnDamage;
    }
    private void OnDisable() {
        enemyHealth.OnDamage-=OnDamage;
    }

    private void OnDamage(){
        if(enemyHealth.MyHealth<=0){
            GameDataContainer.instance.AddGadora(id);
        }
        hurtbox.enabled=false;
        anim.SetBool("Blink", true);
        Invoke("OpenEye",attackPattern[Random.Range(0,attackPattern.Length)]);
    }
     
    public void OpenEye(){
        anim.SetBool("Blink", false);
        hurtbox.enabled=true;
    }
    public void Attack()
    {
        GameObject blast = Instantiate(gadoraBlast, shootPoint.position,transform.rotation);
        Weapon blastBullet = blast.GetComponent<Weapon>();
        blastBullet.Direction = transform.right;
    }
}
