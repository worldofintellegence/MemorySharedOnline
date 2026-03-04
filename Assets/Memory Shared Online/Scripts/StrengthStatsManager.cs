using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class StrengthStatsManager : StatsBar
{
    [Header("Data Storage Keys")]
    private string statKey = "Player_STR_Value";
    private string lastTrainKey = "Last_Train_Date";
    private string historyKey = "Pushup_History"; // Stored as comma separated string

    [Header("Requirement Settings")]
    private bool isInPrivateLocation = true;
    void OnEnable()
    {
     MiniGameTransition.OnMiniGameEnded += CheckForNewSessionData;   
    }
    void OnDisable()
    {
     MiniGameTransition.OnMiniGameEnded -= CheckForNewSessionData;   
    }
    void Start()
    {
        LoadStats();
        CheckForNewSessionData();
        trainingButton.interactable = CanStartTraining(); // Enable/Disable button based on conditions
        trainingButton.onClick.AddListener(() => {
     
                // Load Training Scene (Assuming you have a scene named "TrainingScene")
            //    SceneLoader.Instance.LoadScene("PushupTrainingScene");
        //    SurvivalEngine. TheGame.Get().TransitionToScene("MainGameScene");
           LoadPushupScene();
            
        });
    }
    private void LoadPushupScene() {
        // 1. Save progress (Good practice)
        SurvivalEngine.WorldData.Get().Save(SurvivalEngine.WorldData.Get().filename);

        // 2. Additive load karein taaki Netcode aur Main Game delete na ho
        // UnityEngine.SceneManagement.SceneManager.LoadScene("PushupTrainingScene", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        MiniGameTransition.Instance.LoadMiniGame("PushupTrainingScene");
       
    }

    // 1. Check if we just came back from the Training Scene
    private void CheckForNewSessionData()
    {
        // Training scene se wapas aate waqt hum "LatestSessionPushups" save karenge
        if (PlayerPrefs.HasKey("LatestSessionPushups"))
        {
            int newCount = PlayerPrefs.GetInt("LatestSessionPushups");
            PlayerPrefs.DeleteKey("LatestSessionPushups"); // Use it once and delete
            ProcessNewPushupData(newCount);
        }
    }

    // 2. 24-Hour and Private Location Check
    public bool CanStartTraining()
    {
        if (!isInPrivateLocation) return false;

        if (PlayerPrefs.HasKey(lastTrainKey))
        {
            DateTime lastDate = DateTime.Parse(PlayerPrefs.GetString(lastTrainKey));
            if (DateTime.Now.Date <= lastDate.Date) 
                return false; // Already trained today
        }
        return true;
    }

    // 3. The Core Logic: Average and % Increase
    public void ProcessNewPushupData(int dailyCount)
    {
        // Load history string (e.g., "10,8,12,16,6,0,0")
        int[] history = GetHistoryArray();
        float oldAverage = (float)history.Average();

        // Shift array (Rolling week)
        for (int i = 0; i < 6; i++) history[i] = history[i + 1];
        history[6] = dailyCount;

        float newAverage = (float)history.Average();

        // Load Current Stat
        float currentStr = PlayerPrefs.GetFloat(statKey, 5.0f);

        // Calculate Adjustment
        if (newAverage == 0 && oldAverage > 0)
        {
            currentStr = Mathf.Max(0, currentStr - 2); // Penalty
        }
        else if (newAverage > oldAverage && oldAverage > 0)
        {
            float percentageIncrease = (newAverage - oldAverage) / oldAverage;
            currentStr += (currentStr * percentageIncrease); // Growth
        }

        // Save everything
        SaveHistoryArray(history);
        PlayerPrefs.SetFloat(statKey, currentStr);
        PlayerPrefs.SetString(lastTrainKey, DateTime.Now.ToString());
        PlayerPrefs.Save();

        UpdateStatsBar(currentStr);
    }

    // Helper Methods for Data Management
    private int[] GetHistoryArray() {
        string data = PlayerPrefs.GetString(historyKey, "0,0,0,0,0,0,0");
        return Array.ConvertAll(data.Split(','), int.Parse);
    }

    private void SaveHistoryArray(int[] history) {
        string data = string.Join(",", history);
        PlayerPrefs.SetString(historyKey, data);
    }

    private void LoadStats() {
        float currentStr = PlayerPrefs.GetFloat(statKey, 5.0f);
        UpdateStatsBar(currentStr);
    }
}