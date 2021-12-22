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
        int[] animatorHash = new int[27];
        private Animator animator;
        int booleanParameters=16;
        public Player.GroundChecker GroundChecker { get => groundChecker; }
        public PlayerController PlayerController { get => playerController; }
        private void Awake()
        {
            animator = GetComponent<Animator>();
            for (int i = 0; i < animator.parameterCount; i++) animatorHash[i] = Animator.StringToHash(animator.parameters[i].name);

        }
        public void CheckAmmoSelected(bool disable)
        {
            if (gun.GunMissileAmmoSelected)
            {
                playerGun.enabled = disable ? false : true;
            }
        }
        public void SetAnimation(int index, bool enable) => animator.SetBool(animatorHash[index], enable);
        /*
            Sets the 16 boolean parameters values to animator
        */
        public void SetAnimations(bool[] expressions)
        {
            for (int i = 0; i < expressions.Length; i++) //16 boolean parameters
            {
                animator.SetBool(animator.parameters[i].name,expressions[i]);
            }
        }
        public void FixSpritePivot(bool spinJump, bool screw, bool gravityJump)
        {
            //fix a issue with the sprite pivot. 
            animator.SetBool(animatorHash[9], spinJump);//spin jump
            animator.SetBool(animatorHash[12], screw);//screw
            animator.SetBool(animatorHash[15], gravityJump);//gravity jump
        }
        public bool AnyJump()
        {
            return (
                animator.GetBool(animatorHash[9]) == true ||
                animator.GetBool(animatorHash[12]) == true ||
                animator.GetBool(animatorHash[15]) == true
            );
        }
    }

}
