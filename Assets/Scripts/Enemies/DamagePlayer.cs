using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] Collider2D col;
    public int Damage { get { return damage; } }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.IsTouching(col))
        {
            GameEvents.damagePlayer.Invoke(damage,transform.position.x);
        }
    }
}
