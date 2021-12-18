using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    Controls the components of another gameobjects at game pause
*/
public class PauseSystem : MonoBehaviour
{
    public static PauseSystem instance;
    [SerializeField] PlayerController playerController;
    private void Awake() {
        if(instance==null){
            instance=this;
        }else Destroy(this);
    }
    public void EnablePlayerAnimator(bool enable){
        playerController.anim.enabled=enable;
    }
    public void EnablePlayerContoller(bool enable){
        playerController.enabled=enable;
    }
}
