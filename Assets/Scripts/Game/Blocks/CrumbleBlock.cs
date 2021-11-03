using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbleBlock : MonoBehaviour
{
    #region Properties
    [SerializeField] string[] tags;
    [SerializeField] protected string mainTag;
    private SpriteRenderer spriteRenderer, childRenderer;
    protected Animator anim;
    #endregion
    #region Unity methods
    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        childRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    #endregion
    public void CheckCollision(string tag,System.Action onCollide){
        if(tag.Equals(mainTag)){
            anim.SetTrigger("Destroy");
            onCollide?.Invoke();
            return;
        }
        CheckCollisionTag(tag,onCollide);
    }
    protected void CheckCollisionTag(string tag,System.Action onCollide){
        foreach (string element in tags)
        {
            if (tag.Equals(element)){
                anim.SetBool("Show", true);
                onCollide?.Invoke();
                return;
            } 
        }
    }
    protected void CheckCollisionTag(string tag)
    {
        foreach (string element in tags)
        {
            if (tag.Equals(element))
            {
                anim.SetBool("Show", true);
                return;
            }
        }
    }
    #region Private methods
   
    private void DeactivatingBlock()
    {
        GameEvents.OnCrumble?.Invoke(transform.position);
        Destroy(gameObject);
    }
    #endregion
}