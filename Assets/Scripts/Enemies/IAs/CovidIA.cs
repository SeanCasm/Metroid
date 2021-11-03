using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
public class CovidIA : EnemyBase
{
    private BoxCollider2D boxCol;
    private PlayerDetector pD;
    bool moving, detected;
    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();
        boxCol = GetComponent<BoxCollider2D>();
        pD = GetComponentInChildren<PlayerDetector>();
    }
    // Update is called once per frame
    void Update()
    {

        if (pD.detected)
        {
            detected = moving = true;
            transform.position = Vector3.MoveTowards(transform.position, pD.player.transform.position, speed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Suelo") && moving && collision.IsTouching(boxCol))
        {
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {
        if (moving) rigid.velocity = new Vector2(0f, -speed * 2f);
    }
    private void LateUpdate()
    {
        anim.SetBool("EnemyDetected", detected);
    }
}
