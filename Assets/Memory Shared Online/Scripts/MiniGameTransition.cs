using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System; // Events ke liye zaruri hai

public class MiniGameTransition : MonoBehaviour 
{
    public static MiniGameTransition Instance;

    // Events: Dusri scripts inhein subscribe kar sakti hain
    public static event Action OnMiniGameStarted;
    public static event Action OnMiniGameEnded;

    private List<GameObject> hiddenObjects = new List<GameObject>();
    private string currentMiniGameScene;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void LoadMiniGame(string sceneName) 
    {
        currentMiniGameScene = sceneName;
        GameObject[] allObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject obj in allObjects) {
            // "NetworkManager" aur "GameController" ko skip karein
            if (obj.tag != "NetworkManager" && obj.tag != "GameController" && obj.activeSelf) {
                obj.SetActive(false);
                hiddenObjects.Add(obj);
            }
        }

        // Additive load
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        
        // Event Fire karein: Ab jo bhi script refresh hona chahti hai, wo ho jayegi
        OnMiniGameStarted?.Invoke();
        Debug.Log("MiniGame Event Fired: Started");
    }

    public void UnloadMiniGame() 
    {
        if (string.IsNullOrEmpty(currentMiniGameScene)) return;

        SceneManager.UnloadSceneAsync(currentMiniGameScene);

        foreach (GameObject obj in hiddenObjects) {
            if (obj != null) obj.SetActive(true);
        }
        
        hiddenObjects.Clear();

        // Event Fire karein: Wapis main game mein aane par refresh ke liye
        OnMiniGameEnded?.Invoke();
        Debug.Log("MiniGame Event Fired: Ended");
    }
}