using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EnemyHealth : Health<float>, IDamageable<float>, IFreezeable, IInvulnerable
{
    [Header("Default invulnerabilities")]
    [SerializeField] bool invMissiles;
    [SerializeField] bool invSuperMissiles, invBeams, invBombs, invSuperBombs, invFreeze, invPlasma, invCharged;
    [Header("Invulnerabilities after being freezed")]
    [SerializeField] bool missiles;
    [SerializeField] bool superMissiles, beams, bombs, superBombs, plasma, charged;
    [Header("Misc")]
    [SerializeField] Materials materials;
    [SerializeField] GameObject freezedCol;
    [SerializeField] Collider2D rigidCol;
    [SerializeField] bool pooleable;
    protected List<Behaviour> componentsToDisable = new List<Behaviour>();
    protected Action OnDeath;
    public Action OnDamage;
    public Action<float> OnSideDamage;
    public int collideDamage;
    private Behaviour[] components;
    protected Collider2D hurtbox;
    private PlayerController playerController;
    protected Material dissolve;
    public bool unFreezing { get; set; }
    public bool freezed { get; set; }
    public bool InvPlasma => invPlasma;
    public bool InvMissiles => invMissiles; public bool InvSuperMissiles => invSuperMissiles;
    public bool InvBeams => invBeams; public bool InvBombs => invBombs;
    public bool InvSuperBombs => invSuperBombs; public bool InvFreeze => invFreeze;
    private Dictionary<string, bool> defaultInv = new Dictionary<string, bool>();
    public bool InvSpazer => invCharged;

    #region Unity methods
    protected void Start()
    {
        maxHealth = health;
        AddDefaultInv2Dictionary();
        OnDeath = DestroyOnDeath;
    }
    private void Awake()
    {
        anim = GetComponentInParent<Animator>();
        componentsToDisable = new List<Behaviour>(){
            {anim},
            {GetComponentInParent<Enemy.EnemyBase>()}
        };
        _renderer = GetComponentInParent<SpriteRenderer>();
        dissolve = _renderer.material;
        rb2d = GetComponentInParent<Rigidbody2D>();
        hurtbox = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !freezed && (playerController = other.GetComponentInParent<PlayerController>()) != null)
        {
            if (playerController.status != Status.Powered)
            {
                PlayerKnockBack playerKnockBack = other.GetComponent<PlayerKnockBack>();
                playerKnockBack.HitPlayer(collideDamage, transform.position.x);
            }
        }
    }
    #endregion
    #region Public methods
    private void AddDefaultInv2Dictionary()
    {
        defaultInv = new Dictionary<string, bool>()
        {
            {nameof(invMissiles),invMissiles},
            {nameof(invSuperMissiles),invSuperMissiles},
            {nameof(invBeams),invBeams},
            {nameof(invBombs),invBombs},
            {nameof(invCharged),invCharged},
            {nameof(invPlasma),invPlasma},
            {nameof(invSuperBombs),invSuperBombs}
        };
    }
    private void SetFreezedInv()
    {
        invMissiles = missiles;
        invSuperBombs = superBombs;
        invBeams = beams;
        invCharged = charged;
        invPlasma = plasma;
        invSuperMissiles = superMissiles;
        invBombs = bombs;
    }
    private void SetDefaultInvulnerabilities()
    {
        invMissiles = defaultInv[invMissiles.ToString()];
        invSuperBombs = defaultInv[invSuperBombs.ToString()];
        invBeams = defaultInv[invBeams.ToString()];
        invCharged = defaultInv[invCharged.ToString()];
        invPlasma = defaultInv[invPlasma.ToString()];
        invSuperMissiles = defaultInv[invSuperMissiles.ToString()];
        invBombs = defaultInv[invBombs.ToString()];
    }
    public void FreezeMe()
    {
        CancelInvoke("Unfreeze");
        SetFreezedInv();
        StopAllCoroutines();
        if (rigidCol) rigidCol.enabled = false;
        freezedCol.SetActive(true); freezed = true;
        Invoke("Unfreeze", 4f);
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
        freezedCol.SetActive(false); hurtbox.enabled = false;
        Utilities.SetBehaviours(components, true);
        _renderer.material = dissolve;
        rb2d.constraints = RigidbodyConstraints2D.None;
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        Physics2D.IgnoreLayerCollision(8, 9, true);
        unFreezing = freezed = false;
        hurtbox.enabled = true;
        SetDefaultInvulnerabilities();
    }
    public void AddDamage(float amount)
    {
        health -= amount;
        OnDamage?.Invoke();
        if (health <= 0)
        {
            hurtbox.enabled = false;

            rb2d.velocity = Vector2.zero;

            StartCoroutine(nameof(Dissolve));
        }
        else StartCoroutine(VisualFeedBack());
    }
    #endregion
    #region Private methods
    IEnumerator Dissolve()
    {
        float fade = 1;
        _renderer.material = dissolve;
        DropManager.instance.TryToDrop(transform.position);
        foreach (var item in componentsToDisable)
        {
            item.enabled = false;
        }
        while (fade > 0)
        {
            dissolve.SetFloat("_Fade", fade -= Time.deltaTime);
            yield return null;
        }
        OnDeath?.Invoke();
    }
    private void DestroyOnDeath()
    {
        Destroy(gameObject.GetParent());
    }
    private IEnumerator VisualFeedBack()
    {
        _renderer.color = Color.red;
        yield return new WaitForSecondsRealtime(0.1f);
        _renderer.color = Color.white;
    }
    public IEnumerator FreezeVisualFeedBack()
    {
        _renderer.color = _renderer.color.Default();
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
        if (!freezed)
        {
            OnSideDamage?.Invoke(side);
            side = 0;

        }
    }
    #endregion
}
public enum EnemyType
{
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
    bool InvSpazer { get; }
}