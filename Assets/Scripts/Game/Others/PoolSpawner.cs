using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PoolSpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab; 
    private List<GameObject> enemies=new List<GameObject>();
    private PlayerController playerController;
    int i=0;
    private void Start() {
        for(int i=0;i<8;i++){
            enemies.Add(Instantiate(enemyPrefab, transform.position, Quaternion.identity, transform));
        }
    }
 
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            playerController=other.GetComponentInParent<PlayerController>();
            InvokeRepeating("EnablePool",2.5f,5.5f);
        }    
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player")) {
            playerController=null;
            CancelInvoke("EnablePool");
        }
    }
    void EnablePool()
    {
        if(i>7)i=0;
        if(!enemies[0].activeSelf){
            AvispaIA enemy = enemies[i].GetComponent<AvispaIA>();
            enemy.gameObject.transform.localScale = transform.position.x > playerController.TransformCenter().x ? new Vector2(-1, 1) : new Vector2(1, 1);
            enemy.y = playerController.TransformCenter().y;
            enemy.gameObject.SetActive(true);
            i++;
        }
    }
}