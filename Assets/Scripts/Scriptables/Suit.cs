using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new Suit", menuName = "ScriptableObjects/Player/Suit")]
public class Suit : ScriptableObject
{
    public Sprite portait;
    public Sprite[] suitLeft,suitRight;
}
