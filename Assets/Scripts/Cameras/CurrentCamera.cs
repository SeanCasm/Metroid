using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
public class CurrentCamera : MonoBehaviour
{
    public static CurrentCamera current;
    [SerializeField] Transform camHandler;
    [SerializeField] CinemachineConfiner2D CMConfiner;
    [Header("Animation config")]
    [SerializeField] Animator doorWarp;
    [SerializeField] float time;
    public CinemachineVirtualCamera virtualCam;
    public Camera main;
    float aux;
    CinemachineFramingTransposer framingTransposer;
    private Collider2D curCollider;
    private void Awake() {
        current=this;
        curCollider=CMConfiner.m_BoundingShape2D;
    }
    public void SetPosition2CenterOfPlayer(){
        var componentBase = virtualCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if(componentBase is CinemachineFramingTransposer){
            framingTransposer = componentBase as CinemachineFramingTransposer;
            aux = framingTransposer.m_DeadZoneHeight;
            framingTransposer.m_DeadZoneHeight = 0;
            StartCoroutine("SetDeadZoneHeight");
        }
    }
    IEnumerator SetDeadZoneHeight(){
        yield return new WaitForEndOfFrame();
        framingTransposer.m_DeadZoneHeight = aux;
    }
    public void SwapConfiner(Collider2D newCol){
        CMConfiner.m_BoundingShape2D = newCol;
    }
    public void SetDefaultConfiner(){
        CMConfiner.m_BoundingShape2D = curCollider;
    }
    public void MoveTo(CameraTransition cameraTransition){
        OnWarpCamera();
        switch(cameraTransition){
            case CameraTransition.Left:
            StartCoroutine(MoveTo(Vector2.left));
            break;
            case CameraTransition.Right:
            StartCoroutine(MoveTo(Vector2.right));
            break;
        }
    }
    IEnumerator MoveTo(Vector2 dir){
        Vector2 pos = main.transform.position;
        while(main.transform.position.x<=pos.x+1.8f*dir.x){
            yield return null;
            main.transform.Translate(dir * (Time.deltaTime * (1.8f*time)));
        }
        OnWarpCameraFinished();
    }
    private void OnWarpCamera(){
        doorWarp.SetTrigger("Start");
        virtualCam.gameObject.SetActive(false);
        virtualCam.Follow = null;
        CMConfiner.m_BoundingShape2D = null;
    }
    private void OnWarpCameraFinished(){
        doorWarp.SetTrigger("End");
        virtualCam.Follow = camHandler;
        virtualCam.transform.position = camHandler.position;
        virtualCam.gameObject.SetActive(true);
    }
}
public enum CameraTransition{Left, Right, Up, Down}