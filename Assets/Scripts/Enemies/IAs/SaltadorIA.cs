using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
public class SaltadorIA : EnemyBase
{
    [SerializeField] LayerMask groundLayer;
    public float jumpForce = 2f;
    private Jumper jumper;
    private bool grounded = true;
    private new void OnEnable()
    {
        base.OnEnable();
        jumper.OnJump += OnJump;
        jumper.OutJump += OutJump;
    }
    private void OnDisable()
    {
        jumper.OnJump -= OnJump;
        jumper.OutJump -= OutJump;
    }

    new void Awake()
    {
        base.Awake();
        jumper = GetComponent<Jumper>();
    }
    void Start()
    {
        if (transform.localScale.x > 0) { jumper.facingRight = true; }
        else { jumper.facingRight = false; }
    }
    private void Update()
    {
        if (Physics2D.Raycast(transform.position, transform.right, 0.2f, groundLayer))
        {
            jumper.Flip();
            speed *= -1;
        }
        if(pDetect.detected){
            if (grounded) anim.SetTrigger("Jump");
            if(pDetect.player.transform.position.x<transform.position.x && jumper.facingRight){
                jumper.Flip();speed*=-1;
            }
        }

        if (!IsInvoking("RandomMovement") && !pDetect.detected && grounded) Invoke("RandomMovement", 2f);
    }
    void RandomMovement()
    {
        int i = Random.Range(1, 3);
        if (i == 1 || i == 3)
        {
            anim.SetTrigger("Jump");
        }
    }
    private void OnJump()
    {
        rigid.gravityScale = 0;
    }
    private void OutJump()
    {
        rigid.gravityScale = 1;
    }
    private void FixedUpdate()
    {
        if (jumper.doJump)
        {
            rigid.SetVelocity(speed * Time.deltaTime, jumpForce * Time.deltaTime);
        }
    }
    void groundDetector()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down, 0.4f, groundLayer)) grounded = true;
        else grounded = false;
    }
}