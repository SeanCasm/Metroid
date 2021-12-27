using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Enemy
{
    public class GroundSlopeChecker : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] float wallDistance;
        [SerializeField] float wallEdgeOffset;
        [SerializeField] LayerMask groundLayer;
        [SerializeField, Range(.01f, 1f)] float slopeFrontRay = 0.08f;
        [SerializeField, Range(.01f, 1f)] float groundHitSlope;
        [SerializeField] CapsuleCollider2D coll;
        [SerializeField] bool canFlip;
        private Vector2 slopePerp, posFrontRay;
        private RaycastHit2D frontHit, slopeHit,midHit;
        private float slopeAngle, frontAngle;
        private bool facingRight, wallInFront, onSlope;
        private float spriteWitdh;
        public bool FacingRight { get { return facingRight; } }
        public float xVelocity { get; private set; }
        bool onGround;
        private SpriteRenderer spriteRenderer;
        public bool OnGround { get { return onGround; } }
        public LayerMask GroundLayer => groundLayer;
        public float dir { get; private set; }
        private EnemyBase enemyBase;
        private Rigidbody2D rigid;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            enemyBase = GetComponent<EnemyBase>();
            spriteWitdh = spriteRenderer.bounds.extents.x;
            rigid = GetComponent<Rigidbody2D>();
            if (transform.eulerAngles.y == 0) { dir = 1; facingRight = true; }
            else { facingRight = false; dir = -1; }
            enabled = false;
        }
        void Update()
        {
            CheckGround();
            CheckWall();
            if (wallInFront) Flip();
        }
        private void CheckGround()
        {
            RaycastHit2D hit2D = Physics2D.Raycast(new Vector2(coll.bounds.center.x + (coll.size.x / 2) * dir, coll.bounds.center.y), -transform.up, groundHitSlope, groundLayer);
            if (!hit2D)
            {
                onGround = false;
                Flip();
            }
            else{
                onGround = true;
                OnSlope();
            } 

            //Debug.DrawRay(new Vector2(coll.bounds.center.x + (coll.size.x / 2) * dir, coll.bounds.center.y),-transform.up*groundHitSlope,Color.green);
        }
        private void CheckWall()
        {
            RaycastHit2D wallHit = Physics2D.BoxCast(new Vector2(coll.bounds.center.x + (coll.size.x / 2) * dir,
            coll.bounds.center.y),
            new Vector2(wallDistance, coll.bounds.size.y - wallEdgeOffset), 0f, new Vector2(dir, 0), wallDistance, groundLayer);
            if (wallHit && Vector2.Angle(wallHit.normal, Vector2.up) == 90)
            {
                wallInFront = true;
            }
            else wallInFront = false;
        }
        public void Flip()
        {
            facingRight = !facingRight;
            dir = facingRight ? 1 : -1;
            transform.position = new Vector3(transform.position.x + (spriteWitdh / 2) * dir, transform.position.y, 0);
            if(canFlip) transform.RotateAround(transform.position, transform.up, 180);
            wallInFront = false;
        }
        private void OnSlope()
        {
            posFrontRay = new Vector2((coll.bounds.center.x + (coll.bounds.size.x/2)*dir), coll.bounds.center.y);

            frontHit = Physics2D.Raycast(posFrontRay, Vector2.down, slopeFrontRay, groundLayer);
            RaycastHit2D slopeHit = Physics2D.Raycast(new Vector2(coll.bounds.min.x + coll.size.x / 2,  coll.bounds.center.y), Vector2.down, groundHitSlope, groundLayer);
            midHit = Physics2D.Raycast(new Vector2(coll.bounds.min.x+coll.size.x/2, coll.bounds.min.y+ .05f), 
            Vector2.down, groundHitSlope, groundLayer);
            if(frontHit && midHit){
                slopeAngle = Vector2.Angle(midHit.normal, Vector2.up);
                frontAngle = Vector2.Angle(frontHit.normal, Vector2.up);
                slopePerp = Vector2.Perpendicular(frontHit.normal).normalized;

                if ((slopePerp.y < 0 && dir < 0) || (slopePerp.y > 0 && dir > 0)) frontAngle *= -1;
                if ((frontAngle > 0) || (frontAngle == 0 && slopeAngle != 0) || slopeAngle != 0)
                {
                    onSlope = true;
                }
                else {
                    onSlope = false;
                }
            }
        }
        public void SetOnGroundVelocity(float speed)
        {
            xVelocity = dir * speed * Time.deltaTime;
            rigid.velocity = (!onSlope) ? new Vector2(xVelocity, 0f) : new Vector2(-xVelocity * slopePerp.x, -xVelocity * slopePerp.y);
            if ((frontAngle == 0 && slopeAngle != 0) && (frontHit.point.y > slopeHit.point.y))
            {
                rigid.velocity.Set(xVelocity, 0);
            }
        }
        private void OnBecameInvisible()
        {
            enemyBase.OnBecameInvisible();
            enabled = false;
        }
        private void OnBecameVisible()
        {
            enemyBase.OnBecameVisible();
            enabled = true;
        }
    }
}
