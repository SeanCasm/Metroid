using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using Player.Weapon;

public class MetroidIA : EnemyBase
{
    [Header("Configuration")]
    [SerializeField] float damageDelay;
    [SerializeField] float sideRotation, catchRadius;
    [SerializeField] Transform catchPlayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Collider2D hurtbox;
    private bool onPlayer, followPlayer, startChecks;
    private PlayerHealth pHealth;
    private new void Awake()
    {
        base.Awake();
        pHealth = FindObjectOfType<PlayerHealth>();
        anim.speed = 0;
    }
    private new void OnEnable()
    {
        base.OnEnable();
        pDetect.OnDetection += EnableComp;
    }
    private void OnDisable()
    {
        pDetect.OnDetection -= EnableComp;
    }
    private void Update()
    {
        if (startChecks)
        {
            var hit = Physics2D.OverlapCircle(catchPlayer.position, catchRadius, playerLayer);
            onPlayer = hit != null ? true : false;
            if (onPlayer && !IsInvoking(nameof(Damage)))
            {
                InvokeRepeating(nameof(Damage), damageDelay, damageDelay);
                anim.SetFloat("Anim speed", 2f);
            }
            else if (!onPlayer)
            {
                CancelInvoke(nameof(Damage));
                anim.SetFloat("Anim speed", 1f);
            }

            if ((pDetect.detected || followPlayer) && !onPlayer)
            {
                followPlayer = true;
                if (transform.position.x < pDetect.GetPlayerTransformCenter().x)
                {
                    transform.rotation = Quaternion.Euler(0, 0, -sideRotation);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 0, sideRotation);
                }
                transform.position = Vector2.MoveTowards(transform.position, pDetect.GetPlayerTransformCenter(), speed * Time.deltaTime);
            }
            else if (onPlayer)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.position = pDetect.GetPlayerTransformCenter();
            }
        }
    }
    /*private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(catchPlayer.position,catchRadius);
    }*/
    private void EnableComp()
    {
        anim.speed = 1;
        startChecks = true;
        hurtbox.enabled=true;
    }
    void Damage()
    {
        pHealth.ConstantDamage(enemyHealth.collideDamage);
    }
}