using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] Transform target;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            other.GetComponentInParent<PlayerController>().SetTransformCenter(target.position);
            CurrentCamera.current.SetPosition2CenterOfPlayer();
        }
    }
}
