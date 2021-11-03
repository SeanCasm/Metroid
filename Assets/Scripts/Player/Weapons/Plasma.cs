using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player.Weapon
{
    public class Plasma : Projectil
    {
        #region Unity methods
        new private void Awake()
        {
            base.Awake();
        }
        new void OnEnable() {
            base.OnEnable();
        }
        new void FixedUpdate()
        {
            base.FixedUpdate();
        }
        new void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                health = collision.GetComponent<IDamageable<float>>();
                iInvulnerable = collision.GetComponent<IInvulnerable>();
                if (health == null && iInvulnerable != null) Reject();
                if (health != null && iInvulnerable != null)
                {
                    TryDoDamage(damage, health, beamType, iInvulnerable);
                    if (rejected) Reject();
                }
            }
            else if ((collision.IsTouching(floorCol) && collision.tag == "Suelo")) FloorCollision();
        }
        new void OnBecameInvisible()
        {
            base.OnBecameInvisible();
        }
        #endregion 
    }
}
