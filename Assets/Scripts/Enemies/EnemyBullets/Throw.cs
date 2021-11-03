using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Weapons;
public class Throw : Weapon
{
    [SerializeField] float timeTillHit;
    new void Start()
    {
        base.Start();
    }
    new void Awake() {
       base.OnEnable(); 
    }
    new void OnEnable()
    {
        Invoke("BackToShootPoint", livingTime);
    }
    new void OnBecameInvisible() {
        base.OnBecameInvisible();
    }
    new void FixedUpdate() {
        
    }
    // <summary>
    /// Makes a parabolic trajectory through the bullet throw point and target
    /// </summary>
    /// <param name="throwPoint">transform of throw point</param>
    /// <param name="target">transform of the target point</param>
    public void ThrowPrefab(Transform throwPoint, Vector3 target)
    {
        float xdistance;
        xdistance = target.x - throwPoint.position.x;
        float ydistance;
        ydistance = target.y - throwPoint.position.y;
        float throwAngle;
        throwAngle = Mathf.Atan((ydistance + 4.905f * (timeTillHit * timeTillHit)) / xdistance);
        float totalVelo = xdistance / (Mathf.Cos(throwAngle) * timeTillHit);
        float xVelo, yVelo;
        xVelo = totalVelo * Mathf.Cos(throwAngle);
        yVelo = totalVelo * Mathf.Sin(throwAngle);
        rigid=GetComponent<Rigidbody2D>();
        rigid.velocity = new Vector2(xVelo, yVelo);
    }
}
