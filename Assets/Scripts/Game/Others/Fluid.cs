using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fluid : MonoBehaviour
{
    protected SkinSwapper pSkin;
    protected PlayerHealth playerH;
    protected PlayerController playerController;
    protected Player.GroundChecker groundChecker;
    private Animator pAnimator;
    private bool playerGravity;
    protected void OnEnable()
    {
        PlayerInventory.GravitySetted += UnsetSlow;
        PlayerInventory.GravityUnsetted += SetSlow;
    }
    protected void OnDisable()
    {
        PlayerInventory.GravitySetted -= UnsetSlow;
        PlayerInventory.GravityUnsetted -= SetSlow;
    }
    protected void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !playerGravity)
        {
            pAnimator = col.GetComponentInParent<Animator>();
            pSkin = col.GetComponentInParent<SkinSwapper>();
            playerController = col.GetComponentInParent<PlayerController>();
            groundChecker = col.GetComponentInParent<Player.GroundChecker>();
            if (!pSkin.Gravity) playerController.slow = .5f;
            else playerGravity = true;

        }
    }

    private void SetSlow()
    {
        playerGravity = false;
        playerController.slow = .5f;
    }
    private void UnsetSlow()
    {
        playerGravity = true;
        playerController.slow = 1;
    }
    protected void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !playerGravity)
        {
            if (!pSkin.Gravity)
            {
                playerController.slow = 1;
                groundChecker.ResetGravity();
            }
        }
    }
}
