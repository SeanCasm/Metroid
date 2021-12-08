using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Weapons;
using Enemy;
public class EspinaIA : EnemyBase
{
    private float currentSpeed;
    private GroundSlopeChecker efd;
    public GameObject bulletPrefab;
    private bool _isAttacking;
    private int upsideDown=1;
    GameObject[] bulletArray = new GameObject[5];
    Weapon[] bulletComponent = new Weapon[5];
    new void Awake()
    {
        base.Awake();
        upsideDown = (Vector2)transform.up == Vector2.up ? 1 : -1;
        efd = GetComponent<GroundSlopeChecker>();
    }
    private new void OnEnable() {
        base.OnEnable();
        pDetect.OnDetection+=PlayerDetected;
    }
    private void OnDisable(){
        pDetect.OnDetection-=PlayerDetected;
    }
    void Start()=>currentSpeed = speed;
    void Update()
    {
        if (_isAttacking) currentSpeed = 0f;
        else currentSpeed = speed;
    }
    void FixedUpdate()=>efd.SetOnGroundVelocity(currentSpeed);
    private void PlayerDetected(){
        anim.SetBool("Attack",_isAttacking=true);
    }
    public void Shoot()
    {
        for(int i = 0; i < 5; i++)
        {
            bulletArray[i]= Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;
            bulletComponent[i] = bulletArray[i].GetComponent<Weapon>();
        }
        int angle=0;
        for(int i=0;i<bulletComponent.Length;i++){
            bulletComponent[i].SetDirectionAround(angle);
            angle+=(45*upsideDown);
        }
        rigid.gravityScale = 1;
        anim.SetBool("Attack",_isAttacking=false);
    }
}
