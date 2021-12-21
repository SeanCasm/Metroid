using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CollectibleItem : MonoBehaviour
{
    [SerializeField] GameObject collectibleSound;
    [SerializeField] CollectibleType collectibleType;
    public int pointsRestoration;
    void Start()
    {
        Destroy(gameObject, 5f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Gun pInventory = collision.GetComponentInParent<Gun>();
            PlayerHealth pHealth = collision.GetComponentInParent<PlayerHealth>();
            var ammo = pInventory.limitedAmmo;

            if (collectibleType == CollectibleType.Health)
            {
                pHealth.AddHealth(pointsRestoration);
            }
            else if (collectibleType != CollectibleType.Special)
            {
                switch (collectibleType)
                {
                    case CollectibleType.Missile:
                        pInventory.UpdateCapacity(0, pointsRestoration);
                        break;
                    case CollectibleType.SuperMissile:
                        pInventory.UpdateCapacity(1, pointsRestoration);
                        break;
                    case CollectibleType.SuperBomb:
                        pInventory.UpdateCapacity(2, pointsRestoration);
                        break;
                    case CollectibleType.StickyBomb:
                        pInventory.UpdateCapacity(3, pointsRestoration);
                        break;
                }
            }
            else
            {
                pInventory.SetFullCapacity();
                pHealth.SetFullCapacity();
            }
            Instantiate(collectibleSound);
            Destroy(gameObject);
        }
    }
}
public enum CollectibleType { Missile, SuperMissile, SuperBomb, StickyBomb, Health, Special }
