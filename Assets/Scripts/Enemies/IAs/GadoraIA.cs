using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Weapons;
using Enemy;
public class GadoraIA : EnemyBase
{
    [SerializeField]BoxCollider2D hurtBox;
    [SerializeField]GameObject gadoraBlast,deadSound,deadPrefab;
    [SerializeField]Transform shootPoint;
    private EnemyHealth eHealth;
    private int[] attackPattern=new int[10];
    private int attackIndex=0;
    new void Awake()
    {
        base.Awake();
        eHealth=GetComponentInChildren<EnemyHealth>();
        for(int i=0;i<attackPattern.Length;i++){
            attackPattern[i]=Random.Range(2,4);
        }

        eHealth.OnDamage+=OnDamage;
    }
    private void OnDisable() {
        eHealth.OnDamage-=OnDamage;
    }
    private void OnDamage(){
        anim.SetBool("Blink", true);
        hurtBox.enabled=false;
        Invoke("OpenEye",attackPattern[attackIndex]);
        attackIndex++;
        if(attackIndex>9)attackIndex=0;
    }
     
    public void OpenEye(){
        anim.SetBool("Blink", false);
    }
    public void EnableHurtbox(){
        hurtBox.enabled = true;
    }
    public void Attack()
    {
        GameObject blast = Instantiate(gadoraBlast, shootPoint.position,transform.rotation);
        Weapon blastBullet = blast.GetComponent<Weapon>();
        blastBullet.Direction = transform.right;
    }
}
