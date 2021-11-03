using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
public class FishIA : EnemyBase
{
    [SerializeField]private float playerIgnoringTime;
    private Vector3 spawnPoint;
    Vector2 direction,playerDirection;
    private bool flip,facingRight,ignorePlayer;
    public float swimVelocity,swimToPlayer;
    private float velocity;
    public float groundDistance;
    public LayerMask groundLayer,playerLayer;
    new void Awake()
    {
        base.Awake();
        velocity = swimVelocity;
    }
    private void Start()
    {
        spawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (flip && !IsInvoking("StopFlip")) Invoke("StopFlip", 0.4f);
        if (transform.localScale.x < 0f)
        {
            facingRight = false;
            direction = Vector2.left;
            playerDirection = Vector2.right;
        }
        else if (transform.localScale.x > 0f)
        {
            facingRight = true;
            direction = Vector2.right;
            playerDirection = Vector2.left;
        }
        
        if (Physics2D.Raycast(transform.position, direction, groundDistance, groundLayer))
        {
            swimVelocity = 0f;
            limitReached = false;
            flip = true;
        }
        
        if (pDetect.detected && !ignorePlayer && !limitReached) 
        {
            transform.position = Vector3.MoveTowards(transform.position, pDetect.GetPlayerTransformCenter(), swimToPlayer * Time.deltaTime) ;
            if (facingRight && pDetect.player.transform.position.x < transform.position.x)
            {
                flip = true;
            }else if(!facingRight && pDetect.player.transform.position.x > transform.position.x)
            {
                flip = true;
            }
             
        }
        if(pDetect.player != null)
        {
            if (transform.position == pDetect.GetPlayerTransformCenter())
            {
                ignorePlayer = true;
                if (!IsInvoking("NoIgnorePlayer")) Invoke("NoIgnorePlayer", playerIgnoringTime);
            }
        }
        if (limitReached && !IsInvoking("NoLimitReached"))Invoke("NoLimitReached", 2.5f);
    }
    void NoLimitReached()
    {
        limitReached = false;
    }
    bool limitReached;
    public bool LimitReached { get { return limitReached; } set { limitReached = value; } }
    void NoIgnorePlayer()
    {
        ignorePlayer=false;
    }
    void StopFlip()
    {
        flip = false;
        swimVelocity = velocity;
        Flip();
    }
    
    private void FixedUpdate()
    {
        if (flip) rigid.velocity = new Vector2(0f, rigid.velocity.y);
        else rigid.velocity = new Vector2(velocity,0f);
    }
    private void LateUpdate()
    {
         anim.SetBool("Flip", flip);
    }
    public IEnumerator BackToSpawn()
    {
        while (transform.position != spawnPoint)
        {
            yield return new WaitForSeconds(0.2f);
            transform.position = Vector3.MoveTowards(transform.position, spawnPoint, swimToPlayer * Time.deltaTime);
        }
    }
    private void Flip()
    {
        facingRight =! facingRight;
        float localScaleX = transform.localScale.x;
        localScaleX = localScaleX * -1f;
        velocity *= -1;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }
}
