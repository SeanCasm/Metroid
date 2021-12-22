using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] CapsuleCollider2D capsule;
        [SerializeField] BoxCollider2D edgeCollider;
        [SerializeField] PlayerController playerController;

        [Header("Floor config")]
        [SerializeField] LayerMask groundLayer;
        [SerializeField, Range(.001f, .22f)] float slopeFrontRay = 0.08f, slopeBackRay = 0.08f, groundHitSlope;
        [SerializeField, Range(-1, 1.5f)] float slopeEdgesOffset;
        [SerializeField] float wallDistance, wallEdgeOffset, edgesOffset, spinOffset;
        [SerializeField, Range(.001f, .88f)] float groundDistance = 0.18f, airGroundDistance = 0.18f;
        public System.Action<float> OnGroundLanding;
        Rigidbody2D rb;
        private RaycastHit2D frontHit, backHit;
        private bool onSlope, rbStatic, isLanding;
        float frontAngle = 0, slopeAngle = 0, backAngle = 0, curGroundDis, curSpinOffset = 1;
        private Vector2 posFrontRay, posBackRay, slopePerp, direction;
        public bool OnSlope { get => onSlope; set => onSlope = value; }
        public bool checkFloor { get; set; } = true;
        public bool isGrounded { get; set; }
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            curGroundDis = groundDistance;
        }
        public void FixedUpdateOnGround()
        {
            rb.velocity = (!onSlope) ? new Vector2(playerController.XVelocity, 0f) : new Vector2(-playerController.XVelocity * slopePerp.x, -playerController.XVelocity * slopePerp.y);
            if ((frontAngle == 0 && backAngle != 0 && slopeAngle == 0) && ((frontHit.point.y > backHit.point.y) || (frontHit.point.y < backHit.point.y)))
            {
                transform.position.Set(frontHit.point.x, frontHit.point.y, 0);
                playerController.SetVelocity(new Vector2(rb.velocity.x, 0f));
            }
        }
        public bool SetFloorChecking()
        {
            if (checkFloor) CheckGround();

            return isGrounded;
        }
        private void CheckGround()
        {
            RaycastHit2D raycastHit2D = Physics2D.BoxCast(new Vector2(capsule.bounds.center.x, capsule.bounds.min.y),
            new Vector2(capsule.bounds.size.x / edgesOffset, curGroundDis), 0f,
            Vector2.down, curGroundDis * curSpinOffset, groundLayer);
            isGrounded = raycastHit2D;

            if (raycastHit2D) OnGround(raycastHit2D);
            else { isLanding = false; curGroundDis = airGroundDistance; }

        }
        public void OnJumping()
        {
            checkFloor = isGrounded = false;
            CancelAndInvoke(nameof(CheckFloor), .3f);
        }
        public void ResetState()
        {
            checkFloor = isGrounded = false;
        }
        public void CheckSlopesAndEdges()
        {
            Vector2 v = new Vector2(capsule.bounds.min.x + capsule.size.x / 2, capsule.bounds.min.y);
            posFrontRay = new Vector2(transform.position.x + capsule.size.x / 2 * (direction.x - slopeEdgesOffset), capsule.bounds.min.y + .02f);
            RaycastHit2D hit2D = Physics2D.Raycast(posFrontRay, Vector2.down, groundHitSlope, groundLayer);
            RaycastHit2D hit = Physics2D.Raycast(v, Vector2.down, groundHitSlope, groundLayer);

            if (hit2D && hit)
            {
                frontAngle = Vector2.Angle(hit2D.normal, Vector2.up);
                slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                slopePerp = Vector2.Perpendicular(hit2D.normal).normalized;
                if ((slopePerp.y < 0 && playerController.xInput < 0) || (slopePerp.y > 0 && playerController.xInput > 0)) frontAngle *= -1;

                if ((frontAngle != 0) || (frontAngle == 0 && slopeAngle != 0) || slopeAngle != 0)
                    onSlope = true;
                else if (frontAngle == 0 && slopeAngle == 0 && backAngle == 0)
                {
                    onSlope = false;
                }

            }

            posBackRay = new Vector2(transform.position.x - capsule.size.x / 2 * (direction.x - slopeEdgesOffset), capsule.bounds.min.y + .02f);

            frontHit = Physics2D.Raycast(posFrontRay, Vector2.down, slopeFrontRay, groundLayer);
            backHit = Physics2D.Raycast(posBackRay, Vector2.down, slopeBackRay, groundLayer);
            if (frontHit && backHit)
            {
                frontAngle = Vector2.Angle(frontHit.normal, Vector2.up);
                backAngle = Vector2.Angle(backHit.normal, Vector2.up);
            }

            if ((!frontHit || !backHit) && !hit && !onSlope)
            { // enable the edge collider when player is on ground edge
                edgeCollider.enabled = true;
                edgeCollider.transform.localPosition = new Vector3(0, 0.075f / 2);
            }
            else edgeCollider.enabled = false;
        }
        public void ResetGravity()
        {
            if (onSlope) rb.gravityScale = 0;
            else rb.gravityScale = 1;
        }
        private void OnGround(RaycastHit2D raycastHit2D)
        {
            if (!isLanding) OnGroundLanding.Invoke(raycastHit2D.point.y);

            if (onSlope && playerController.xInput == 0 && !rbStatic)
            {
                transform.position = new Vector3(transform.position.x, raycastHit2D.point.y, 0);
                rb.gravityScale = 0;
                rbStatic = true;
            }
            else if (!onSlope || playerController.xInput != 0)
            {
                rbStatic = false;
                rb.gravityScale = 1 / playerController.Slow2Gravity;
            }
            curGroundDis = groundDistance;
            isLanding = true;
        }
        void CheckFloor() => checkFloor = true;
        private void CancelAndInvoke(string method, float time)
        {
            CancelInvoke(method);
            Invoke(method, time);
        }

    }
}

