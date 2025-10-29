using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewZoneData", menuName = "Data/Zone Data")]
public class ZoneData : ScriptableObject
{
    [Header("Zone Identification")]
    public string zoneName;
}
