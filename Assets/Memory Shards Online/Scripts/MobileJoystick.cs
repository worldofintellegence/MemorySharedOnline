// using UnityEngine;
// using UnityEngine.EventSystems;

// public class MobileJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
// {
//     public RectTransform handle;
//     public float radius = 60f;

//     Vector2 input;

//     public void OnDrag(PointerEventData eventData)
//     {
//         Vector2 pos;
//         RectTransformUtility.ScreenPointToLocalPointInRectangle(
//             transform as RectTransform,
//             eventData.position,
//             eventData.pressEventCamera,
//             out pos);

//         pos = Vector2.ClampMagnitude(pos, radius);
//         handle.anchoredPosition = pos;
//         input = pos / radius;

//         PlayerControls.Get().mobile_move = input;
//     }

//     public void OnPointerDown(PointerEventData eventData) { }

//     public void OnPointerUp(PointerEventData eventData)
//     {
//         handle.anchoredPosition = Vector2.zero;
//         input = Vector2.zero;
//         PlayerControls.Get().mobile_move = Vector2.zero;
//     }
// }
