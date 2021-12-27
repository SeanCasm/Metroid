using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Weapons;
using Enemy;
public class GadoraIA : EnemyBase
{
    [SerializeField]GameObject gadoraBlast;
    [SerializeField]Transform shootPoint;
    private EnemyHealth eHealth;
    PlayerDetector playerDetector;
    private int[] attackPattern=new int[10];
    new void Awake()
    {
        base.Awake();
        eHealth=GetComponentInChildren<EnemyHealth>();
        playerDetector=GetComponentInChildren<PlayerDetector>();
        for(int i=0;i<attackPattern.Length;i++){
            attackPattern[i]=Random.Range(2,3);
        }

    }
    private new void OnEnable() {
        base.OnEnable();
        eHealth.OnDamage+=OnDamage;
    }
    private void OnDisable() {
        eHealth.OnDamage-=OnDamage;
    }
    private void OnDamage(){
        anim.SetBool("Blink", true);
        Invoke("OpenEye",attackPattern[Random.Range(0,attackPattern.Length)]);
    }
     
    public void OpenEye(){
        anim.SetBool("Blink", false);
    }
    public void Attack()
    {
        GameObject blast = Instantiate(gadoraBlast, shootPoint.position,transform.rotation);
        Weapon blastBullet = blast.GetComponent<Weapon>();
        blastBullet.Direction = transform.right;
    }
}
