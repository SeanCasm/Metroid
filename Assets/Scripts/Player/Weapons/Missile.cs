using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Weapon;
namespace Player.Weapon
{
    public class Missile : Projectil, IRejectable
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
        new void OnDisable() {
            base.OnDisable();
        }
        new void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
            if(collision.tag=="Suelo" && beamType==WeaponType.SuperMissile){
                GameEvents.OnMissileImpact?.Invoke(transform.position);
            }
        }
        new void OnBecameInvisible()
        {
            base.OnBecameInvisible();
        }
        #endregion
        new void Reject()
        {
            base.Reject();
        }
    }
}
