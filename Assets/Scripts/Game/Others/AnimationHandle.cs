using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class AnimationHandle : MonoBehaviour
{
    public static AnimationHandle current;
    [SerializeField] Animator animator;
    private Action StartCompleted,EndCompleted;
    private void Awake() {
        if(current==null){
            current=this;
            DontDestroyOnLoad(gameObject);
        }else Destroy(gameObject);
    }
    public void EnableMainMenu(Action action){
        StartCompleted=delegate{};
        StartCompleted=action;
        StartCompleted+=RetryEnd;
        animator.SetTrigger("Start");
    }
    public void StartRetry(Action action)
    {
        StartCompleted -= RetryEnd;
        StartCompleted=action;
        animator.SetTrigger("Start");
    }
    private void RetryEnd(){
        animator.SetTrigger("End");
    }
    public void EnableEnd(Action action){
        EndCompleted=action;
        animator.SetTrigger("End");
    }
    #region  UI
    private void CompletedStart(){
        StartCompleted.Invoke();
    }
    private void CompletedEnd(){
        EndCompleted?.Invoke();
        EndCompleted=delegate{};
    }
    #endregion
}
