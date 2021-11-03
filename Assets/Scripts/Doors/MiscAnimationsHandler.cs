using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class MiscAnimationsHandler : MonoBehaviour
{
    [SerializeField] Animator camAnimator;
    private SpriteRenderer spriteRenderer;
    CameraTransition cameraTransition;
    private Animator animator;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        GameEvents.OnWarp += StartTransition;
        Retry.Start+=StartDeathAnimation;
    }
    private void OnDisable()
    {
        Retry.Start -= StartDeathAnimation;
        GameEvents.OnWarp -= StartTransition;
    }
    private void StartDeathAnimation(){
        animator.SetTrigger("Death");
    }
    private void DestroyScenario()=>SceneHandler.current.DestroyScenario();
    private void StartTransition(CameraTransition camTransition)
    {
        CurrentCamera.current.OnWarpCamera();
        cameraTransition = camTransition;
        animator.SetTrigger("Start");
        StartCoroutine("Lerp");
    }
    public void StopAll()
    {
        StopAllCoroutines();
        CurrentCamera.current.OnWarpCameraFinished();
    }
    IEnumerator Lerp()
    {
        yield return new WaitForSecondsRealtime(.5f);
        switch (cameraTransition)
        {
            case CameraTransition.Left:
                camAnimator.SetTrigger("Left");
                break;
            case CameraTransition.Right:
                camAnimator.SetTrigger("Right");
                break;
            case CameraTransition.Up:
                camAnimator.SetTrigger("Up");
                break;
            case CameraTransition.Down:
                camAnimator.SetTrigger("Down");
                break;
        }
    }
}
