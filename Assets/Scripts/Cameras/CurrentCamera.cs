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
        float targetaspect = 1280.0f / 768.0f;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = Camera.main;

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
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