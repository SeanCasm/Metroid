using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathCamera : MonoBehaviour
{
    [SerializeField] Transform camHandle;
    private Cinemachine.CinemachineVirtualCamera cam;
    private void Start()
    {
        cam = GetComponent<Cinemachine.CinemachineVirtualCamera>();
    }
    private void OnEnable()
    {
        Retry.Completed += OnPlayerDeath;
    }
    private void OnDisable()
    {
        Retry.Completed -= OnPlayerDeath;
    }
    private void OnPlayerDeath()
    {
        CurrentCamera.current.PlayerDeathConfiner();
        cam.LookAt = camHandle;
    }
}
