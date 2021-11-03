using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetroidIA : MonoBehaviour
{
    public float visionRadius;
    
    public float speed;

    GameObject player;
    private bool _enemyDetected;
    private Animator _animator;
    Vector3 initialPosition;

    Animator anim;
    Rigidbody2D rb2d;
    // Start is called before the first frame update
    void Start()
    {
        
        player = GameObject.FindGameObjectWithTag("Player");
        initialPosition = transform.position;
        _animator = GetComponent<Animator>();
        rb2d= GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _enemyDetected = false;
        Vector3 target = initialPosition;
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if (dist < visionRadius)
        {
            target = player.transform.position;
            _enemyDetected = true;
        }
        float fixedSpeed = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, fixedSpeed);

    }
    void LateUpdate()
    {
        _animator.SetBool("EnemyDetected", _enemyDetected);
    }
}
