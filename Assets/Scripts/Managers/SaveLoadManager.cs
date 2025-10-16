using UnityEngine;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "SaveData.Sav");
    }

    // Saves the given PlayerProgress ScriptableObject as a JSON.
    public void SaveProgress(PlayerProgress progress)
    {
        if (progress == null)
        {
            Debug.LogWarning("SaveLoadManager.SaveProgress: progress is null, aborting save.");
            return;
        }

        string json = JsonUtility.ToJson(progress, true);

        try
        {
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"SaveLoadManager: Progress saved to {saveFilePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SaveLoadManager: Failed to save progress. Exception: {ex}");
        }
    }

    // Loads PlayerProgress and returns a new PlayerProgress ScriptableObject instance with data.
    // If there's no save, returns a new PlayerProgress ScriptableObject with default values.
    public PlayerProgress LoadProgress()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                PlayerProgress progress = ScriptableObject.CreateInstance<PlayerProgress>();
                JsonUtility.FromJsonOverwrite(json, progress);
                Debug.Log("SaveLoadManager: Progress loaded successfully.");
                return progress;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"SaveLoadManager: Failed to load or parse save file. Exception: {ex}");
                // Fall through to create default progress
            }
        }

        Debug.Log("SaveLoadManager: No valid save found. Creating default PlayerProgress.");
        PlayerProgress newProgress = ScriptableObject.CreateInstance<PlayerProgress>();
        InitializeDefaultProgress(newProgress);
        return newProgress;
    }

    // Creates default values for a new PlayerProgress instance.
    private void InitializeDefaultProgress(PlayerProgress progress)
    {
        // Leveling
        progress.playerLevel = 1;
        progress.playerExp = 0;
        progress.nextLevelExp = 100;
        progress.skillPoints = 0;

        // Stats
        progress.maxHP = 100f;
        progress.currentHP = progress.maxHP;
        progress.baseDamage = 10f;

        // Economy
        progress.money = 0;

        // Inventory counts
        progress.teaCount = 0;
        progress.milkCount = 0;
        progress.elixirCount = 0;

        // Cosmetics
        progress.cosmeticsOwned = new System.Collections.Generic.List<string>();
    }

    // Deletes the save file
    public void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("SaveLoadManager: Save file deleted.");
        }
        else
        {
            Debug.Log("SaveLoadManager: No save file to delete.");
        }
    }
}