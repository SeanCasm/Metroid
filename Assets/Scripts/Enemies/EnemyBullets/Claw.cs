using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Weapons;
public class Claw : Weapon,IDrop
{
    new void OnEnable() {
        base.SetDirection();  
    }
    new void FixedUpdate() {
        base.FixedUpdate();
    }
    new void Start() {
       base.Start(); 
    }
    new void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IPlayerWeapon>() != null)
        {
            base.DoDrop();
        }
        base.OnTriggerEnter2D(collision);
    }
}
