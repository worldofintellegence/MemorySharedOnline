using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SurvivalEngine;

/// <summary>
/// Drag on an empty area of the screen to rotate the camera.
/// Put it on a transparent Image behind the mobile buttons.
/// </summary>
[RequireComponent(typeof(Image))]
public class MobileCameraDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public float degrees_per_screen = 220f; //Camera degrees rotated when dragging across the full screen width

    private bool dragging = false;
    private int finger_id = -99;
    private float drag_x = 0f;

    void Update()
    {
        PlayerControls controls = PlayerControls.Get();
        TheCamera cam = TheCamera.Get();
        if (!dragging || controls == null || cam == null || Mathf.Approximately(cam.rotate_speed, 0f))
            return;

        //Convert this frame's drag into the rotate value TheCamera expects (rotate_cam * rotate_speed = degrees per second)
        float degrees = drag_x / Screen.width * degrees_per_screen;
        controls.mobile_rotate_cam = -degrees / (cam.rotate_speed * Mathf.Max(Time.deltaTime, 0.001f));
        drag_x = 0f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (dragging)
            return;
        dragging = true;
        finger_id = eventData.pointerId;
        drag_x = 0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId == finger_id)
            drag_x += eventData.delta.x;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != finger_id)
            return;
        Release();
    }

    private void OnDisable()
    {
        Release();
    }

    private void Release()
    {
        dragging = false;
        finger_id = -99;
        drag_x = 0f;
        PlayerControls controls = PlayerControls.Get();
        if (controls != null)
            controls.mobile_rotate_cam = 0f;
    }
}
