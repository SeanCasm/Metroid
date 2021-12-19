using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Enemy.Weapons;
public class Fireball : LookPlayerFirst
{
    [SerializeField]private GameObject groundFireBall;
    [SerializeField] float groundFireBallTime;
    new void Awake()
    {
        base.OnEnable();
        transform.SetParent(null);
        base.SetDirectionAndRotation();
    }
    new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("Suelo"))
        {
            GameObject gF=Instantiate(groundFireBall, transform.GetChild(0).transform.position, Quaternion.identity,null);
            Destroy(gF,groundFireBallTime);
            transform.SetParent(gF.transform);
            gameObject.SetActive(false);
        }
    }

}
