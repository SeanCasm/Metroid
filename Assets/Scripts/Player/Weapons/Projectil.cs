using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player.Weapon
{
    public class Projectil : Weapon, IRejectable, IPooleable, IPlayerWeapon
    {
        [SerializeField] protected GameObject reject, impactClip;
        [SerializeField] protected float speed;
        [Tooltip("When beam collides generates this impact.")]
        [SerializeField] protected GameObject impactPrefab;
        [SerializeField] bool pooleable;
        protected Rigidbody2D rigid;
        protected IDamageable<float> health;
        private bool poolRemoved;
        protected bool collided;
        protected IInvulnerable iInvulnerable;

        public Transform parent { get; set; }
        bool IPooleable.pooleable { get => this.pooleable; set => this.pooleable = value; }

        #region Unity methods
        protected void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
        }
        protected void OnDisable()
        {
            Pool.OnPoolChanged -= PoolChanged;
            poolRemoved = false;
        }
        protected new void OnEnable()
        {
            base.OnEnable();
            collided = false;
            Invoke("BackToGun", livingTime);
            OverHeatBar.SetFill.Invoke(hotPoints);
            if (parent != null)
            {

                direction = parent.right;
                transform.eulerAngles = parent.eulerAngles;
            }
            Pool.OnPoolChanged += PoolChanged;
        }
        protected void FixedUpdate()
        {
            if (!rejected) rigid.velocity = direction.normalized * speed;
        }
        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                health = collision.GetComponent<IDamageable<float>>();
                iInvulnerable = collision.GetComponent<IInvulnerable>();
                if (health == null && iInvulnerable != null)
                {
                    Instantiate(impactPrefab, transform.position, Quaternion.identity, null);
                    Reject();
                }
                else if (health != null && iInvulnerable != null)
                {
                    TryDoDamage(damage, health, beamType, iInvulnerable);
                    if (!collided) Instantiate(impactPrefab, transform.position, Quaternion.identity, null);
                    if (rejected) Reject();
                }
                BackToGun();
                return;
            }
            else CheckCollisions(collision);
        }
        protected void CheckCollisions(Collider2D collision)
        {
            if (collision.CompareTag("EnemyBeam"))
            {
                IDrop iDrop = collision.GetComponent<IDrop>();
                if (iDrop != null) FloorCollision();
            }
            else if (collision.CompareTag("Suelo") && collision.name != "freezed") FloorCollision();
            else if (collision.CompareTag("Crumble"))
            {
                collision.GetComponent<CrumbleBlock>().CheckCollision(gameObject.tag, FloorCollision);
            }
        }
        private void PoolChanged()
        {
            poolRemoved = true;
        }
        protected virtual void BackToGun()
        {
            if (poolRemoved || !pooleable) Destroy(gameObject);
            else
            {
                transform.SetParent(parent);
                transform.position = parent.position;
                gameObject.SetActive(false);
            }
            collided = true;
            rejected = false;
        }
        public void FloorCollision()
        {
            if (impactClip) Instantiate(impactClip);
            if (!collided) Instantiate(impactPrefab, transform.position, Quaternion.identity, null);
            BackToGun();
        }
        protected void OnBecameInvisible()
        {
            if (!collided) BackToGun();
        }
        #endregion
        public void Reject()
        {
            Instantiate(reject);
            BackToGun();
        }
    }
}
