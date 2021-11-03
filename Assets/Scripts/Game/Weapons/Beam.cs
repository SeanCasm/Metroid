using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Weapon;
namespace Player.Weapon{
    public class Beam : Projectil
    {
        #region Unity methods
        new private void Awake() {
            base.Awake();
        }
        new void OnEnable() {
            base.OnEnable();
        }
        protected new void OnDisable()
        {
            base.OnDisable();
        }
        new void FixedUpdate()
        {
            base.FixedUpdate();
        }
        new void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
        }
        public new void OnBecameInvisible()
        {
            base.OnBecameInvisible();
        }
        #endregion
        new public void Reject()
        {
            base.Reject();
        }
    }
}
 