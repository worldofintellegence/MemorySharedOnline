using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsBar : MonoBehaviour
{
    [Header("Base UI References")]
    public Image bar;
    public TMP_Text statsValue;
    public float maxValue = 100f;

    public Button trainingButton; // Reference to the training button
    // Is method ko virtual rakha hai taake child class ise modify kar sake
    public virtual void UpdateStatsBar(float currentVal)
    {
        if (bar != null)
            bar.fillAmount = currentVal / maxValue;

        if (statsValue != null)
            statsValue.text = Mathf.RoundToInt(currentVal).ToString();
    }
}