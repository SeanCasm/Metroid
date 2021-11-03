using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] Collider2D detector;
    public bool detected{get;set;}
    public GameObject player { get;set; }
    private PlayerController playerController;
    public Vector3 GetPlayerTransformCenter(){
        return playerController.TransformCenter();
    }
    public System.Action OnDetection,OnOut;
    void OnTriggerEnter2D(Collider2D col) { 
        if (col.CompareTag("Player"))
        {
            OnDetection?.Invoke();
            detected = true;
            player = col.GetComponentInParent<PlayerController>().gameObject;
            playerController =col.GetComponentInParent<PlayerController>();
        }
    } 
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") )
        {
            OnOut?.Invoke();
            detected = false;
            player = null;
        }
    }
}
