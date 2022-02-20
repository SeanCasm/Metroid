using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player.Weapon
{
    public class SpazerChild : Projectil
    {
        [SerializeField] float yOffset;
        private new void OnEnable()
        {
            Invoke("BackToGun", livingTime);
            direction = parent.right;
            transform.localPosition = new Vector2(transform.localPosition.x, yOffset);
            transform.SetParent(null);
        }
        protected override void BackToGun()
        {
            transform.SetParent(parent);
            transform.position = parent.position;
            gameObject.SetActive(false);
            collided = true;
            rejected = false;
        }

    }

}