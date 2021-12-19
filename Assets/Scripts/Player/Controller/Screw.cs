using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player.PowerUps{
    public class Screw : MonoBehaviour
    {
        public PlayerController pContr;
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Enemy"))
            {
                EnemyHealth enemy = col.GetComponent<EnemyHealth>();
                if (pContr.status == Status.Powered)
                {
                    enemy.AddDamage(999);
                }
            }
        }
    }
}
 
