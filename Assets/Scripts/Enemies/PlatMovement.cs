using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class PlatMovement : EnemyBase
{
    public float wallAware = 0.5f;
    public LayerMask WallLayer;
    private EnemyHealth health;
    // Movement
    private bool _facingRight;
    Vector2 direction;
    void Start()
    {
        health = GetComponentInChildren<EnemyHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!health.freezed)
        {
            if (transform.localScale.x < 0f)
            {
                _facingRight = false;
            }
            else if (transform.localScale.x > 0f)
            {
                _facingRight = true;
            }

            if (!_facingRight)
            {
                direction = Vector2.left;
            }
            else
            {
                direction = Vector2.right;
            }

            if (Physics2D.Raycast(transform.position, direction, wallAware, WallLayer))
            {
                Flip();
            }
        }
         
    }
    void FixedUpdate()
    {
        if (!health.freezed)
        {
            float horizontalVelocity = speed;

            if (!_facingRight)
            {
                horizontalVelocity = horizontalVelocity * -1f;
            }

            rigid.velocity = new Vector2(horizontalVelocity, rigid.velocity.y);
        }
         
    }
    void Flip()
    {
        _facingRight = !_facingRight;
        float localScaleX = transform.localScale.x;
        localScaleX = localScaleX * -1f;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }
}
