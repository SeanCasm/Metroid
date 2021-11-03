using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Cinemachine;
using UI.Controller;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapController : Controller
{
    [SerializeField] float limitX,limitY;
    [SerializeField] float movementSpeed;
    [SerializeField]RenderTexture renderTexture;
    [SerializeField] Camera mapCamera,main;
    [SerializeField] UnityEvent back, enable;
    [SerializeField] UnityEvent<bool> onEnableAndroid;
    [SerializeField] GameObject upArrow,rightArrow,downArrow,leftArrow;
    [SerializeField] CinemachineVirtualCamera vcam;
    private Vector2 moveVector;
    bool canMove;
    private Transform originalParent;
    private void OnEnable()
    {
        canMove = true;
        main.gameObject.SetActive(false);
        mapCamera.targetTexture=null;
        vcam.m_Lens.OrthographicSize=40;
        enable.Invoke();
        #if UNITY_ANDROID
        onEnableAndroid?.Invoke(true);
        #endif
        base.inputManager.Submit+=OnSubmit;
        base.inputManager.Back+=OnCancel;
        StartCoroutine("CameraUpdate");
    }
    private void OnDisable()
    {
        StopCoroutine("CameraUpdate");
        canMove = false;
        back?.Invoke();
        main.gameObject.SetActive(true);
        vcam.m_Lens.OrthographicSize=5.3f;
        mapCamera.targetTexture=renderTexture;
        #if UNITY_ANDROID
        onEnableAndroid?.Invoke(false);
        #endif
        base.inputManager.Submit -= OnSubmit;
        base.inputManager.Back -= OnCancel;
        vcam.transform.localPosition = Vector2.zero;
    }
    IEnumerator CameraUpdate(){
        while(true){
            moveVector=base.inputManager.moveAction.ReadValue<Vector2>();
            vcam.transform.localPosition = new Vector3(
                Mathf.Clamp(vcam.transform.localPosition.x + movementSpeed * moveVector.x, -limitX, limitX),
                Mathf.Clamp(vcam.transform.localPosition.y + movementSpeed * moveVector.y,-limitY,limitY),
                0f
            );
            if(vcam.transform.localPosition.x>=limitX) rightArrow.SetActive(false);
            else rightArrow.SetActive(true);
            
            if(vcam.transform.localPosition.x<=-limitX) leftArrow.SetActive(false);
            else leftArrow.SetActive(true);

            if(vcam.transform.localPosition.y>=limitY) upArrow.SetActive(false);
            else upArrow.SetActive(true);

            if(vcam.transform.localPosition.y<=-limitY) downArrow.SetActive(false);
            else downArrow.SetActive(true);

            yield return new WaitForSecondsRealtime(0.025f);
        }
    }
    /*private void Update() {
        moveVector=base.inputManager.moveAction.ReadValue<Vector2>();
        vcam.transform.localPosition = new Vector3(
                Mathf.Clamp(vcam.transform.localPosition.x + movementSpeed * moveVector.x, -limitX, limitX),
                Mathf.Clamp(vcam.transform.localPosition.y + movementSpeed * moveVector.y,-limitY,limitY),
                0f
        );
        if(vcam.transform.localPosition.x>=limitX) rightArrow.SetActive(false);
        else rightArrow.SetActive(true);
        
        if(vcam.transform.localPosition.x<=-limitX) leftArrow.SetActive(false);
        else leftArrow.SetActive(true);

        if(vcam.transform.localPosition.y>=limitY) upArrow.SetActive(false);
        else upArrow.SetActive(true);

        if(vcam.transform.localPosition.y<=-limitY) downArrow.SetActive(false);
        else downArrow.SetActive(true);
    }
    */
    private void OnCancel(InputAction.CallbackContext context){
        enabled=false;
    }
    private void OnSubmit(InputAction.CallbackContext context)
    {
        vcam.transform.localPosition=Vector2.zero;
    }
}