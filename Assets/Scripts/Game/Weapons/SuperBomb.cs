using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Weapon;
public class SuperBomb : Bomb
{
    #region Unity methods
    private Collider2D coll2D;
    public class EnemyQueue{
        public IDamageable<float> health;
        public IInvulnerable iInv;
    }
    private Queue enemies=new Queue();
    new void Start()
    {
        base.Start();
        coll2D=GetComponent<Collider2D>();
    }
    new private void OnEnable() {
        base.OnEnable();
        InvokeRepeating("RepeatingDamage", .25f, .25f);
    }
    new void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            health = col.GetComponent<IDamageable<float>>();
            iInvulnerable = col.GetComponent<IInvulnerable>();
            if (health != null && iInvulnerable != null)
                enemies.Enqueue(new EnemyQueue{health=health,iInv=iInvulnerable});
        }
    }
    private void OnTriggerExit2D(Collider2D col) {
        if (col.CompareTag("Enemy") && enemies.Count>0)
        {
            var enemy=enemies.Dequeue() as EnemyQueue;
            TryDoDamage(damage,enemy.health,beamType,enemy.iInv);
        }
    }
    #endregion
    private void Explode()
    {
        animator.SetTrigger("Explode");
    }
    #region Public methods
    public new void PlayExplosion()
    {
        base.PlayExplosion();
    }
    public new void Destroy()
    {
        Destroy(gameObject);
    }
    #endregion
    private void RepeatingDamage()
    {
        coll2D.enabled=!coll2D.enabled;
    }
}
