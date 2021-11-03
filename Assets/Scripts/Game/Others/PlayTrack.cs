using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTrack : MonoBehaviour
{
    private AudioSource audioPlayer;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerDetect"))
        {
            audioPlayer = GetComponent<AudioSource>();
            audioPlayer.Play();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerDetect"))
        {
            audioPlayer = GetComponent<AudioSource>();
            audioPlayer.Stop();
        }
    }
}
