using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashSystem : MonoBehaviour
{
    [SerializeField] GameObject splashSystem;
    [SerializeField] float time;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && col.transform.position.y>=transform.position.y)
        {
            splashSystem.SetActive(true);
            var pos=col.GetComponentInParent<PlayerController>().TransformCenter();
            splashSystem.transform.position=pos;
            Invoke("DisableParticles",time);
        }
    }
    void DisableParticles(){
        splashSystem.SetActive(false);
    }
}
