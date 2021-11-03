using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Weapon;
namespace Player.Weapon
{
    public class Bomb : Weapon,IPooleable
    {
        [SerializeField] protected AudioClip bombExplosion;
        [SerializeField] protected float timeToExplode;
        [SerializeField] bool pooleable;
        protected Animator animator;
        protected AudioSource audioPlayer;
        protected IDamageable<float> health;
        protected IInvulnerable iInvulnerable;
        protected System.Action OnExplosion;
        public Transform parent { get; set; }
        bool IPooleable.pooleable { get => this.pooleable; set => this.pooleable=value; }
        #region Unity methods
        protected new void OnEnable() {
            base.OnEnable();
            OverHeatBar.SetFill.Invoke(hotPoints);
            Invoke("Explode", timeToExplode);
        }
        protected void Start() {
            audioPlayer = GetComponent<AudioSource>();
            animator=GetComponent<Animator>();
        }
        protected void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Enemy"))
            {
                health = col.GetComponent<IDamageable<float>>();
                iInvulnerable = col.GetComponent<IInvulnerable>();
                if (health != null && iInvulnerable != null)
                {
                    TryDoDamage(damage, health, beamType, iInvulnerable);
                }
            }
        }
        #endregion
        private void Explode()=>animator.SetTrigger("Explode");
        #region Public methods
        public void PlayExplosion()
        {
            audioPlayer.clip = bombExplosion;
            audioPlayer.Play();
        }
        private void Destroy()
        {
            OnExplosion?.Invoke();
            transform.SetParent(parent);
            animator.Rebind();
            transform.position = parent.position;
            gameObject.SetActive(false);
        }

        #endregion 
    }
}

