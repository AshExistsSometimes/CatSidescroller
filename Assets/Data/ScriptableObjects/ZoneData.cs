using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewZoneData", menuName = "Data/Zone Data")]
public class ZoneData : ScriptableObject
{
    [Header("Zone Identification")]
    public string zoneName;

    [Header("Layered Background Prefabs")]
    public List<LayerData> layers = new List<LayerData>();
}

[System.Serializable]
public class LayerData
{
    [Tooltip("Prefab to spawn for this layer (can be tiled or single).")]
    public GameObject prefab;

    [Tooltip("Determines which parallax layer this is.")]
    public LayerType layerType;

    [Tooltip("Parallax movement multiplier (lower = moves slower with camera).")]
    public float parallaxMultiplier = 1f;

    [Tooltip("Should this layer tile horizontally to fill screen width?")]
    public bool tileHorizontally = true;

    [Tooltip("Number of tiles to spawn (if tileHorizontally == true).")]
    public int tileCount = 3;
}

public enum LayerType
{
    Background = 0,   // Sky / distant hills
    Environment = 1,  // Trees / far objects
    Ground = 2,       // Player platform
    Foreground = 3    // Closest decorations
}

