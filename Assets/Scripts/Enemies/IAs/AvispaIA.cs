using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
/// <summary>
/// Enemy instantiated in the pool of enemies.
/// </summary>
public class AvispaIA : EnemyBase
{
    public PoolSpawner ePool{get;set;}
    private float direction;
    public float y{get;set;}=0;
    new void Awake() {
        base.Awake();
    }
    void OnEnable()
    {
        direction=transform.localScale.x;
    }
    void FixedUpdate()
    {
        rigid.velocity = (transform.position.y<=y) ? new Vector2(0,speed*Time.deltaTime) : new Vector2(speed*Time.deltaTime*direction,0);  
    }
    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
    private void OnDisable() {
        transform.localPosition = Vector2.zero;
        direction=0;
        y=0;
    }
}
