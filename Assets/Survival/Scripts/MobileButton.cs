using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SurvivalEngine
{
    /// <summary>
    /// Mobile button input handler for Attack, Jump, Action
    /// Attach this to your UI buttons and assign the PlayerControls reference
    /// </summary>
    public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public enum ButtonType { Attack, Jump, Action }

        [Header("Button Settings")]
        public ButtonType buttonType;

        private PlayerControls controls;

        private void Start()
        {
            controls = PlayerControls.Get();
            if (controls == null)
                Debug.LogWarning("PlayerControls instance not found!");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (controls == null) return;

            switch (buttonType)
            {
                case ButtonType.Attack:
                    controls.mobile_attack = true;
                    break;
                case ButtonType.Jump:
                    controls.mobile_jump = true;
                    break;
                case ButtonType.Action:
                    controls.mobile_action = true;
                    break;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (controls == null) return;

            switch (buttonType)
            {
                case ButtonType.Attack:
                    controls.mobile_attack = false;
                    break;
                case ButtonType.Jump:
                    controls.mobile_jump = false;
                    break;
                case ButtonType.Action:
                    controls.mobile_action = false;
                    break;
            }
        }
    }
}
