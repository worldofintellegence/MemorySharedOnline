using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorToCamera : MonoBehaviour
{
    public enum AnchorSide { Left, Right }
    public AnchorSide anchorSide;
    public float xOffset = 0f;
    public float yOffset = 0f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found. Please make sure there's a camera tagged as MainCamera in the scene.");
        }
    }

    void Update()
    {
        if (mainCamera != null)
        {
            Vector3 newPos = transform.position;
            Vector3 cameraPos = mainCamera.transform.position;
            float cameraWidth = mainCamera.orthographicSize * mainCamera.aspect;

            if (anchorSide == AnchorSide.Left)
            {
                newPos.x = cameraPos.x - cameraWidth + xOffset;
            }
            else if (anchorSide == AnchorSide.Right)
            {
                newPos.x = cameraPos.x + cameraWidth - xOffset;
            }

            newPos.y = cameraPos.y + yOffset;
            transform.position = newPos;
        }
    }
}