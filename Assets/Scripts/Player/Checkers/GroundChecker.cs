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
        [SerializeField] float overHeadCheck = .01f;
        [SerializeField, Range(.001f, .22f)] float slopeFrontRay = 0.08f, slopeBackRay = 0.08f, groundHitSlope;
        [SerializeField, Range(-1, 1.5f)] float slopeEdgesOffset;
        [SerializeField] float wallDistance, wallEdgeOffset, edgesOffset, spinOffset;
        [SerializeField, Range(.001f, .88f)] float groundDistance = 0.18f, airGroundDistance = 0.18f;
        private RaycastHit2D frontHit, backHit;
        private Rigidbody2D rigid;
        private bool rbStatic, isLanding, firstAir,_shootOnWalk;
        float frontAngle = 0, slopeAngle = 0, backAngle = 0, curGroundDis, curSpinOffset = 1;
        private Vector2 posFrontRay, posBackRay, slopePerp;
        public bool onSlope { get; set; }
        public bool wallInFront { get; private set; }
        public bool checkFloor { get; set; } = true;
        public bool ShootOnWalk
        {
            get => _shootOnWalk;
            set
            {
                _shootOnWalk = value;
                if (_shootOnWalk) CancelAndInvoke(nameof(shootingClocking), 2f);
                else CancelInvoke(nameof(shootingClocking));
            }
        }
        private bool groundOverHead;
        public bool GroundOverHead
        {
            get
            {
                return Physics2D.Raycast(capsule.bounds.max, Vector2.up, overHeadCheck, groundLayer)
                      && playerController.GroundState != GroundState.Stand;
            }
            set
            {
                groundOverHead = value;
            }
        }
        public bool isGrounded { get; set; }
        private void Start()
        {
            rigid = playerController.rb;
            curGroundDis = groundDistance;
        }
        internal void FixedUpdateOnGround(float xInput, float xVelocity)
        {
            if (isGrounded && xInput != 0 && !wallInFront)
            {
                rigid.velocity = (!onSlope) ? new Vector2(xVelocity, 0f) : new Vector2(-xVelocity * slopePerp.x, -xVelocity * slopePerp.y);
                if ((frontAngle == 0 && backAngle != 0 && slopeAngle == 0) && ((frontHit.point.y > backHit.point.y) || (frontHit.point.y < backHit.point.y)))
                {
                    transform.position.Set(frontHit.point.x, frontHit.point.y, 0);
                    rigid.SetVelocity(new Vector2(rigid.velocity.x, 0f));
                }
            }
        }
        internal bool SetFloorChecking()
        {
            if (checkFloor) CheckGround();

            return isGrounded;
        }
        internal void WallAndSlopeCheck(float xInput)
        {
            CheckWallInFront(xInput);
            CheckSlopesAndEdges(xInput);
        }
        internal bool CheckWallJump(float xInput)
        {
            if (Physics2D.Raycast(transform.position, Vector2.left, 0.3f, groundLayer) && xInput > 0) return true;
            else if (Physics2D.Raycast(transform.position, Vector2.right, 0.3f, groundLayer) && xInput < 0) return true;
            return false;
        }
        void CheckWallInFront(float xInput)
        {
            RaycastHit2D wallHit = Physics2D.BoxCast(new Vector2(capsule.bounds.center.x + (capsule.size.x / 2) * xInput,
            capsule.bounds.center.y),
            new Vector2(wallDistance, capsule.bounds.size.y - wallEdgeOffset), 0f, new Vector2(xInput, 0), wallDistance, groundLayer);
            if (wallHit && Vector2.Angle(wallHit.normal, Vector2.up) == 90)
            {
                wallInFront = true;
                xInput = 0;
            }
            else wallInFront = false;
        }
        private void CheckGround()
        {
            RaycastHit2D raycastHit2D = Physics2D.BoxCast(new Vector2(capsule.bounds.center.x, capsule.bounds.min.y),
            new Vector2(capsule.bounds.size.x / edgesOffset, curGroundDis), 0f,
            Vector2.down, curGroundDis * curSpinOffset, groundLayer);
            isGrounded = raycastHit2D;

            if (raycastHit2D) OnGround(raycastHit2D);
            else curGroundDis = airGroundDistance;

        }
        private void OnAir()
        {
            isLanding = wallInFront = onSlope = false;
        }
        public void OnJumping()
        {
            checkFloor = isGrounded = false;
            CancelAndInvoke(nameof(CheckFloor), .3f);
        }
        internal void ResetState()
        {
            ShootOnWalk=checkFloor = isGrounded = false;
        }
        internal void CheckSlopesAndEdges(float xInput)
        {
            Vector2 v = new Vector2(capsule.bounds.min.x + capsule.size.x / 2, capsule.bounds.min.y);
            posFrontRay = new Vector2(transform.position.x + capsule.size.x / 2 * (xInput - slopeEdgesOffset), capsule.bounds.min.y + .02f);
            RaycastHit2D hit2D = Physics2D.Raycast(posFrontRay, Vector2.down, groundHitSlope, groundLayer);
            RaycastHit2D hit = Physics2D.Raycast(v, Vector2.down, groundHitSlope, groundLayer);

            if (hit2D && hit)
            {
                frontAngle = Vector2.Angle(hit2D.normal, Vector2.up);
                slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                slopePerp = Vector2.Perpendicular(hit2D.normal).normalized;
                if ((slopePerp.y < 0 && xInput < 0) || (slopePerp.y > 0 && xInput > 0)) frontAngle *= -1;

                if ((frontAngle != 0) || (frontAngle == 0 && slopeAngle != 0) || slopeAngle != 0)
                    onSlope = true;
                else if (frontAngle == 0 && slopeAngle == 0 && backAngle == 0)
                {
                    onSlope = false;
                }

            }

            posBackRay = new Vector2(transform.position.x - capsule.size.x / 2 * (xInput - slopeEdgesOffset), capsule.bounds.min.y + .02f);

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
            if (onSlope) rigid.gravityScale = 0;
            else rigid.gravityScale = 1;
        }
        private void OnGround(RaycastHit2D raycastHit2D)
        {
            if (!isLanding) playerController.FirstOnGround(raycastHit2D.point.y);

            if (onSlope && playerController.xInput == 0 && !rbStatic)
            {
                transform.position = new Vector3(transform.position.x, raycastHit2D.point.y, 0);
                rigid.gravityScale = 0;
                rbStatic = true;
            }
            else if (!onSlope || playerController.xInput != 0)
            {
                rbStatic = false;
                rigid.gravityScale = 1 / playerController.Slow2Gravity;
            }
            curGroundDis = groundDistance;
            firstAir=false;
            isLanding = true;
        }
        internal void FirstAir(bool onSpin, float slow2Gravity, float spriteCenter)
        {
            if (!firstAir)
            {
                if (onSpin) transform.position = new Vector3(transform.position.x, transform.position.y + spriteCenter, 0);
                edgeCollider.enabled = false; playerController.rb.gravityScale = 1 / slow2Gravity; firstAir = true;
                OnAir(); ShootOnWalk = false;
            }
        }
        void shootingClocking() => ShootOnWalk = false;
        void CheckFloor() => checkFloor = true;
        private void CancelAndInvoke(string method, float time)
        {
            CancelInvoke(method);
            Invoke(method, time);
        }

    }
}