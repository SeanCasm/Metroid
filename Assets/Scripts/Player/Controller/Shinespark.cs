using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Events;

namespace Player.PowerUps
{
    public class Shinespark : MonoBehaviour
    {
        [SerializeField] InputManager inputManager;
        [SerializeField] float shinesParkVel;
        [SerializeField] Transform coll;
        [SerializeField] Rigidbody2D rb;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] AssetReference powerGhost;
        [SerializeField] SpeedBooster speedBooster;
        [SerializeField] PlayerHealth health;
        [SerializeField] PlayerFXHandler playerFX;
        [SerializeField] Behaviour[] behaviours;
        [SerializeField] UnityEvent OnFulled,OnNone,OnCollided;
        private GameObject pgClone1, pgClone2;
        private PlayerController playerController;
        private ShinesparkState _shinesparkState = ShinesparkState.None;
        public ShinesparkState ShinesparkState
        {
            get => _shinesparkState;
            set
            {
                _shinesparkState = value;
                switch (_shinesparkState)
                {
                    case ShinesparkState.Full:
                        OnFulled?.Invoke();
                        foreach(var e in behaviours)e.enabled=true;
                        break;
                    case ShinesparkState.None:
                        OnNone?.Invoke();
                        foreach (var e in behaviours) e.enabled = false;
                        break;
                }
            }
        }
        private Vector2 hyperJumpDir;
        private void Start() {
            playerController=GetComponentInParent<PlayerController>();
        }
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Enemy"))
            {
                EnemyHealth enemy = col.GetComponent<EnemyHealth>();
                if (playerController.status == Status.Powered)
                {
                    enemy.AddDamage(999);
                }
            }
            else
            if ((col.CompareTag("Suelo") || col.CompareTag("Crumble")) && ShinesparkState == ShinesparkState.Full && !col.GetComponent<SpeedBlock>())
            {
                OnCollision();
                Invoke("DisableShinespark", 1.1f);
                playerFX.StopAudio(StopAction.HyperJump);
                health.AddDamage(20);
                speedBooster.CancelGhost();
            }
        }
        private void DisableShinespark()
        {
            DisableGhost();
            playerController.ResetState();
            inputManager.EnablePlayerInput();
            OnCollided.Invoke();
        }
        private void DisableGhost()
        {
            pgClone1.SetActive(false);
            pgClone2.SetActive(false);
            pgClone1.transform.localPosition = Vector2.zero;
            pgClone2.transform.localPosition = Vector2.zero;

            ShinesparkState = ShinesparkState.None;
        }
        public void Init()
        {
            powerGhost.InstantiateAsync(transform.position, Quaternion.identity, transform).Completed += OnCompleted;
            powerGhost.InstantiateAsync(transform.position, Quaternion.identity, transform).Completed += OnCompleted;
        }
        private void OnCompleted(AsyncOperationHandle<GameObject> obj)
        {
            if (pgClone1 == null){
                pgClone1 = obj.Result;
                pgClone1.GetComponent<Screw>().pContr=GetComponentInParent<PlayerController>();
            } 
            else{
                pgClone2 = obj.Result;
                pgClone1.GetComponent<Screw>().pContr=GetComponentInParent<PlayerController>();
            }

        }
        public void SetJumpToDirection(Vector2 dir)
        {
            hyperJumpDir = dir;
            ShinesparkState = ShinesparkState.Full;
        }
        public void SetJumpAngleUp(bool leftLook)
        {
            if (leftLook) hyperJumpDir = Vector2.left+Vector2.up;
            else hyperJumpDir = Vector2.one;
            ShinesparkState = ShinesparkState.Full;
        }
        public void SetJumpUp()
        {
            hyperJumpDir = Vector2.up;
            ShinesparkState = ShinesparkState.Full;
        }
        private void OnCollision()
        {
            Vector2 perp = Vector2.Perpendicular(hyperJumpDir);
            hyperJumpDir = Vector2.zero;
            pgClone1.SetActive(true);
            pgClone2.SetActive(true);
            pgClone1.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
            pgClone2.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
            StartCoroutine(MovePowerGhost(perp));
        }
        IEnumerator MovePowerGhost(Vector2 perp)
        {
            while (pgClone1.activeSelf)
            {
                //Debug.DrawRay(transform.position,perp*10,Color.red);
                //Debug.DrawRay(transform.position,-perp*10,Color.red);
                pgClone1.transform.Translate(perp * Time.deltaTime*3,Space.World);
                pgClone2.transform.Translate(-perp * Time.deltaTime * 3, Space.World);
                yield return null;
            }
            this.enabled=false;
        }
        void FixedUpdate(){
            rb.velocity = hyperJumpDir * shinesParkVel * Time.deltaTime;
            transform.position = coll.position;
        }
        private void OnEnable(){
            OnFulled?.Invoke();
            inputManager.DisablePlayerInput();
        }
        private void OnDisable() => OnNone?.Invoke();
    }
}

public enum ShinesparkState
{
    None = 0, Prepared = 1, Full = 2, Charged = 4
}
