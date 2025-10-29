using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages horizontal scrolling of background layers with parallax speed differences.
/// Properly tiles panels edge-to-edge, loops seamlessly, and fills the screen on start.
/// </summary>
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
        float cameraHeight = 2f * Camera.main.orthographicSize;
        screenWidth = cameraHeight * Camera.main.aspect;

        AlignAllLayersAtStart();
    }

    private void Update()
    {
        if (isScrolling)
            ScrollAllLayers();
    }

    /// <summary>
    /// Scrolls all panels and repositions those that exit the screen to the far right.
    /// </summary>
    private void ScrollAllLayers()
    {
        foreach (var layer in layers)
        {
            if (layer.panels == null || layer.panels.Count == 0)
                continue;

            float layerSpeed = baseScrollSpeed * layer.scrollSpeed / (layer.layerDepth + 1f);

            foreach (var panel in layer.panels)
            {
                panel.Translate(Vector3.left * layerSpeed * Time.deltaTime);
            }

            // Handle looping
            foreach (var panel in layer.panels)
            {
                float panelWidth = GetPanelWidth(panel);
                float leftEdge = panel.position.x - (panelWidth / 2f);

                // When panel exits fully left, reposition it to rightmost panel
                if (leftEdge < -screenWidth / 2f - panelWidth)
                {
                    float rightmostX = GetRightmostPanelRightEdge(layer.panels);
                    panel.position = new Vector3(rightmostX + (panelWidth / 2f), panel.position.y, panel.position.z);
                }
            }
        }
    }

    /// <summary>
    /// Calculates world-space width of a panel based on its SpriteRenderer.
    /// </summary>
    private float GetPanelWidth(Transform panel)
    {
        SpriteRenderer sr = panel.GetComponent<SpriteRenderer>();
        if (sr != null)
            return sr.bounds.size.x;
        return screenWidth;
    }

    /// <summary>
    /// Returns the world-space rightmost edge among all panels.
    /// </summary>
    private float GetRightmostPanelRightEdge(List<Transform> panels)
    {
        float maxRightEdge = float.MinValue;
        foreach (var p in panels)
        {
            if (p == null) continue;
            float width = GetPanelWidth(p);
            float rightEdge = p.position.x + (width / 2f);
            if (rightEdge > maxRightEdge)
                maxRightEdge = rightEdge;
        }
        return maxRightEdge;
    }

    /// <summary>
    /// Positions panels so they fill the screen from left to right, edge to edge.
    /// </summary>
    public void AlignAllLayersAtStart()
    {
        if (Camera.main == null) return;

        float cameraCenterX = Camera.main.transform.position.x;
        float cameraLeftEdge = cameraCenterX - (screenWidth / 2f);

        foreach (var layer in layers)
        {
            if (layer.panels == null || layer.panels.Count == 0)
                continue;

            float nextX = cameraLeftEdge;
            for (int i = 0; i < layer.panels.Count; i++)
            {
                Transform panel = layer.panels[i];
                float panelWidth = GetPanelWidth(panel);

                // Center panel on its intended position
                float centerX = nextX + (panelWidth / 2f);
                panel.position = new Vector3(centerX, panel.position.y, panel.position.z);

                // Move next panel directly after this one
                nextX += panelWidth;
            }
        }
    }

    public void StopScrolling() => isScrolling = false;
    public void ResumeScrolling() => isScrolling = true;
    public void SetScrollSpeed(float newSpeed) => baseScrollSpeed = newSpeed;
}

