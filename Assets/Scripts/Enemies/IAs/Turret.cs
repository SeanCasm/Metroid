using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Weapons;
public class Turret : MonoBehaviour
{
    [SerializeField] GameObject ammo;
    [SerializeField] Transform shootPoint,shooter;
    Transform player;
    Vector2 direction;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            player=other.transform;
            StartCoroutine(FollowPlayer());
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            player = null;
            StopAllCoroutines();
        }
    }
    IEnumerator FollowPlayer(){
        StartCoroutine(Attack());
        while(player!=null){
            direction = player.position - shooter.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            shooter.rotation = Quaternion.Slerp(shooter.rotation, rotation, 0.44f);
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator Attack(){
        while (player != null)
        {
            yield return new WaitForSeconds(3.5f);
            Shoot();
        }
    }
    private void Shoot(){
        GameObject bullet=Instantiate(ammo,shootPoint.position,shooter.rotation,null);
        bullet.GetComponent<Weapon>().Direction=direction;
    }
}
