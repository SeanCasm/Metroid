using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Weapons;
using Enemy;
public class HornadIA : EnemyBase
{
    [SerializeField] float wallAware;
    private Jumper jumper;
    public float jumpForce = 2f;

    public LayerMask wallLayer;
    public GameObject bulletPrefab;
    public Transform _firePoint;
    private SpriteRenderer spriteRenderer;
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
        spriteRenderer=GetComponent<SpriteRenderer>();
        jumper = GetComponent<Jumper>();
    }
    void Start()
    {
        if (transform.localScale.x > 0) { jumper.facingRight = true; }
        else { jumper.facingRight = false; }
    }
    void Update()
    {
        //Debug.DrawRay(transform.position,transform.right*wallAware,Color.blue);
        if (Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y + spriteRenderer.size.y / 2), transform.right, wallAware, wallLayer)) SwapDirection();
        if (pDetect.detected)
        {
            if (pDetect.player.transform.position.x < transform.position.x && jumper.facingRight) SwapDirection();
            else if (pDetect.player.transform.position.x > transform.position.x && !jumper.facingRight) SwapDirection();
        }

        if (!IsInvoking("RandomMovement") && !pDetect.detected) Invoke("RandomMovement", 2f);
        if (pDetect.detected){
            anim.SetTrigger("Moving");
        }
    }
    private void SwapDirection()
    {
        jumper.Flip();
        speed *= -1;
    }
    private void OnJump()
    {
        rigid.gravityScale = 0;
    }
    private void OutJump()
    {
        rigid.gravityScale = 1;
    }
    void RandomMovement()
    {
        int i = Random.Range(1, 5);
        if (i == 1 || i == 2)
        {
            anim.SetTrigger("Moving");
        }
    }
    void LateUpdate()
    {
        anim.SetBool("DetectedClose", pDetect.detected);
    }
    private void FixedUpdate()
    {
        if (jumper.doJump)
        {
            rigid.SetVelocity(speed * Time.deltaTime, jumpForce * Time.deltaTime);
        }
    }

    public void Shoot()
    {
        GameObject myBullet = Instantiate(bulletPrefab, _firePoint.position, Quaternion.identity) as GameObject;
        Throw bulletComponent = myBullet.GetComponent<Throw>();
        bulletComponent.ThrowPrefab(_firePoint, pDetect.GetPlayerTransformCenter());
    }
}
