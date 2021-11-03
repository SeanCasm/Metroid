using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy.Weapons{
    public class ShooterLook : Weapon
    {
        new void Start()
        {
            base.Start();
        }
        new void OnEnable()
        {
            base.OnEnable();
            direction=transform.right;
        }
        new void FixedUpdate() {
            base.FixedUpdate();
        }
        new void OnTriggerEnter2D(Collider2D other) {
            base.OnTriggerEnter2D(other);
        }
    }

}
 