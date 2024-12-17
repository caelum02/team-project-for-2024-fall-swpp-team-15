using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;
    public float minY = 5f; // Minimum zoom (height)
    public float maxY = 40f; // Maximum zoom (height)

    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float minX = -50f; // Minimum X limit
    public float maxX = 50f;  // Maximum X limit
    public float minZ = -50f; // Minimum Z limit
    public float maxZ = 50f;  // Maximum Z limit

    [Header("Mouse Drag Settings")]
    public float dragSpeed = 2f;        // 마우스 드래그 속도

    private Camera cam;
    private float currentZoom;          // 현재 줌 상태
    private Vector3 lastMousePosition;  // 마지막 마우스 위치
    private bool isDragging = false;    // 드래그 상태 확인

    [Header("Main Restaurant")]
    private PlacementSystem placementSystem;
    [SerializeField] private GameObject mainRestaurant;

    void Start()
    {
        placementSystem = GameObject.Find("PlacementSystem").GetComponent<PlacementSystem>(); // placementSystem 찾기
    }

    void Update()
    {
        HandleZoom();
        ShowMainRestaurant();
        HandleMovement();
        HandleMouseDrag();
    }

    void HandleZoom()
    {   
        if (!IsActiveScrollbarInScene())
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
    }

    private void ShowMainRestaurant()
    {
        if (transform.position.y >= maxY - 0.01 && placementSystem != null)
        {
            mainRestaurant.SetActive(true);
            placementSystem.SetActiveAllWalls(false);
            PauseGame();
        }
        else
        {
            mainRestaurant.SetActive(false);
            placementSystem.SetActiveAllWalls(true);
            ResumeGame();
        }
    }

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(2)) // 마우스 휠 클릭 시작
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(2)) // 마우스 휠 클릭 종료
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 deltaMousePosition = Input.mousePosition - lastMousePosition;
            Vector3 moveDirection = new Vector3(-deltaMousePosition.y, 0, deltaMousePosition.x) * dragSpeed * Time.deltaTime;

            // 이동 적용
            transform.Translate(moveDirection, Space.World);
            lastMousePosition = Input.mousePosition;
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

    public void PauseGame()
    {   
        Time.timeScale = 0; // 게임 일시정지
    }

    public void ResumeGame()
    {   
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1; // 게임 재개
        }
    }
}