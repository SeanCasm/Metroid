using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using Enemy.Weapons;
public class CovidIA : EnemyBase
{
    [SerializeField] GameObject projectil;
    [SerializeField] Collider2D detector;
    private float currentSpeed;
    private PlayerDetector pD;
    bool moving, detected;
    // Start is called before the first frame update
    new void Awake()
    {
        currentSpeed=speed;
        base.Awake();
        pD = GetComponentInChildren<PlayerDetector>();
    }
    // Update is called once per frame
    void Update()
    {

        if (pD.detected)
        {
            detected = moving = true;
            transform.position = Vector3.MoveTowards(transform.position, pD.player.transform.position, speed * Time.deltaTime);
            detector.enabled=false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Suelo") && moving)
        {
            speed = 0f;
            Invoke(nameof(Shoot),.7f);
        }
    }
    private void FixedUpdate()
    {
        if (moving) rigid.velocity = new Vector2(0f, -speed * 2f);
    }
    private void Shoot()
    {
        int degrees = 0;
        for (int i = 0; i < 5; i++)
        {
            var b = Instantiate(projectil, transform.position, Quaternion.identity);
            var w = b.GetComponent<Weapon>();
            w.SetDirectionAround(degrees);
            degrees += 45;
        }
        Invoke("Hide",1.2f);
    }
    private void Hide(){
        speed=currentSpeed/2.5f;
        Destroy(gameObject,1.8f);
    }
    private void LateUpdate()
    {
        anim.SetBool("EnemyDetected", detected);
    }
}
