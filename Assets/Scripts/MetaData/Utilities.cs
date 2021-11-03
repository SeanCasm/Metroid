using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class Utilities
{
    #region Color
    public static Color Default(this Color color)
    {
        Color newColor = color;
        newColor.r = newColor.g = newColor.b = 1;
        return color = newColor;
    }
    public static Color SetColorRGB(this Color color, float value)
    {
        return new Color(value, value, value);
    }
    #endregion 
    #region GameObject
    /// <summary>
    /// Gets the total childs count from gameObject.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static int ChildCount(this GameObject gameObject)
    {
        if (gameObject.GetChild(0))
        {
            return gameObject.transform.childCount;
        }
        else return 0;
    }
    /// <summary>
    /// Gets the parent gameobject.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static GameObject GetParent(this GameObject gameObject)
    {
        return gameObject.transform.parent.gameObject;
    }
    /// <summary>
    /// Gets a child from gameobject by index.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static GameObject GetChild(this GameObject gameObject, int index)
    {
        if (gameObject.transform.GetChild(index) != null) return gameObject.transform.GetChild(index).gameObject;
        else throw new ArgumentException("This parameter doesn't have a child", nameof(gameObject));
    }
    /// <summary>
    /// Gets all childs on gameObject.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static GameObject[] GetChilds(this GameObject gameObject)
    {
        if (gameObject.transform.GetChild(0))
        {
            int totalChilds = gameObject.transform.childCount;
            GameObject[] temp = new GameObject[totalChilds];
            for (int i = 0; i < totalChilds; i++)
            {
                temp[i] = gameObject.GetChild(i);
            }
            return temp;
        }
        else throw new ArgumentException("This parameter doesn't have any child", nameof(gameObject));
    }
    /// <summary>
    /// Gets gameObject childs until specific amount.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static GameObject[] GetChilds(this GameObject gameObject, int amount)
    {
        if (gameObject.transform.GetChild(0))
        {
            GameObject[] temp = new GameObject[amount];
            for (int i = 0; i < amount; i++)
            {
                temp[i] = gameObject.GetChild(i);
            }
            return temp;
        }
        else throw new ArgumentException("This parameter doesn't have any child", nameof(gameObject));
    }
    #endregion
    #region RigidBody
    public static Vector2 SetVelocity(this Rigidbody2D rigidbody, float xVelo, float yVelo)
    {
        return rigidbody.velocity = new Vector2(xVelo, yVelo);
    }
    public static Vector2 SetVelocity(this Rigidbody2D rigidbody, Vector2 vector)
    {
        return rigidbody.velocity = vector;
    }
    #endregion
    /// <summary>
    /// Disable or enable all components in a gameobject, trought on array of components.
    /// </summary>
    /// <param name="behaviours">the array of behaviours to set.</param>
    /// <param name="value">true: enable behaviour,false: disable behaviour</param>
    public static void SetBehaviours(Behaviour[] behaviours,bool value)
    {
        foreach(Behaviour element in behaviours)
        {
            if(!(element is Collider2D))element.enabled = value;
        }
    }
    /// <summary>
    /// Sets clip to audioSource and play it inmediatly
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="clip"></param>
    public static void ClipAndPlay(this AudioSource audioSource,AudioClip clip){
        audioSource.clip=clip;
        audioSource.Play();
    }
    /// <summary>
    /// Gets a random position of list.
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetRandom<T>(this List<T> list){
        int i= UnityEngine.Random.Range(0,list.Count);
        return list[i];
    }
}