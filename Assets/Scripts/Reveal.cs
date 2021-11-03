using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reveal : MonoBehaviour
{
    [SerializeField]PolygonCollider2D confiner;
    private Collider2D previousConfiner;
    private SpriteRenderer sprite;
    private Color tempColor;
    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(confiner!=null){
                previousConfiner = CurrentCamera.current.CMConfiner.m_BoundingShape2D;
                CurrentCamera.current.CMConfiner.m_BoundingShape2D = confiner;
            }
            StartCoroutine(FadeOut(sprite));
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(confiner!=null)
            {
                CurrentCamera.current.CMConfiner.m_BoundingShape2D=previousConfiner;
                previousConfiner=null;
            }
            StartCoroutine(FadeIn(sprite));
        }
    }
    IEnumerator FadeOut(SpriteRenderer sprite)
    {
        for (float i = 1f; i >= 0f; i -= 0.1f)
        {
            tempColor = sprite.color;
            tempColor.a = i;
            sprite.color = tempColor;
            yield return new WaitForSeconds(0.05f);
        }
        //Fixed alpha
        tempColor=sprite.color;
        tempColor.a=0;
        sprite.color=tempColor;
    }
    IEnumerator FadeIn(SpriteRenderer sprite)
    {
        for (float i = 0f; i <= 1f; i += 0.1f)
        {
            tempColor = sprite.color;
            tempColor.a = i;
            sprite.color = tempColor;
            yield return new WaitForSeconds(0.05f);
        }
        //Fixed alpha
        tempColor = sprite.color;
        tempColor.a = 1;
        sprite.color = tempColor;
    }
}
