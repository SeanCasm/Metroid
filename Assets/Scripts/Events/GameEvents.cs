using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameEvents:MonoBehaviour
{
    public static GameEvents instance;
    private void Awake() {
        instance=this;
    }
    public Action<MiniMap> UnexploredMap;
    /// <summary>
    /// Verifies the existence of reserve and items in the collector manager.
    /// </summary>
    public Func<int,bool,bool> CheckExistence;
    /// <summary>
    /// Goes directly to the minimap, used in map updater.
    /// </summary>
    public Action MinimapShortcout;
    /// <summary>
    /// Starts the transition animation.
    /// </summary>
    public Func<float> StartTransition;
    public Action<Vector2> OnMissileImpact;
    public Action<Vector2> OnCrumble;
}