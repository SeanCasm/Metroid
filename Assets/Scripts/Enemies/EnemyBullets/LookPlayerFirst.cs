using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy.Weapons{
    public class LookPlayerFirst : Weapon
    {
        [Tooltip("Represents the limit (negative and positive) of the angle when this object looks right toward the player")]
        [SerializeField, Range(-1f, 45f)] protected float angleLimit;
       
        new void Start()
        {
            base.Start();
        }
        new void OnEnable() {
            base.OnEnable();
            base.SetDirectionAndRotationLimit(angleLimit);
        }
        new void OnTriggerEnter2D(Collider2D other) {
            base.OnTriggerEnter2D(other);
        }
        new void OnBecameInvisible() {
            base.OnBecameInvisible();
        }
        new void FixedUpdate() {
           base.FixedUpdate(); 
        }
    }
}
 
