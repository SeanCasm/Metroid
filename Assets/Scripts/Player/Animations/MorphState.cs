using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphState : BaseState
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        CheckAnimSpeed(animator);
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CheckAnimSpeed(animator);
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController.morphballSlow = 1;
    }
    private void CheckAnimSpeed(Animator animator)
    {
        if (playerController.xInput == 0 && groundChecker.isGrounded)
        {
            playerController.morphballSlow = 0;
        }
        else if (!groundChecker.isGrounded || (playerController.xInput != 0 && groundChecker.isGrounded))
        {
            playerController.morphballSlow = 1;
        }
    }
}
