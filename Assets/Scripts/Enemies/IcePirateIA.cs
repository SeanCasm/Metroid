using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
public class IcePirateIA : SpacePirate
{
    [SerializeField] PlayerDetector playerDetector;
    bool waitAfterFreeze = false;
    private new void Awake()
    {
        base.Awake();
    }
    private new void Update()
    {
        if (!waitAfterFreeze)
        {
            base.Update();
        }
        
        if (playerDetector.playerHealth != null && playerDetector.playerHealth.freezed) waitAfterFreeze = true;
        else waitAfterFreeze = false;
    }
    private new void LateUpdate()
    {
        anim.SetBool("Idle", waitAfterFreeze);
    }
}
