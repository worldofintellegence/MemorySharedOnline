using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SurvivalEngine
{
    /// <summary>
    /// Mobile button input handler for Attack, Jump, Action, Craft, Pause and camera rotation
    /// Attach this to your UI buttons and assign the PlayerControls reference
    /// </summary>
    public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public enum ButtonType { Attack, Jump, Action, Craft, Pause, CameraLeft, CameraRight }

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
            if (controls == null)
                controls = PlayerControls.Get();
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
                case ButtonType.Craft:
                    controls.mobile_craft = true;
                    break;
                case ButtonType.Pause:
                    controls.mobile_pause = true;
                    break;
                case ButtonType.CameraLeft:
                    controls.mobile_rotate_cam = -1f;
                    break;
                case ButtonType.CameraRight:
                    controls.mobile_rotate_cam = 1f;
                    break;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Release();
        }

        private void OnDisable()
        {
            Release();
        }

        private void Release()
        {
            if (controls == null) return;

            switch (buttonType)
            {
                case ButtonType.Attack:
                    controls.mobile_attack = false;
                    break;
                case ButtonType.CameraLeft:
                case ButtonType.CameraRight:
                    controls.mobile_rotate_cam = 0f;
                    break;
            }
        }
    }
}
