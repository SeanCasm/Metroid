using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegsAnimEvent : MonoBehaviour
{
    [SerializeField] GameObject claw;
    [SerializeField] Transform shootpointFront1, shootpointFront2;
    [SerializeField] Transform shootpointBehind1, shootpointBehind2;
    public void ShootClawFront()
    {
        Instantiate(claw, shootpointFront1.position,Quaternion.identity);
        Instantiate(claw, shootpointFront2.position,Quaternion.identity);
    }
    public void ShootClawBehind()
    {
        Instantiate(claw, shootpointBehind1.position, Quaternion.identity);
        Instantiate(claw, shootpointBehind2.position, Quaternion.identity);
    }
}
