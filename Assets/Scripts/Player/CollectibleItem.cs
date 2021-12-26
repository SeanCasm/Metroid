using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CollectibleItem : MonoBehaviour
{
    [SerializeField] GameObject collectibleSound;
    [SerializeField] CollectibleType collectibleType;
    public int pointsRestoration;
    private void Start() {
        if(collectibleType!=CollectibleType.Special)
            Destroy(gameObject,5f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Gun gun = collision.GetComponentInParent<Gun>();
            PlayerHealth pHealth = collision.GetComponentInParent<PlayerHealth>();

            switch (collectibleType)
            {
                case CollectibleType.Missile:
                    gun.UpdateCapacity(0, pointsRestoration);
                    break;
                case CollectibleType.SuperMissile:
                    gun.UpdateCapacity(1, pointsRestoration);
                    break;
                case CollectibleType.SuperBomb:
                    gun.UpdateCapacity(2, pointsRestoration);
                    break;
                case CollectibleType.StickyBomb:
                    gun.UpdateCapacity(3, pointsRestoration);
                    break;
                case CollectibleType.Health:
                    pHealth.AddHealth(pointsRestoration);
                    break;
                case CollectibleType.Special:
                    gun.SetFullCapacity();
                    pHealth.SetFullCapacity();
                    break;
            }
            Instantiate(collectibleSound);
            Destroy(gameObject);
        }
    }
}
public enum CollectibleType { Missile, SuperMissile, SuperBomb, StickyBomb, Health, Special }
