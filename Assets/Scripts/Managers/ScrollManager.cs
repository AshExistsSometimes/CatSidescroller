using System.Collections.Generic;
using UnityEngine;

// Manages horizontal scrolling of level backgrounds with parallax layers.
// Handles pausing/resuming during boss fights or menus.
public class ScrollManager : MonoBehaviour
{
    public static ScrollManager Instance;

    [System.Serializable]
    public class ScrollLayer
    {
        public string layerName;
        public float scrollSpeed = 1f;
        public int layerDepth = 0;
        public List<Transform> panels = new List<Transform>();
    }

    [Header("Scrolling Layers")]
    [Tooltip("Each layer group represents a depth that scrolls at a different speed for parralax.")]
    public List<ScrollLayer> layers = new List<ScrollLayer>();

    [Header("Scroll Settings")]
    public float baseScrollSpeed = 5f;
    public bool isScrolling = true;

    private float screenWidth;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Calculate world-space screen width based on main camera
        float cameraHeight = 2f * Camera.main.orthographicSize;
        screenWidth = cameraHeight * Camera.main.aspect;
    }

    private void Update()
    {
        if (!isScrolling) return;

        ScrollAllLayers();
    }

    // Scrolls all active parallax layers horizontally and loops them when they exit the screen.
    private void ScrollAllLayers()
    {
        foreach (var layer in layers)
        {
            float layerSpeed = baseScrollSpeed * layer.scrollSpeed / (layer.layerDepth + 1f);

            for (int i = 0; i < layer.panels.Count; i++)
            {
                Transform panel = layer.panels[i];
                panel.Translate(Vector3.left * layerSpeed * Time.deltaTime);

                // If the panel exits fully to the left, move it to the far right
                if (panel.position.x < -screenWidth)
                {
                    float rightmostX = GetRightmostPanelX(layer.panels);
                    Vector3 newPos = new Vector3(rightmostX + screenWidth, panel.position.y, panel.position.z);
                    panel.position = newPos;
                }
            }
        }
    }

    // Finds the rightmost panel in the list for looping calculations.
    private float GetRightmostPanelX(List<Transform> panels)
    {
        float maxX = float.MinValue;
        foreach (var p in panels)
        {
            if (p.position.x > maxX)
                maxX = p.position.x;
        }
        return maxX;
    }

    // Stops scrolling, typically during boss fights or pause menus.
    public void StopScrolling()
    {
        isScrolling = false;
    }

    // Resumes scrolling after pause or boss fight.
    public void ResumeScrolling()
    {
        isScrolling = true;
    }

    /// Dynamically sets scroll speed (for slowing or speeding up).
    public void SetScrollSpeed(float newSpeed)
    {
        baseScrollSpeed = newSpeed;
    }
}

