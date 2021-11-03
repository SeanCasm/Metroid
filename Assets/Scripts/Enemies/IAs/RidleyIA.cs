using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PathCreation.Examples;
public class RidleyIA : Boss
{
    [SerializeField] Materials materials;
    [SerializeField] float speed, beginNeutralTime, projectilRepeatRate;
    [SerializeField] int totalAttackSize;
    [SerializeField] Animator animHead, animNeck, animBody;
    [SerializeField] List<SpriteRenderer> ridleyAllRenderers;
    [SerializeField] GameObject fireBall, firePoint, blackHole;
    GameObject player;
    private (float time, bool ultimate)[] attackPattern;
    private PathFollower path;
    float currentSpeed, pathSpeed;
    BossHealth health;
    bool attacking, onUltimate, newPatterns;
    void Awake()
    {
        path = GetComponent<PathCreation.Examples.PathFollower>();
        health = GetComponentInChildren<BossHealth>();
        player = FindObjectOfType<PlayerController>().gameObject;
        health.OnDamage+=OnDamage;
    }
    private void OnDisable() {
        health.OnDamage -= OnDamage;
    }
    new void Start()
    {
        base.Start();
        pathSpeed = path.speed;
        currentSpeed = speed;
        attackPattern = new (float time, bool ultimate)[totalAttackSize];
        FillAttackPattern();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x > transform.position.x) transform.localScale = new Vector3(-1, 1);
        else transform.localScale = new Vector3(1, 1);
    }
    private void LateUpdate()
    {
        animHead.SetBool("attacking", attacking);
        animNeck.SetBool("attacking", attacking);
    }
    private void OnDamage(){
        if (health.MyHealth < health.TotalHealth / 2.25f && !newPatterns)
        {
            FillUltimateAttackPatern();
            newPatterns = true;
        }
    }
    private void FillAttackPattern()
    {
        for (int i = 0; i < attackPattern.Length; i++)
        {
            attackPattern[i].time = Random.Range(3.5f, 6.2f);
        }
        StartCoroutine("Attack");
    }
    private void FillUltimateAttackPatern()
    {
        for (int i = 0; i < attackPattern.Length; i++)
        {
            attackPattern[i].time = Random.Range(3.23f, 5.4f);
            attackPattern[i].ultimate = Random.Range(1, 3) == 2 ? true : false;
        }
        StartCoroutine("Attack");
    }
    IEnumerator Attack()
    {
        int index = 0;
        yield return new WaitForSeconds(beginNeutralTime);//waits before start attack

        while (health.MyHealth > 0)
        {
            if (!attackPattern[index].ultimate)
            {
                StopCoroutine("AttackRate");
                attacking = true; StartCoroutine("AttackRate", fireBall);
            }
            else Ultimate();
            path.speed = 0;
            yield return new WaitForSeconds(attackPattern[index].time);//ends the attack
            if (onUltimate) UnsetUltimate();
            attacking = false;
            path.speed = pathSpeed;
            LoadMaterial(materials.defaultMaterial);
            index++;
            yield return new WaitForSeconds(attackPattern[index].time);//waits before start attack
            index++;
        }
    }
    private void LoadMaterial(Material material)
    {
        foreach (SpriteRenderer element in ridleyAllRenderers)
        {
            element.material = material;
        }
    }
    IEnumerator AttackRate(GameObject p)
    {
        while (attacking)
        {
            yield return new WaitForSeconds(projectilRepeatRate);
            GameObject o = Instantiate(p, firePoint.transform.position, Quaternion.identity);
            o.transform.SetParent(null);
        }
    }
    IEnumerator OnceUlti(GameObject p)
    {
        attacking=true;
        health.SetHealthColor(Color.white);
        yield return new WaitForSeconds(beginNeutralTime);
        GameObject o = Instantiate(p, firePoint.transform.position, Quaternion.identity);
        o.transform.SetParent(null);
        attacking=false;
        health.SetHealthColor(health.currentHealthColor);
    }
    void Ultimate()
    {
        StopCoroutine("AttackRate");
        StartCoroutine("OnceUlti", blackHole);
        onUltimate = true;
        LoadMaterial(materials.gold);
        health.GetComponent<Collider2D>().enabled = false;

    }
    void UnsetUltimate()
    {
        health.GetComponent<Collider2D>().enabled = true;
        onUltimate = false;
    }
}
