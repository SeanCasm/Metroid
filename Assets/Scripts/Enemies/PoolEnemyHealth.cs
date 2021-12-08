using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolEnemyHealth : EnemyHealth
{
    private new void Start() {
        base.Start();
        OnDeath = ResetOnDeath;
    }
    private void ResetOnDeath()
    {
        health = maxHealth;
        dissolve.SetFloat("_Fade", 1);
        _renderer.color = Color.white;
        hurtbox.enabled = true;
        foreach (var item in componentsToDisable)
        {
            item.enabled = true;
        }
        gameObject.GetParent().SetActive(false);
    }
}
