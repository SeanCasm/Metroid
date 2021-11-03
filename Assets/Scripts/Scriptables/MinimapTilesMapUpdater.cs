using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="new MinimapTilesMapUpdater",menuName ="ScriptableObjects/Minimap/MTMapUpdater")]
public class MinimapTilesMapUpdater : ScriptableObject
{
    public List<MiniMap> minimapScripts=new List<MiniMap>();
}
