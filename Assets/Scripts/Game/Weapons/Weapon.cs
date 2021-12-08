using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Weapon;
namespace Player.Weapon
{
    public class Weapon : WeaponBase<float>, IPlayerWeapon
    {
        [SerializeField] protected float hotPoints;
        [SerializeField] protected int iD,tier;
        [SerializeField] protected WeaponType beamType;
        public WeaponType BeamType => beamType;
        protected bool rejected;
        public int ID { get => iD; }

        public int weaponTier => tier;

        public void TryDoDamage(float damage, IDamageable<float> healthManager, WeaponType beamType, IInvulnerable iInvulnerable)
        {
            switch (beamType)
            {
                case WeaponType.Beam:
                    if (!iInvulnerable.InvBeams) healthManager.AddDamage(damage);
                    else rejected = true;
                    break;
                case WeaponType.Plasma:
                    if (!iInvulnerable.InvPlasma) healthManager.AddDamage(damage);
                    else rejected = true;
                    break;
                case WeaponType.Missile:
                    if (!iInvulnerable.InvMissiles) healthManager.AddDamage(damage);
                    else rejected = true;
                    break;
                case WeaponType.SuperMissile:
                    if (!iInvulnerable.InvSuperMissiles) healthManager.AddDamage(damage);
                    else rejected = true;
                    break;
                case WeaponType.Bomb:
                    if (!iInvulnerable.InvBombs) healthManager.AddDamage(damage);
                    break;
                case WeaponType.SuperBomb:
                    if (!iInvulnerable.InvSuperBombs) healthManager.AddDamage(damage);
                    break;
                case WeaponType.Spazer:
                    if (!iInvulnerable.InvSpazer) healthManager.AddDamage(damage);
                    break;
            }
            healthManager.SetDide(transform.position.x);
        }
    }
}
public enum WeaponType { Missile, SuperMissile, Beam, IceBeam, Plasma, Bomb, SuperBomb, Spazer, All }