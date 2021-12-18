using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public static class GameEvents
{
    public static Action<int, float> damagePlayer;
    public static Action<SaveStation> save;
    /// <summary>
    /// Displays the save message when player is using the save station.
    /// </summary>
    public static Action saveMessage;
    public static Action<MiniMap> UnexploredMap;
    /// <summary>
    /// Verifies the registry of reserve and items in the collector manager.
    /// </summary>
    public static Func<int,bool,bool> verifyRegistry;
    /// <summary>
    /// Enables the transition when player enters to a door.
    /// </summary>
    public static Action<CameraTransition> OnWarp;
    public static Action<bool> timeCounter;
    /// <summary>
    /// Goes directly to the minimap, used in map updater.
    /// </summary>
    public static Action MinimapShortcout;
    /// <summary>
    /// Starts the transition animation.
    /// </summary>
    public static Func<float> StartTransition;
    public static Action<Vector2> OnMissileImpact;
    public static Action<Vector2> OnCrumble;
}