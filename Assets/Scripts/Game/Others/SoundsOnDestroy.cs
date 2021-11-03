using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsOnDestroy : MonoBehaviour
{
    public GameObject audioSound;
    // Start is called before the first frame update
 
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") )
        {
            Instantiate(audioSound,col.transform);
        }
         
    }
}
