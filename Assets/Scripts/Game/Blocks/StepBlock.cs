using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepBlock : CrumbleBlock
{
    private BoxCollider2D box;
    private Rigidbody2D rigid;
    new void Awake()
    {
        base.Awake();
        rigid = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
    }
    new void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.transform.position.y>transform.position.y)
        {
            anim.SetBool("Hide", true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))Invoke("ResetState", 2f);
    }

    private void ResetState()=>anim.SetBool("Hide", false);
}