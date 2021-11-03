using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
public class ConchaIA : EnemyBase
{
    [SerializeField]Vector2 direction;
    private float currentSpeed;
    Vector2 lastVelocity;
    new void Awake()
    {
        base.Awake();
        currentSpeed = speed;
    }
    void FixedUpdate()
    {
        lastVelocity = rigid.velocity;
        rigid.velocity = direction.normalized * currentSpeed;
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Suelo")
        {
            var speed = lastVelocity.magnitude;
            direction = Vector2.Reflect(lastVelocity.normalized, col.contacts[0].normal);
            rigid.SetVelocity(direction * Mathf.Max(currentSpeed, 0f));
            transform.eulerAngles=(direction.x<0)?new Vector3(0, 180, 0):new Vector3(0, 0, 0);
        }
    }
}
