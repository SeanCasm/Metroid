using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonID : MonoBehaviour
{
    [SerializeField] int iD;
    [SerializeField]ButtonUtilities buttonEssentials;
    public bool selected{get;set;}
    public int ID { get { return iD; } }
}
