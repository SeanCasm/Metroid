using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    [SerializeField] int damage;
    public int Damage { get { return damage; } }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") )
        {
            PlayerKnockBack playerKnockBack = collision.GetComponent<PlayerKnockBack>();
            playerKnockBack.HitPlayer(damage,transform.position.x);
        }
    }
}
