using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EnemyHealth : Health<float>,IDamageable<float>,IFreezeable,IInvulnerable
{
    [SerializeField] EnemyType enemyType;
    [SerializeField] bool invMissiles, invSuperMissiles, invBeams, invBombs, invSuperBombs, invFreeze,invPlasma,invCharged;
    [SerializeField] Materials materials;
    [SerializeField]GameObject deathClip,freezedCol;
    [SerializeField]Collider2D rigidCol;
    private Action OnDeath;
    public Action OnDamage;
    public Action<float> OnSideDamage;
    public int collideDamage;
    public GameObject deadPrefab;
    private Behaviour[] components;
    private Collider2D box;
    private PlayerController playerController;
    public bool unFreezing{get;set;}
    public bool freezed { get;set; }
    public bool InvPlasma=>invPlasma;
    public bool InvMissiles => invMissiles;public bool InvSuperMissiles => invSuperMissiles;
    public bool InvBeams => invBeams;public bool InvBombs => invBombs;
    public bool InvSuperBombs => invSuperBombs;public bool InvFreeze => invFreeze;

    public bool InvSpazer => invCharged;

    #region Unity methods
    private void Start() {
        maxHealth=health;
        if(enemyType==EnemyType.Destroyable)OnDeath=DestroyOnDeath;
        else OnDeath=ResetOnDeath;
    }
    private void Awake()
    {
        anim = GetComponentInParent<Animator>();
        _renderer = GetComponentInParent<SpriteRenderer>();
        rb2d = GetComponentInParent<Rigidbody2D>();
        box=GetComponent<Collider2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player") && !freezed && (playerController=other.GetComponentInParent<PlayerController>())!=null){
            if(playerController.status != Status.Powered)
                GameEvents.damagePlayer.Invoke(collideDamage,transform.position.x);
        }
    }
    #endregion
    #region Public methods
    public void FreezeMe()
    {
        CancelInvoke("Unfreeze");
        StopAllCoroutines();
        if(rigidCol)rigidCol.enabled=false;
        freezedCol.SetActive(true);freezed = true;
        Invoke("Unfreeze",4f);
        _renderer.material = materials.freeze;
        rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        components = transform.parent.gameObject.GetComponents<Behaviour>();
        Utilities.SetBehaviours(components, false);
        Physics2D.IgnoreLayerCollision(8, 9, false);
        StartCoroutine(FreezeVisualFeedBack());     
    }
     
    public void Unfreeze()
    {
        if (rigidCol) rigidCol.enabled = true;
        freezedCol.SetActive(false); box.enabled=false;
        Utilities.SetBehaviours(components, true);
        _renderer.material = materials.defaultMaterial;
        rb2d.constraints = RigidbodyConstraints2D.None;
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        Physics2D.IgnoreLayerCollision(8, 9, true);
        unFreezing=freezed = false;
        box.enabled = true;
    }
    public void AddDamage(float amount)
    {
        health -= amount;
        OnDamage?.Invoke();
        if (health <= 0)
        {
            if (deathClip != null && deadPrefab != null)
            {
                var obj = DropManager.instance.TryToDrop();
                if(obj!=null) Instantiate(obj,transform.position,Quaternion.identity);
                gameObject.GetParent().SetActive(false);
                Instantiate(deathClip);
                Instantiate(deadPrefab, transform.position, Quaternion.identity, null);
            }
            health = 0;
            OnDeath?.Invoke();
        }else StartCoroutine(VisualFeedBack());
    }
    #endregion
    #region Private methods
    private void DestroyOnDeath(){
        Destroy(gameObject.GetParent());
    }
    private void ResetOnDeath(){
        health=maxHealth;
        _renderer.color=Color.white;
        gameObject.GetParent().SetActive(false);
    }
    private IEnumerator VisualFeedBack()
    {
        _renderer.color = Color.red;
        yield return new WaitForSecondsRealtime(0.1f);
        _renderer.color = Color.white;
    }
    public IEnumerator FreezeVisualFeedBack()
    {
        _renderer.color=_renderer.color.Default();
        yield return new WaitForSeconds(2f);
        unFreezing = true;
        while (freezed)
        {
            for (float i = 1; i >= 0.5f; i -= .5f)
            {
                Color color = _renderer.color;
                _renderer.color = color.SetColorRGB(i);
                yield return new WaitForSeconds(0.05f);
            }
            for (float i = 0.5f; i <= 1; i += .5f)
            {
                Color color = _renderer.color;
                _renderer.color = color.SetColorRGB(i);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    public void SetDide(float side)
    {
        OnSideDamage?.Invoke(side);
        side=0;
    }
    #endregion
}
public enum EnemyType{
    Destroyable, Pooleable
}
 
public interface IInvulnerable
{
    bool InvMissiles { get; }
    bool InvSuperMissiles { get; }
    bool InvBeams { get; }
    bool InvBombs { get; }
    bool InvSuperBombs { get; }
    bool InvFreeze { get; }
    bool InvPlasma { get; }
    bool InvSpazer{get;}
}