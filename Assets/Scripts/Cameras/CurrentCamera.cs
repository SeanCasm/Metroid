using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CurrentCamera : MonoBehaviour
{
    public static CurrentCamera current;
    [SerializeField] Transform camHandler;
    public CinemachineVirtualCamera virtualCam{get;set;}
    public Camera main;
    float aux;
    CinemachineFramingTransposer framingTransposer;
    public CinemachineConfiner2D CMConfiner{get;set;}
    private void Start() {
        if(current==null){
            current=this;
        }
        virtualCam=GetComponent<CinemachineVirtualCamera>();
        CMConfiner = GetComponent<CinemachineConfiner2D>();

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
    public void OnWarpCamera(){
        virtualCam.gameObject.SetActive(false);
        virtualCam.Follow = null;
        CMConfiner.m_BoundingShape2D = null;
    }
    public void OnWarpCameraFinished(){
        virtualCam.Follow = camHandler;
        virtualCam.transform.position = camHandler.position;
        virtualCam.gameObject.SetActive(true);
    }
}
