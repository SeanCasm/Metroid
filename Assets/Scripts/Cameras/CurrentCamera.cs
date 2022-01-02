using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
public class CurrentCamera : MonoBehaviour
{
    public static CurrentCamera current;
    [SerializeField] CinemachineConfiner2D CMConfiner;
    public CinemachineVirtualCamera virtualCam;
    float aux;
    CinemachineFramingTransposer framingTransposer;
    private Collider2D curCollider;
    private void Awake()
    {
        current = this;
        curCollider = CMConfiner.m_BoundingShape2D;
        Screen.SetResolution(1280,768,true);
    }
    public void SetPosition2CenterOfPlayer()
    {
        var componentBase = virtualCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if (componentBase is CinemachineFramingTransposer)
        {
            framingTransposer = componentBase as CinemachineFramingTransposer;
            aux = framingTransposer.m_DeadZoneHeight;
            framingTransposer.m_DeadZoneHeight = 0;
            StartCoroutine("SetDeadZoneHeight");
        }
    }
    IEnumerator SetDeadZoneHeight()
    {
        yield return new WaitForEndOfFrame();
        framingTransposer.m_DeadZoneHeight = aux;
    }
    public void SwapConfiner(Collider2D newCol)
    {
        CMConfiner.m_BoundingShape2D = newCol;
    }
    public void SetDefaultConfiner()
    {
        CMConfiner.m_BoundingShape2D = curCollider;
    }
}