using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy.Weapons
{
    public class Weapon : WeaponBase<int>,IPooleable
    {
        [SerializeField]protected float speed;
        [SerializeField]bool pooleable;
        public Transform parent{get;set;}
        public Vector3 playerPosition{get;set;}
        private float rotationZ;
        protected Rigidbody2D rigid;
        public Vector3 Direction{get{return direction;}set{direction=value;}}

        bool IPooleable.pooleable { get => this.pooleable; set => this.pooleable=value; }
       
        protected virtual new void OnEnable() {
            base.OnEnable();
            playerPosition = PlayerController.current.TransformCenter();
            Invoke("BackToShootPoint", livingTime);
        }
        protected void Start(){
            rigid=GetComponent<Rigidbody2D>();
        }
        protected virtual void OnBecameInvisible()
        {
            BackToShootPoint();
        }
        protected void BackToShootPoint(){
            CancelInvoke(nameof(BackToShootPoint));
            if(!pooleable){
                Destroy(gameObject);
            }else{
                transform.position = parent.position;
                transform.SetParent(parent);
                gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// Looks to the player.
        /// </summary>
        public void SetDirectionAndRotation()
        {
            SetDirection();
            rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);        
        }
        public void SetDirectionAndRotationLimit(float zLimit)
        {
            SetDirection();
            rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation=Quaternion.Euler(0,0,rotationZ);
            direction = transform.right;
        }
        
        public void SetDirectionAround(float zDegrees)
        {
            direction = Quaternion.Euler(0,0,zDegrees)*Vector2.right;
            transform.eulerAngles=new Vector3(0,0,zDegrees);
        }
        protected void OnTriggerEnter2D(Collider2D collision)
        {
            switch(collision.tag){
                case "Player":
                    GameEvents.damagePlayer.Invoke(damage, transform.position.x);
                    BackToShootPoint();
                break;
                case "Suelo":BackToShootPoint();
                break;
            }
        }
        protected void FixedUpdate()
        {
            rigid.MovePosition(transform.position + direction * Time.deltaTime*speed);
        }
        protected void SetDirection()=> direction = (playerPosition - transform.position).normalized;
        private Vector2 Vector2FromAngle(float a)
        {
            a *= Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
        }
        protected void DoDrop()
        {
            DropManager.instance.TryToDrop();
            BackToShootPoint();
        }
    }
}