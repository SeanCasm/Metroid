using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Weapon;
public class IceBeam : Beam, IBossFreezeable
{
    [SerializeField] AudioClip freezeClip;
    #region Unity Methods
    new void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable<float> health = other.GetComponent<IDamageable<float>>();
            IFreezeable ifreeze = other.GetComponent<IFreezeable>();
            IInvulnerable iInvulnerable = other.GetComponent<IInvulnerable>();
            if (ifreeze != null && health != null)
            {
                TryDoFreeze(health, ifreeze, iInvulnerable);
                if (!rejected) { Instantiate(impactPrefab, transform.position, Quaternion.identity, null); base.BackToGun(); }
                else Reject();
            }
            return;
        }
        else base.CheckCollisions(other);
    }
    new void OnBecameInvisible()
    {
        base.OnBecameInvisible();
    }
    #endregion
    private void TryDoFreeze(IDamageable<float> healthManager, IFreezeable ifreeze, IInvulnerable iInvulnerable)
    {
        if (!iInvulnerable.InvFreeze)
        {
            if (ifreeze.freezed && !ifreeze.unFreezing) healthManager.AddDamage(damage);
            ifreeze.FreezeMe();
        }
        else rejected = true;
    }
}
