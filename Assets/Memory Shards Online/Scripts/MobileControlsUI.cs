using UnityEngine;
using SurvivalEngine;

/// <summary>
/// Hides the mobile controls while a blocking panel (shop, storage, crafting...) is open,
/// so they don't cover the panel or steal its touches.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class MobileControlsUI : MonoBehaviour
{
    public float hidden_alpha = 0f;
    public float fade_speed = 8f;

    private CanvasGroup group;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        TheUI ui = TheUI.Get();
        bool visible = ui == null || !ui.IsBlockingPanelOpened();

        group.interactable = visible;
        group.blocksRaycasts = visible;
        group.alpha = Mathf.MoveTowards(group.alpha, visible ? 1f : hidden_alpha, fade_speed * Time.unscaledDeltaTime);
    }
}
