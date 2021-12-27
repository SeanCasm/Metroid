using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
public class ConchaIA : EnemyBase
{
    [SerializeField] Vector2 direction;
    [SerializeField] LayerMask groundLayer;
    private float currentSpeed;
    new void Awake()
    {
        base.Awake();
        currentSpeed = speed;
    }
    void Update()
    {
        RaycastHit2D raycastHit2D = Physics2D.CircleCast(transform.position, .1f, Vector2.zero, 0, groundLayer);
        if (raycastHit2D)
        {
            direction = Vector2.Reflect(direction, raycastHit2D.normal);
        }
        transform.eulerAngles = (direction.x < 0) ? new Vector3(0, 180, 0) : new Vector3(0, 0, 0);

    }
    private void FixedUpdate() {
        rigid.velocity = direction*currentSpeed*Time.deltaTime;
    }

}
