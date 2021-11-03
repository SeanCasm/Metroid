using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    public bool facingRight{get;set;}
    public bool doJump{get;set;}
    public System.Action OnJump,OutJump;
    public void Flip()
    {
        facingRight = !facingRight;
        transform.eulerAngles = transform.eulerAngles.y==0? new Vector2(0,180): new Vector2(0,0);
    }
    public void Jump()
    {
        doJump = !doJump;
        if(doJump)OnJump?.Invoke();
        else OutJump?.Invoke();
    }
}
