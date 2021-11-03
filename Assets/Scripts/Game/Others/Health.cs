using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health<T> : MonoBehaviour
{
    [SerializeField] protected T health;
    public T maxHealth{get;set;}
    public T MyHealth{get=>health;set=>health=value;}
    protected SpriteRenderer _renderer;
    protected Rigidbody2D rb2d;
    protected Animator anim; 
}
public interface IDamageable<T>
{
    void AddDamage(T damage);
    void SetDide(float side);
}
public interface IDamageableTwo{
    bool canDetectSide{get;set;}
    void SetSide();
}
public interface IFreezeable
{
    bool unFreezing{get;set;}
    bool freezed { get; set; }
    void FreezeMe();
    void Unfreeze();
    IEnumerator FreezeVisualFeedBack();
}