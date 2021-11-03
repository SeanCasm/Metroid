using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BombBlock : CrumbleBlock
{
    #region Unity methods
    new void Awake()
    {
        base.Awake();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag(mainTag)){
            print(other.tag);
            anim.SetTrigger("Destroy");
        }else{
            CheckCollisionTag(other.tag);
        }
    }
    #endregion
}