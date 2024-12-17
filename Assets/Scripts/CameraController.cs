using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // Get scroll input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f)
        {
            // Calculate zoom direction and movement
            Vector3 moveDirection = transform.forward * scrollInput * zoomSpeed;

            // Create a new target position
            Vector3 newPosition = transform.position + moveDirection;

            // Clamp the Y position to enforce zoom limits
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            // Restrict movement along X and Z if at zoom boundaries
            if (newPosition.y <= minY || newPosition.y >= maxY)
            {
                newPosition.x = transform.position.x; // Restrict X
                newPosition.z = transform.position.z; // Restrict Z
            }

            // Apply the new position
            transform.position = newPosition;
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
}