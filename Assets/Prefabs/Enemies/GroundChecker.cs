using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{
    public class GroundChecker : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] float wallAware, floorAware;
        [SerializeField] LayerMask groundLayer;
        private RaycastHit2D wallHit;
        private bool facingRight, wallInFront;
        private float spriteWitdh, wallAngle;
        public bool FacingRight { get { return facingRight; } }
        public float xVelocity { get; private set; }
        bool onGround;
        public float dir { get; private set; }
        private Rigidbody2D rigid;

        private void Start()
        {
            spriteWitdh = GetComponent<SpriteRenderer>().bounds.extents.x;
            rigid = GetComponent<Rigidbody2D>();
            if (transform.eulerAngles.y == 0) { dir = 1; facingRight = true; }
            else { facingRight = false; dir = -1; }
        }
        void Update()
        {
            if (!Physics2D.Raycast(transform.position, -transform.up, floorAware, groundLayer))
            {
                onGround = false;
                Flip();
            }
            else onGround = true;
            CheckWall();
            if (wallInFront) Flip();
        }
        private void CheckWall()
        {
            wallHit = Physics2D.Raycast(transform.position, transform.right, wallAware, groundLayer);
            Debug.DrawRay(transform.position, transform.right * wallAware, Color.red);
            if (wallHit)
            {
                wallAngle = Vector2.Angle(wallHit.normal, Vector2.up);
                wallInFront = wallAngle >= 90 ? true : false;
            }
        }
        public void Flip()
        {
            float yRot;
            facingRight = !facingRight;
            if (facingRight)
            {
                transform.position = new Vector3(transform.position.x + spriteWitdh / 2, transform.position.y, 0);
                yRot = 0;
                dir = 1;
            }
            else
            {
                transform.position = new Vector3(transform.position.x - spriteWitdh / 2, transform.position.y, 0);
                yRot = 180;
                dir = -1;
            }
            wallInFront = false;
            transform.eulerAngles = new Vector3(0, yRot, 0);
        }
        public void SetOnGroundVelocity(float speed)
        {
            xVelocity = dir * speed * Time.deltaTime;
            rigid.velocity = new Vector2(xVelocity, 0f);
        }
    }
}

