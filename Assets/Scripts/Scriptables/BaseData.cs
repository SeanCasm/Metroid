using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player{
    [CreateAssetMenu(fileName="new BaseData",menuName="ScriptableObjects/Player/BaseData")]
    public class BaseData : ScriptableObject
    {
        [Header("Player inventory")]
        public int missileAmmo;
        [Header("Player health")]
        public int health;
        public int totalHealth;
        public int energyTanks;
        [Header("Misc")]
        public Vector3 spawn;
        public float jumpForce,jumpForceUp;
        public Beams beams;
        public virtual void SetInventoryData(PlayerInventory inventory){ 
            inventory.AddToItems(4,true);//add the morfball item, initial game item by default.
            //inventory.transform.position=spawn;
            inventory.SetSuit();
            inventory.bUtilities.SetButton(4,true);
        }
         
        public virtual void SetHealthData(PlayerHealth playerHealth){
            playerHealth.MyHealth=health;
            playerHealth.ETanks=energyTanks;
            playerHealth.healthUpdate.Invoke(health,energyTanks);
        }
    }
}
 
