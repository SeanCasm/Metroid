using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Weapons;
public class IceRay : Weapon
{
    [Tooltip("Represents the limit (negative and positive) of the angle when this object looks right toward the player")]
    [SerializeField, Range(-1f, 45f)] protected float angleLimit;
    new void OnEnable() {
        base.OnEnable();
        base.SetDirectionAndRotationLimit(angleLimit);
    }
    new private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            IFreezeable ifreezeable=other.GetComponentInParent<IFreezeable>();
            ifreezeable.FreezeMe();
            BackToShootPoint();
        }else if (other.CompareTag("Suelo")){
            BackToShootPoint();
        }
    }
}
