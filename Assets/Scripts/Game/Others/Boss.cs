using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] protected int iD;
    public List<Sensor> roomDoors= new List<Sensor>();
    protected bool quarterReached, halfReached, lowReached;
    public int ID { get=>iD; }
    public static List<int> defeateds { get; set; } = new List<int>();
    protected void Start()
    {
        quarterReached =halfReached=lowReached = false;
    }
    public void OnDeath()
    {
        foreach (Sensor element in roomDoors)
        {
            element.EnableDoor();
        }
        defeateds.Add(iD);
    }
}
