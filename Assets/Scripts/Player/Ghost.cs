using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField] float disableTime;
    [SerializeField] SpriteRenderer sRen;
    private void OnEnable() {
        Destroy(gameObject,disableTime);
    }
    
    public void SetSprite(Sprite sprite,bool flipX){
        sRen.sprite=sprite;
        sRen.flipX=flipX;
    }
}
