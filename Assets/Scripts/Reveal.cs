using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Reveal : MonoBehaviour
{
    [SerializeField] Collider2D confiner;
    private Tilemap tilemap;
    private Color tempColor;
    private void Start()
    {
        tilemap = GetComponentInParent<Tilemap>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(confiner!=null && confiner is PolygonCollider2D){
                CurrentCamera.current.SwapConfiner(confiner);
            }
            print("XD");
            StartCoroutine(FadeOut(tilemap.color));
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(confiner!=null && confiner is PolygonCollider2D)
            {
                CurrentCamera.current.SetDefaultConfiner();
            }
            StartCoroutine(FadeIn(tilemap.color));
        }
    }
    IEnumerator FadeOut(Color color)
    {
        for (float i = 1f; i >= 0f; i -= 0.1f)
        {
            tempColor = color;
            tempColor.a = i;
            color= tempColor;
            tilemap.color=color;
            yield return new WaitForSeconds(0.05f);
        }
        //Fixed alpha
        tempColor=color;
        tempColor.a=0;
        color=tempColor;
         tilemap.color=color;
    }
    IEnumerator FadeIn(Color color)
    {
        for (float i = 0f; i <= 1f; i += 0.1f)
        {
            tempColor = color;
            tempColor.a = i;
            color= tempColor;
            tilemap.color=color;
            yield return new WaitForSeconds(0.05f);
        }
        //Fixed alpha
        tempColor = color;
        tempColor.a = 1;
        color = tempColor;
        tilemap.color=color;
    }
}
