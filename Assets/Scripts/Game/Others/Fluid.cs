using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fluid : MonoBehaviour
{
    protected SkinSwapper pSkin;
    protected PlayerHealth playerH;
    protected PlayerController playerController;
    private Animator pAnimator;
    private bool playerGravity;
    protected void OnEnable() {
        PlayerInventory.GravitySetted+=UnsetSlow;
        PlayerInventory.GravityUnsetted+=SetSlow;
    }
    protected void OnDisable() {
        PlayerInventory.GravitySetted -= UnsetSlow;
        PlayerInventory.GravityUnsetted -= SetSlow;
    }
    protected void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !playerGravity)
        {
            pAnimator = col.GetComponentInParent<Animator>();
            pSkin=col.GetComponentInParent<SkinSwapper>();
            playerController=col.GetComponentInParent<PlayerController>();
            if (!pSkin.Gravity)
            {
                playerController.slow2Forces=1.75f;
                playerController.SetGravitySlow(2f);
            }else playerGravity = true;
            
        }
    }
    
    private void SetSlow(){
        playerGravity =false;
        playerController.slow2Forces = 1.75f;
        playerController.SetGravitySlow(2f);
    }
    private void UnsetSlow(){
        playerGravity =true;
        playerController.slow2Forces = 1;
        playerController.SetGravitySlow(1f);
    }
    protected void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !playerGravity)
        {
            if(!pSkin.Gravity){
                playerController.slow2Forces = 1;
                playerController.SetGravitySlow(1f);
                playerController.ResetGravity();
            }
        }
    }
}
