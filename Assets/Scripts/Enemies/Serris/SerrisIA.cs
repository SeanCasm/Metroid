using System.Collections;
using System.Collections.Generic;
using PathCreation.Examples;
using UnityEngine;
using EnemyBoss.Serris;
public class SerrisIA : Boss
{
    #region Properties
    [SerializeField]List<SpriteRenderer> bodyRenderers=new List<SpriteRenderer>();
    [SerializeField]List<PathFollower> bodyPaths=new List<PathFollower>();
    [SerializeField]int boostCount;
    [SerializeField] float timeOffsetBodyParts,timeOffsetHead,rageSpeed;
    [SerializeField]Animator head,body;
    private PathFollower path;
    public bool invulnerable{get;private set;}
    float calmSpeed;
    private bool firstEntry;
    private int damageCount;
    public int DamageCount{get{
        return damageCount;
    }
    set{
        damageCount=value;
            if (damageCount == boostCount && !invulnerable)
            {
                invulnerable = true;
                Invoke("StopBoost", Random.Range(3, 6));
                SpeedSwapper(rageSpeed);
                OnRage?.Invoke();
            }
    }}
    public System.Action OnRage,OutRage;
    bool headDelayed ;
    #endregion
    private void Awake() {
        path = GetComponent<PathCreation.Examples.PathFollower>();
        path.speed=0;
        damageCount=0;
        StartCoroutine(WaitToHead());
        Invoke("HeadSpeedDelay",timeOffsetHead);
    }
    new void Start() {
        base.Start();
    }
    #region Serris initialization
    private void HeadSpeedDelay(){
        calmSpeed=path.speed=2.5f;
        headDelayed=true;
    }
    private void SetBodyParts(){
        bodyPaths.ForEach(item =>
        {
            item.pathCreator = path.pathCreator;

        });
        StartCoroutine(EnableSpeed());
    }
    IEnumerator WaitToHead(){
        while(!headDelayed){
            yield return null;
        }
        SetBodyParts();
    }
    IEnumerator EnableSpeed(){
        int count=0;
        while(count<bodyPaths.Count)
        {
            yield return new WaitForSeconds(timeOffsetBodyParts);
            bodyPaths[count].speed = this.path.speed;
            count++;
        }
    }
    #endregion
    private void LateUpdate() {
        head.SetBool("Injured",invulnerable);
        body.SetBool("Injured",invulnerable);
    }
    void SpeedSwapper(float amount){
        path.speed=amount;
        bodyPaths.ForEach(item=>{
            item.speed=amount;
        });
    }
    void StopBoost(){
        invulnerable=false;
        damageCount=0;
        SpeedSwapper(calmSpeed);
        OutRage?.Invoke();
    }
    new public void OnDeath(){
        Instantiate(DropManager.instance.reloadAll, transform.position, Quaternion.identity, null);
        Destroy(gameObject);
        base.OnDeath();
    } 
}
