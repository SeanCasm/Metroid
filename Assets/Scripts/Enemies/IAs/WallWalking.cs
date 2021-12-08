using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class WallWalking : EnemyBase
{
    #region Properties
    [SerializeField] int direction;
    [SerializeField] float floorAware, wallAware, fallAware;
    [Tooltip("Adds a delay after change the eulerAngles on slopes")]
    [SerializeField] float checkDelay;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] CircleCollider2D coll;
    private Transform floorCorner;
    private Vector2 slopePerp;
    private float wallAngle, slopeAngle, prevAngle, curAngle, curSpeed;
    private bool wallInFront, checkFloor = true, fall;
    #endregion
    #region Unity Methods
    private new void OnEnable()
    {
        base.OnEnable();
        GameEvents.OnMissileImpact += ReactToMissileImpact;
        GameEvents.OnCrumble += ReactToCrumbleBlock;
    }
    private void OnDisable()
    {
        GameEvents.OnMissileImpact -= ReactToMissileImpact;
        GameEvents.OnCrumble -= ReactToCrumbleBlock;
    }
    new void Awake()
    {
        base.Awake();
        curSpeed = speed;
        floorCorner = transform.GetChild(0);
    }
    void Update()
    {
        if (!fall)
        {

            //Debug.DrawRay(transform.position, -transform.up * floorAware, Color.blue);
            if (!Physics2D.Raycast(transform.position, -transform.up, floorAware, groundLayer) && checkFloor)
            {
                RaycastHit2D hit2D = Physics2D.Raycast(floorCorner.position, -transform.right, floorAware, groundLayer);
                transform.position = hit2D.point;
                transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - 90f);
                checkFloor = false;
                Invoke("CheckFloor", checkDelay);
            }
            else
            {
                CheckAlign();
                CheckWall();
            }
        }
        else
        {
            RaycastHit2D hit;
            if (hit= Physics2D.Raycast(coll.bounds.center, Vector2.down, fallAware, groundLayer))
            {
                rigid.bodyType = RigidbodyType2D.Kinematic;
                fall = false;
                transform.position=hit.point;
                transform.eulerAngles = new Vector3(0, 0, 0);
                speed = curSpeed;
            }
        }
    }
    private void FixedUpdate()
    {
        rigid.velocity = speed * Time.deltaTime * transform.right;
    }
    #endregion
    #region Private Methods 
    private void ReactToMissileImpact(Vector2 pos)
    {
        if (transform.eulerAngles.z != 0 && Vector2.Distance(transform.position, pos) < 1.2f &&
            enabled)
            Fall();
    }
    private void ReactToCrumbleBlock(Vector2 pos)
    {
        if (Vector2.Distance(transform.position, pos) <= 0.2f)
            Fall();
    }
    private void Fall()
    {
        fall = true;
        rigid.bodyType = RigidbodyType2D.Dynamic;
        rigid.gravityScale = 5f;
        speed = 0;
    }
    private void CheckAlign()
    {
        RaycastHit2D align = Physics2D.Raycast(coll.bounds.center, -transform.up, floorAware, groundLayer);
        if (align) SlopeInit(align);
    }
    private void SlopeInit(RaycastHit2D align)
    {
        slopeAngle = Vector2.Angle(align.normal, Vector2.up);
        slopePerp = Vector2.Perpendicular(align.normal).normalized;
        if ((slopePerp.y < 0 && direction < 0) || (slopePerp.y > 0 && direction > 0)) slopeAngle *= -1;

        prevAngle = curAngle;
        curAngle = slopeAngle;

        if (prevAngle != curAngle) checkFloor = false;
        if (checkFloor) transform.eulerAngles = new Vector3(0, 0, slopeAngle);
        else if (!IsInvoking("CheckFloor")) Invoke("CheckFloor", checkDelay);
    }
    void CheckFloor() => checkFloor = true;
    private void CheckWall()
    {
        RaycastHit2D wallHit = Physics2D.Raycast(coll.bounds.center, transform.right, wallAware, groundLayer);
        if (wallHit)
        {
            wallAngle = Vector2.Angle(wallHit.normal, Vector2.up);
            if (CheckWallAngle(89, 91) || CheckWallAngle(-1, 1) || CheckWallAngle(179, 181)) wallInFront = true;
        }
        else wallAngle = 0;

        if (wallInFront)
        {
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z+90);
            transform.position=wallHit.point;
            wallInFront = false;
        }
    }
    private bool CheckWallAngle(float value1, float value2)
    {
        if (wallAngle >= value1 && wallAngle <= value2) return true;
        else return false;
    }
    #endregion
}
