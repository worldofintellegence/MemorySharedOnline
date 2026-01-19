using UnityEngine;
using SurvivalEngine;
using Suriyun.MCS;

public class AnalogStickBridge : MonoBehaviour
{
    [Header("References")]
    public PlayerControls controls;
    public AnalogStick analogStick;

    [Header("Config")]
    public bool isMovementStick = true; // false = camera stick

    [Header("Read Only")]
    public Vector2 currentDirection;

    void Awake()
    {
        if (controls == null)
            controls = PlayerControls.Get();
    }

    // 🔹 Inspector-compatible (On Drag / On Begin Drag)
    public void OnStickDrag_Int(int fingerId)
    {
        if (analogStick == null || controls == null)
            return;

        Vector3 dir = analogStick.direction;
        currentDirection = new Vector2(dir.x, dir.y);

        if (isMovementStick)
            controls.mobile_move = currentDirection;
        else
            controls.mobile_freelook = currentDirection;
    }

    // 🔹 Inspector-compatible (On End Drag / Pointer Up)
    public void OnStickRelease_Int(int fingerId)
    {
        currentDirection = Vector2.zero;

        if (controls == null)
            return;

        if (isMovementStick)
            controls.mobile_move = Vector2.zero;
        else
            controls.mobile_freelook = Vector2.zero;
    }
}
