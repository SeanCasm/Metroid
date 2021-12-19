using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Weapons;
using Enemy;
public class ScizerIA : EnemyBase
{
    [SerializeField] GameObject scizerBulltet;
    [SerializeField] Transform[] shootPoints;
    Throw pF;
    private GroundSlopeChecker efd;
    bool attacking;
    private new void OnEnable()
    {
        base.OnEnable();
        pDetect.OnDetection += SetAttack;
        pDetect.OnOut += StopAttack;
    }
    private void OnDisable()
    {
        pDetect.OnDetection -= SetAttack;
        pDetect.OnOut -= StopAttack;
    }
    new void Awake()
    {
        base.Awake();
        efd = GetComponent<GroundSlopeChecker>();
    }
    private void FixedUpdate()
    {
        if (!attacking) efd.SetOnGroundVelocity(speed);
        else rigid.SetVelocity(0f, 0f);
    }
    public void Attack()
    {
        if (pDetect.GetPlayerTransformCenter().x < transform.position.x) ThrowPrefab(1);
        else if (pDetect.GetPlayerTransformCenter().x > transform.position.x) ThrowPrefab(0);
    }
    private void ThrowPrefab(int shootPoint)
    {
        GameObject mb = Instantiate(scizerBulltet, shootPoints[shootPoint].position, Quaternion.identity) as GameObject;
        pF = mb.GetComponent<Throw>();
        pF.ThrowPrefab(shootPoints[shootPoint], pDetect.GetPlayerTransformCenter());
    }
    private void SetAttack()
    {
        attacking = true;
        anim.SetBool("Attacking", true);
    }
    private void StopAttack()
    {
        attacking = false;
        anim.SetBool("Attacking", false);
    }
}