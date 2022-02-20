using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CamConfinerSwapper : MonoBehaviour
{
    [SerializeField] Collider2D newConfiner;
    private void OnEnable()
    {
        CurrentCamera.current.CamScenarioConfiner(newConfiner);
    }
}
