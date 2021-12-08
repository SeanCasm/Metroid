using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Enemy;
using System;

public class PirateIA : EnemyBase
{
    #region Properties
    [SerializeField] Enemy.Weapon.Pool bulletsPool;
    [SerializeField] float minAltitude;
    private float currentSpeed, horizontalVelocity;
    private GroundSlopeChecker efd;
    private bool idleShooting;
    #endregion
    #region Unity Methods
    new void Awake()
    {
        base.Awake();
        currentSpeed = speed;
        enemyHealth = GetComponentInChildren<EnemyHealth>();
        efd = GetComponent<GroundSlopeChecker>();
    }
    private new void OnEnable()
    {
        base.OnEnable();
        enemyHealth.OnSideDamage += OnDamage;
    }
    private void OnDisable()
    {
        enemyHealth.OnSideDamage -= OnDamage;
    }
    void Update()
    {
        if (pDetect.detected && efd.OnGround)
        {
            if (pDetect.GetPlayerTransformCenter().x < transform.position.x)
            {
                if (efd.FacingRight) { efd.Flip(); }
            }
            else if (pDetect.GetPlayerTransformCenter().x > transform.position.x)
            {
                if (!efd.FacingRight) { efd.Flip(); }
            }
            if (!idleShooting)
            {
                if (pDetect.GetPlayerTransformCenter().y >= transform.position.y + minAltitude)
                {
                    anim.SetTrigger("Shoot D up");
                }
                else if (pDetect.GetPlayerTransformCenter().y < transform.position.y - minAltitude)
                {
                    anim.SetTrigger("Shoot D down");
                }
                else anim.SetTrigger("Shoot");

                idleShooting = true;
                Invoke("StartCheck", 2f);
            }
        }
        else if (!pDetect.detected) horizontalVelocity = speed;
    }
    private void LateUpdate() => anim.SetBool("Idle", pDetect.detected);
    private void FixedUpdate()
    {
        if (pDetect.detected) { rigid.SetVelocity(0f, 0f); rigid.gravityScale = 0; }
        else if (!pDetect.detected && !idleShooting)
        {
            //rigid.gravityScale = 1;
            if (pDetect.detected) efd.SetOnGroundVelocity(horizontalVelocity * 2f);
            else efd.SetOnGroundVelocity(speed);
        }
    }

    #endregion
    private void OnDamage(float side)
    {
        if (!pDetect.detected)
        {
            if (side < transform.position.x && efd.FacingRight)
            {
                efd.Flip();
            }
            else if (side > transform.position.x && !efd.FacingRight)
            {
                efd.Flip();
            }
        }
    }

    void StartCheck() => idleShooting = false;
    public void ShootEvent()
    {
        foreach (Transform element in bulletsPool.ShootPoint)
        {
            bulletsPool.ActiveNextPoolObject();
        }
    }
}