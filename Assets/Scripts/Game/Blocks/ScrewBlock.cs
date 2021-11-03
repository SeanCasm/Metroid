using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ScrewBlock : CrumbleBlock
{
    new void Awake() {
       base.Awake(); 
    }
    void OnTriggerEnter2D(Collider2D other) {
       if(other.CompareTag("Power"))anim.SetTrigger("Destroy");
       else base.CheckCollisionTag(other.tag);
    }
}
