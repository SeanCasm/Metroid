using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Weapons;
using Enemy;
using System;
using UnityEngine.Events;

public class PlayerKnockBack : MonoBehaviour
{
    #region Properties
    [SerializeField] Player.PowerUps.Shinespark shinespark;
    [SerializeField] PlayerHealth health;
    [SerializeField] float knockBackPowUp, knockBackPowHor;
    [SerializeField] UnityEvent knockBack;
    private float dir;
    PlayerController player;
    #endregion
    #region Unity Methods
    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
        shinespark.Init();
    }
    #endregion
    #region Private Methods

    public void HitPlayer(int damage, float xPosition)
    {
        if (!health.invulnerable)
        {
            knockBack?.Invoke();
            Hitted(transform.position.x, xPosition,damage);
        }
    }
    /// <summary>
    /// Set the direction of the player knock back depending of player position in X axis and
    /// the collision position in X axis, at any animation state except balled.
    /// </summary>
    /// <param name="myXPosition">player X position</param>
    /// <param name="collisionX">collision X position</param>
    private void Hitted(float myXPosition, float collisionX,int damage)
    {
        health.AddDamage(damage);

        if (collisionX >= myXPosition) { dir = -1; player.leftLook = false; player.OnLeft(false); }
        else { player.leftLook = true; player.OnLeft(true); dir = 1; }
        if(!player.groundOverHead) player.SetVelocity(new Vector2(dir * knockBackPowHor, knockBackPowUp));
        Invoke("EnableMovement", .5f);
    }
    private void EnableMovement()=>player.RestoreValuesAfterHit();
    #endregion
}