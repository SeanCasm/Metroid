using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{
    public class EnemyBase : MonoBehaviour
    {
        [Header("Enemy settings")]
        [SerializeField] protected float speed;
        [SerializeField] protected bool detectPlayer;
        [Header("On became invisible")]
        [SerializeField] bool stopAll;
        public Animator anim { get; set; }
        public Rigidbody2D rigid { get; set; }
        protected EnemyHealth enemyHealth;
        protected PlayerDetector pDetect;
        private float curGravity;
        protected bool isVisible,isSubscribed;
        protected void Awake()
        {
            if (detectPlayer) pDetect = GetComponentInChildren<PlayerDetector>();
            anim = GetComponent<Animator>();
            enemyHealth = GetComponentInChildren<EnemyHealth>();
            rigid = GetComponent<Rigidbody2D>();
        }
        protected void OnEnable()
        {
            if(!isSubscribed){
                OnBecameInvisible();
                Pause.OnPause += SetBehaviour;
                isSubscribed=true;
            }
        }
        protected void OnDestroy()
        {
            Pause.OnPause -= SetBehaviour;
        }
        private void SetBehaviour(bool value)
        {
            if (isVisible)
            {
                if (!enemyHealth.freezed && value)
                {
                    SetVisible();
                }
                else if (!enemyHealth.freezed && !value)
                {
                    SetInvisible();
                }
            }
        }
        private void SetVisible()
        {
            anim.enabled = true;
            enabled = true;
            rigid.gravityScale = curGravity;
        }
        private void SetInvisible()
        {
            anim.enabled = false;
            enabled = false;
            curGravity = rigid.gravityScale;
            rigid.gravityScale = 0;
            rigid.velocity = Vector2.zero;
        }
        public void OnBecameInvisible()
        {
            isVisible = false;
            if (!enemyHealth.freezed && stopAll)
            {
                SetInvisible();
            }
        }
        public void OnBecameVisible()
        {
            isVisible = true;

            if (!enemyHealth.freezed && stopAll)
            {
                SetVisible();
            }
        }
    }
}