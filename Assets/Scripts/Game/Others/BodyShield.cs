using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{
    public class BodyShield : MonoBehaviour, IInvulnerable
    {
        [SerializeField]bool invMissile,invSuperMissile,invBomb,invSuperBomb,invFreeze,invBeam,invPlasma,invSpazer;
        public bool InvMissiles => invMissile;

        public bool InvSuperMissiles => invSuperMissile;
        public bool InvPlasma=>invPlasma;

        public bool InvBeams => invBeam;

        public bool InvBombs => invBomb;

        public bool InvSuperBombs => invSuperBomb;

        public bool InvFreeze => invFreeze;

        public bool InvSpazer => invSpazer;
    }
}
 
