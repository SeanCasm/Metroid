using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CollectibleItem : MonoBehaviour
{
    [SerializeField]GameObject collectibleSound;
    [SerializeField]CollectibleType collectibleType;
    public int pointsRestoration;
    void Start()
    {
        Destroy(gameObject,5f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") )
        {
            PlayerInventory pInventory=collision.GetComponentInParent<PlayerInventory>();
            PlayerHealth pHealth=collision.GetComponentInParent<PlayerHealth>();
            var ammo = pInventory.limitedAmmo;

            if (collectibleType==CollectibleType.Health)
            {
                pHealth.AddHealth(pointsRestoration);
            }
            else if(collectibleType!=CollectibleType.Special)
            {
                switch (collectibleType)
                {
                    case CollectibleType.Missile:
                        ammo[0].ActualAmmoCount(pointsRestoration);
                        break;
                    case CollectibleType.SuperMissile:
                        ammo[1].ActualAmmoCount(pointsRestoration);
                        break;
                    case CollectibleType.SuperBomb:
                        ammo[2].ActualAmmoCount(pointsRestoration);
                        break;
                    case CollectibleType.StickyBomb:
                        ammo[3].ActualAmmoCount(pointsRestoration);
                    break;
                }
            }else{
                GameEvents.refullAll.Invoke();
            }
            Instantiate(collectibleSound);
            Destroy(gameObject);
        }
    }
}
public enum CollectibleType{Missile, SuperMissile, SuperBomb,StickyBomb, Health, Special}
