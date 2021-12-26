using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player
{
    public class AnimatorHandler : MonoBehaviour
    {
        [SerializeField] SpriteRenderer playerGun;
        [SerializeField] PlayerController playerController;
        [SerializeField] Player.GroundChecker groundChecker;
        [SerializeField] Gun gun;
        public Player.GroundChecker GroundChecker { get => groundChecker; }
        public PlayerController PlayerController { get => playerController; }
        public void CheckAmmoSelected(bool disable)
        {
            if (gun.GunMissileAmmoSelected)
            {
                playerGun.enabled = disable ? false : true;
            }
        }
    }
}
