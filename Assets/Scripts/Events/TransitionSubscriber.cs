using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class runs a animation when played use the map updater
/// </summary>
public class TransitionSubscriber : MonoBehaviour
{
    [Range(0.1f,3f)]
    [SerializeField]float animationTime;
    private Animator animator;
    private void Awake() {
        animator=GetComponent<Animator>();
    }
    private void OnEnable() {
        GameEvents.instance.StartTransition+=StartTrantision;
    }
    private void OnDisable() {
        GameEvents.instance.StartTransition -=StartTrantision;
    }
    private float StartTrantision(){
        animator.SetTrigger("Start");
        return animationTime;
    }
}
