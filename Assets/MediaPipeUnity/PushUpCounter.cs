using UnityEngine;
using System.Collections.Generic;
using Mediapipe.Tasks.Components.Containers;
using TMPro;
using UnityEngine.SceneManagement;

public class PushUpCounter : MonoBehaviour {
    [Header("UI References")]
    public TextMeshProUGUI counterText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI timerText;
    public GameObject startButton; // Button to start tracking

    [Header("Settings")]
    public float downAngleThreshold = 85f;
    public float upAngleThreshold = 165f;
    public float requiredHoldTime = 0.5f;
    public float sessionDuration = 30f;

    private int count = 0;
    private enum GameState { Idle, CheckingPosition, StartingCountdown, Training, Finished }
    private GameState currentGameState = GameState.Idle;
    
    private enum PushUpState { Up, GoingDown, Holding, GoingUp }
    private PushUpState currentPushState = PushUpState.Up;

    private float gameTimer;
    private float startCountdown = 3f;
    private float currentHoldTimer = 0f;
    private bool isHoldingNow = false;
    private string currentStatus = "Click Start";
    private bool needsUIUpdate = false;

    // --- BUTTON TRIGGER ---
    public void OnClickStartTracking() {
        if (currentGameState == GameState.Idle) {
            currentGameState = GameState.CheckingPosition;
            currentStatus = "Get into PUSH-UP position!";
            if(startButton) startButton.SetActive(false);
            needsUIUpdate = true;
        }
    }

    public void ProcessLandmarks(IReadOnlyList<NormalizedLandmark> landmarks) {
        if (landmarks == null || landmarks.Count < 16) return;

        float angle = CalculateAngle(landmarks[11], landmarks[13], landmarks[15]);

        // State Machine Flow
        switch (currentGameState) {
            case GameState.CheckingPosition:
                // Check karein ke player ready position mein hai (Up position)
                if (angle >= upAngleThreshold - 10f) {
                    currentGameState = GameState.StartingCountdown;
                    startCountdown = 3.5f; // Ready for countdown
                }
                break;

            case GameState.Training:
                ProcessPushupLogic(angle);
                break;
        }
    }
public void OnClickBackToGame()
    {
        // 1. Save progress (Good practice)

        // 2. Unload Training Scene and go back to Main Game
        MiniGameTransition.Instance.UnloadMiniGame();

    } 
       private void ProcessPushupLogic(float angle) {
        switch (currentPushState) {
            case PushUpState.Up:
                if (angle < upAngleThreshold - 15f) {
                    currentPushState = PushUpState.GoingDown;
                    currentStatus = "Lowering...";
                    needsUIUpdate = true;
                }
                break;

            case PushUpState.GoingDown:
                if (angle <= downAngleThreshold) {
                    currentPushState = PushUpState.Holding;
                    isHoldingNow = true;
                    currentHoldTimer = 0f;
                    currentStatus = "HOLD IT!";
                    needsUIUpdate = true;
                }
                break;

            case PushUpState.Holding:
                if (angle > downAngleThreshold + 15f) {
                    currentPushState = PushUpState.GoingDown;
                    isHoldingNow = false;
                } else if (currentHoldTimer >= requiredHoldTime) {
                    currentPushState = PushUpState.GoingUp;
                    isHoldingNow = false;
                    currentStatus = "Now PUSH UP!";
                    needsUIUpdate = true;
                }
                break;

            case PushUpState.GoingUp:
                if (angle >= upAngleThreshold) {
                    count++;
                    currentPushState = PushUpState.Up;
                    currentStatus = "Good Job!";
                    needsUIUpdate = true;
                }
                break;
        }
    }

    void Update() {
        if (currentGameState == GameState.StartingCountdown) {
            startCountdown -= Time.deltaTime;
            statusText.text = "Starting in: " + Mathf.CeilToInt(startCountdown);
            if (startCountdown <= 0.5f) {
                currentGameState = GameState.Training;
                gameTimer = sessionDuration;
                currentStatus = "GO!";
                needsUIUpdate = true;
            }
        }

        if (currentGameState == GameState.Training) {
            gameTimer -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.CeilToInt(gameTimer) + "s";
            
            if (isHoldingNow) currentHoldTimer += Time.deltaTime;

            if (gameTimer <= 0) {
                FinishSession();
            }
        }

        if (needsUIUpdate) {
            counterText.text = "Pushups: " + count;
            statusText.text = currentStatus;
            needsUIUpdate = false;
        }
    }

    private void FinishSession() {
        currentGameState = GameState.Finished;
        // Save count to PlayerPrefs for StatsBar to pick up
        PlayerPrefs.SetInt("LatestSessionPushups", count);
        PlayerPrefs.Save();
        
        statusText.text = "FINISHED!";
        Invoke("LoadMainScene", 2f); // 2 second baad scene change
    }

    private void LoadMainScene() {
         OnClickBackToGame();
    }

    float CalculateAngle(NormalizedLandmark p1, NormalizedLandmark p2, NormalizedLandmark p3) {
        Vector2 v1 = new Vector2(p1.x - p2.x, p1.y - p2.y);
        Vector2 v2 = new Vector2(p3.x - p2.x, p3.y - p2.y);
        return Vector2.Angle(v1, v2);
    }
}