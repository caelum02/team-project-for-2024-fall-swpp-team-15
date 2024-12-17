using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;
    public float minY = 5f; // Minimum zoom (height)
    public float maxY = 20f; // Maximum zoom (height)

    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float minX = -50f; // Minimum X limit
    public float maxX = 50f;  // Maximum X limit
    public float minZ = -50f; // Minimum Z limit
    public float maxZ = 50f;  // Maximum Z limit

    void Update()
    {
        HandleZoom();
        HandleMovement();
    }

    void HandleZoom()
    {   
        if (!IsActiveScrollbarInScene())
        {
            // Get scroll input
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            // Calculate zoom movement in the forward direction
            Vector3 moveDirection = transform.forward * scrollInput * zoomSpeed;

            // Apply movement to the position
            transform.position += moveDirection;

            // Clamp the Y position to enforce zoom limits
            transform.position = new Vector3(
                transform.position.x,
                Mathf.Clamp(transform.position.y, minY, maxY), // Restrict Y
                transform.position.z
            );
        }
    }

    void HandleMovement()
    {
        float moveX = 0f;
        float moveZ = 0f;

        // Input for horizontal and vertical movement
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveZ += moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveZ -= moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveX += moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveX -= moveSpeed * Time.deltaTime;
        }

        // Apply movement
        Vector3 newPosition = transform.position + new Vector3(moveX, 0, moveZ);

        // Clamp X and Z positions
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        // Update camera position
        transform.position = newPosition;
    }

    // GameScene에 활성화된 Scrollbar가 있는지 확인하는 함수
    public bool IsActiveScrollbarInScene()
    {
        Scrollbar[] scrollbars = FindObjectsOfType<Scrollbar>();
        foreach (Scrollbar scrollbar in scrollbars)
        {
            if (scrollbar.gameObject.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }
}