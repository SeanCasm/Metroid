using System.Collections;
using System.Collections.Generic;
using Player.Weapon;
using UnityEngine;

public class BeamCharged : Projectil
{
    new void OnEnable()
    {
        Invoke("BackToGun", livingTime);
        OverHeatBar.SetFill.Invoke(hotPoints);
        direction = transform.parent.right;
        transform.eulerAngles = transform.parent.eulerAngles;
    }
    new private void Awake()
    {
        base.Awake();
    }
    new void FixedUpdate()
    {
        if (!rejected) rigid.velocity = direction.normalized * speed;
    }
    new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
