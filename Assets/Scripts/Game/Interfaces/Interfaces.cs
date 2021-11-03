using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public interface IPooleable{
    Transform parent{get;set;}
    bool pooleable{get;set;}
}
public interface IBossFreezeable{

}
public interface IFreezeUndamageable{
    
}
 
public interface IBeamsInvulnerable{

    bool InvMissiles { get; }
    bool InvSuperMissiles { get; }
    bool InvBeams { get; }
}
public interface IPlayerWeapon{
}
public interface IDrop{

}
public interface IRejectable{
    void Reject();
}
public interface ICollecteable{
    
}