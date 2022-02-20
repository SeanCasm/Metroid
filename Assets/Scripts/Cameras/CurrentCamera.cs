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
    private Collider2D scenarioConfiner, hideConfiner;
    private void Awake()
    {
        current = this;
    }
    private void Start()
    {
        Screen.SetResolution(1280, 720, true);
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
    public void CamScenarioConfiner(Collider2D confiner)
    {
        scenarioConfiner = confiner;
        CMConfiner.m_BoundingShape2D = scenarioConfiner;
    }
    public void CamHideConfiner(Collider2D confiner)
    {
        hideConfiner = confiner;
        CMConfiner.m_BoundingShape2D = hideConfiner;
    }
    public void PlayerDeathConfiner()
    {
        CMConfiner.m_BoundingShape2D = null;
    }
    public void SetDefaultConfiner()
    {
        CMConfiner.m_BoundingShape2D = scenarioConfiner;
        hideConfiner = null;
    }
}