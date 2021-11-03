using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSwimLimit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            FishIA fish;
            collision.TryGetComponent<FishIA>(out fish);
            fish?.BackToSpawn(); 
        }
    }
}
