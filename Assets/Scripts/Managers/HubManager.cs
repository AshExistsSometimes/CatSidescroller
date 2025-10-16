using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class HubManager : MonoBehaviour
{
    public static HubManager Instance;

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject levelSelectPanel;
    public GameObject shopPanel;
    public GameObject settingsPanel;

    [Header("Level Select")]
    public Transform levelButtonContainer;
    public GameObject levelButtonPrefab;

    [Header("Player Stats")]
    public TMP_Text playerLevelText;
    public Slider playerXPSlider;
    public TMP_Text playerMoneyText;

    [Header("Item Display")]
    public Image teaIcon;
    public TMP_Text teaCountText;
    public Image milkIcon;
    public TMP_Text milkCountText;
    public Image elixirIcon;
    public TMP_Text elixirCountText;

    [Header("Available Levels")]
    public List<LevelData> availableLevels = new List<LevelData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SaveLoadManager.Instance.LoadProgress();

        RefreshPlayerStats();
        RefreshItemDisplay();
        PopulateLevelSelect();
        ShowMainMenu();
    }

    /// <summary>
    /// Updates player stat texts.
    /// </summary>
    public void RefreshPlayerStats()
    {
        var p = SaveLoadManager.Instance.progress;
        playerLevelText.text = $"Level: {p.playerLevel}";
        playerXPSlider.value = p.playerExp;
        playerXPSlider.maxValue = p.nextLevelExp;
        playerMoneyText.text = $"Money: {p.money}";
    }

    /// <summary>
    /// Updates item counts and greys out icons if none are owned.
    /// </summary>
    public void RefreshItemDisplay()
    {
        var p = SaveLoadManager.Instance.progress;

        UpdateItemUI(teaIcon, teaCountText, p.teaCount);
        UpdateItemUI(milkIcon, milkCountText, p.milkCount);
        UpdateItemUI(elixirIcon, elixirCountText, p.elixirCount);
    }

    /// <summary>
    /// Updates a single item slot's visuals and count.
    /// </summary>
    private void UpdateItemUI(Image icon, TMP_Text countText, int count)
    {
        countText.text = $"x{count}";
        bool hasItem = count > 0;

        icon.color = hasItem ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        countText.color = hasItem ? Color.white : new Color(0.7f, 0.7f, 0.7f, 0.6f);
    }

    /// <summary>
    /// Dynamically generates level buttons.
    /// </summary>
    private void PopulateLevelSelect()
    {
        foreach (Transform child in levelButtonContainer)
            Destroy(child.gameObject);

        foreach (LevelData level in availableLevels)
        {
            GameObject btnObj = Instantiate(levelButtonPrefab, levelButtonContainer);
            TMP_Text txt = btnObj.GetComponentInChildren<TMP_Text>();
            string zoneName = level.zoneData != null ? level.zoneData.zoneName : "Unknown Zone";
            txt.text = $"{level.levelID}: {zoneName}";

            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnLevelSelected(level));
        }
    }

    /// <summary>
    /// Handles level button click.
    /// </summary>
    private void OnLevelSelected(LevelData level)
    {
        StartCoroutine(LoadLevelScene(level));
    }

    /// <summary>
    /// Loads a level scene and passes data to GameManager.
    /// </summary>
    private System.Collections.IEnumerator LoadLevelScene(LevelData level)
    {
        GameManager.Instance.StartLevel(level);
        yield return null;
    }

    // ===== Menu Controls =====
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        shopPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void ShowLevelSelect()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        shopPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void ShowShop()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        shopPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        shopPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
