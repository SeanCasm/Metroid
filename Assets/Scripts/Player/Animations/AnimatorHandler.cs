using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    [SerializeField] SpriteRenderer playerGun;
    [SerializeField] Gun gun;
    public void CheckAmmoSelected(bool disable){
        if(gun.GunMissileAmmoSelected){
            playerGun.enabled = disable ? false : true;
        }
        
         
    }
}
