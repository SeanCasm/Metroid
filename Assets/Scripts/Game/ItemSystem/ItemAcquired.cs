using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ItemAcquired : MonoBehaviour,ICollecteable
{
    #region Properties
    [SerializeField] string message;
    [SerializeField] int iD;
    [SerializeField] ItemType itemType;
    private BoxCollider2D box;
    private Animator anim;

    public int ID { get { return iD; } }
    public string Message { get { return message; } }
    public ItemType iType { get { return itemType; } }
    #endregion
    #region Unity Methods
    private void OnEnable() {
        bool registered = GameEvents.verifyRegistry.Invoke(iD, false);
        if (registered) Destroy(gameObject);
    }
    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }
    private void OnBecameInvisible()
    {
        anim.enabled =box.enabled = false;
    }
    private void OnBecameVisible()
    {
        anim.enabled = box.enabled = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CollectorManager.instance.HandlePickupItem(this);
        }
    }
    #endregion
}
public enum ItemType{Special, Suit, Beam}
