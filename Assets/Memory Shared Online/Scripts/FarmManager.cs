using UnityEngine;

public class FarmManager : MonoBehaviour
{
    public GameObject healthBarCanvas;

    void Start()
    {
        if (Application.isBatchMode)
            return; // Dedicated Server, UI skip

        Invoke(nameof(SpawnHealthBar), 2.5f);
    }

    void SpawnHealthBar()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null || healthBarCanvas == null)
            return;

        Instantiate(healthBarCanvas, player.transform);
    }
}
