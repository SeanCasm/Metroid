using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpAnimationCamera : MonoBehaviour
{
    public static WarpAnimationCamera current;
    [SerializeField] GameObject virtualCamera;
    [SerializeField] Transform playerTransform;
    private Animator animator;

    private void Awake()
    {
        current = this;
        animator = GetComponent<Animator>();
    }
    public void MoveTo(CameraTransition cameraTransition)
    {
        string state = "";
        switch (cameraTransition)
        {
            case CameraTransition.Right:
                state = "Right";
                break;
            case CameraTransition.Left:
                state = "Left";
                break;
        }
        animator.Rebind();
        virtualCamera.SetActive(false);
        animator.enabled=true;
        animator.Play(state);

    }
    //Called in animation event
    void MoveEnding()
    {
        virtualCamera.SetActive(true);
        animator.enabled=false;
    }
}
public enum CameraTransition { Left, Right, Up, Down }

