using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    private AudioSource audioSource;
    private void Start() {
        TryGetComponent<AudioSource>(out audioSource);
        if(audioSource!=null){
            Destroy(gameObject,audioSource.clip.length);
        }
    }
    public void DestroyPrefab()
    {
        Destroy(gameObject);
    }
}
