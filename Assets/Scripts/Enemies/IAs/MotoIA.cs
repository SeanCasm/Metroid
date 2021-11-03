using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
public class MotoIA : EnemyBase
{
    #region Properties
    [SerializeField] BoxCollider2D playerDetector;
    private GroundSlopeChecker efd;
    private float currentSpeed;
    private CurrentState curState = CurrentState.Default;
    private enum CurrentState
    {
        Charging, Ramming, Default
    }
    #endregion
    #region Unity Methods
    new void Awake()
    {
        base.Awake();
        pDetect.OnDetection += () => curState = CurrentState.Charging;

        currentSpeed = speed;
        efd = GetComponentInChildren<GroundSlopeChecker>();
    }
    private void OnDisable()
    {
        pDetect.OnDetection -= () => curState = CurrentState.Charging;
    }
    private void FixedUpdate()
    {
        switch (curState)
        {
            case CurrentState.Default:
                efd.SetOnGroundVelocity(currentSpeed);
                break;
            case CurrentState.Charging:rigid.velocity = new Vector2(0f, 0f);
                break;
            case CurrentState.Ramming:efd.SetOnGroundVelocity(currentSpeed * 1.85f);
                break;
        }
    }
    private void LateUpdate()
    {
        anim.SetBool("detected", curState == CurrentState.Charging);
        anim.SetBool("prepared", curState == CurrentState.Ramming);
    }
    #endregion
    public void Charged()
    {
        curState = CurrentState.Ramming;
        playerDetector.enabled = false;
        Invoke("StopRamming",3f);
    }
    private void StopRamming(){
        playerDetector.enabled = true;
        curState=CurrentState.Default;
    }
}
