using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CamConfinerSwapper : MonoBehaviour
{
    [SerializeField]Collider2D newConfiner;
    public static System.Action SwapCam;
    private void OnEnable() {
        CurrentCamera.current.CMConfiner.m_BoundingShape2D = newConfiner;
    }
}
