using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class BaseState : StateMachineBehaviour
{
    [SerializeField] protected string parameterName;
    protected AnimatorHandler animatorHandler;
    protected PlayerController playerController;
    protected Player.GroundChecker groundChecker;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animatorHandler = animator.GetComponent<AnimatorHandler>();
        playerController = animatorHandler.PlayerController;
        groundChecker=animatorHandler.GroundChecker;
    }
}
