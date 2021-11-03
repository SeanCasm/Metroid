using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Weapons;
public class IceRay : LookPlayerFirst
{
    new void Start()=>base.Start();
    new void FixedUpdate()=>base.FixedUpdate();
    new void OnEnable()
    {
        base.OnEnable();
        base.SetDirectionAndRotationLimit(angleLimit);
    }
    new private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            IFreezeable ifreezeable=other.GetComponentInParent<IFreezeable>();
            ifreezeable.FreezeMe();
        }
    }
    new private void OnBecameInvisible() {
        base.OnBecameInvisible();
    }
}
