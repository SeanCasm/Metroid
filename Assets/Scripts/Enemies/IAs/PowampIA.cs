using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Weapons;
using Enemy;
public class PowampIA : EnemyBase
{
    [SerializeField]private LayerMask ground;
    [SerializeField]private float groundDistance,swimVelocity;
    [SerializeField]private GameObject bulletPrefab;
    private PlayerDetector pD;
    private bool facingUp,ignoring;
    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();
        pD = GetComponentInChildren<PlayerDetector>();
    }
    void Start()
    {
        facingUp = true;
    }
    Vector2 direction;
    private void Update()
    {
        if (facingUp)
        {
            direction = Vector2.up;
        }
        else
        {
            direction = Vector2.down;
        }

        if(Physics2D.Raycast(transform.position, direction, groundDistance, ground))
        {
            facingUp =!facingUp;
        }
    }
    private void FixedUpdate()
    {
        if (facingUp)
        {
            rigid.velocity = new Vector2(0f, swimVelocity);
        }
        else
        {
            rigid.velocity = new Vector2(0f,swimVelocity*-1f);
        }
    }
    void StopIgnore(){
        ignoring=false;
    }
    private void LateUpdate()
    {
        anim.SetBool("detected", pD.detected && !ignoring);
    }
    public void Shoot()
    {
        int degrees = 45;
        for(int i = 0; i < 8; i++)
        {
            var b= Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            var w= b.GetComponent<Weapon>();
            w.SetDirectionAround(degrees);
            degrees += 45;
        }
        ignoring=true;
        Invoke("StopIgnore",2.5f);
    }
}
