using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yogaewonsil.Common;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; set; }

    [SerializeField] private float moveSpeed = 15.0f;
    [SerializeField] private GameInput gameInput;
    public Food? heldFood = Food.쌀; // 플레이어가 들고 있는 음식 (Nullable)
    private bool isMovementEnabled = true; // 플레이어 이동 가능 여부

    [Header("Food Database")]
    [SerializeField] private FoodDatabaseSO foodDatabase; // 음식 데이터베이스

    [Header("Prefab Spawn Settings")]
    [SerializeField] private Transform holdPoint; // 객체를 붙일 위치

    private GameObject currentHeldObject; // 플레이어가 현재 들고 있는 프리팹 객체

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 기존 인스턴스가 있으면 현재 객체 삭제
            return;
        }
        Instance = this;

        GameObject gameInputObject = GameObject.Find("GameInput");
        gameInput = gameInputObject.GetComponent<GameInput>();
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 파괴되지 않도록 설정
        InstantiateFoodPrefab();
    }

    /// <summary>
    /// 플레이어가 음식을 들고 있는지 확인
    /// </summary>
    public bool HasHeldFood()
    {
        return heldFood != null;
    }

    /// <summary>
    /// 들고 있는 음식 아이템을 내려놓음
    /// </summary>
    public void DropFood()
    {
        if (HasHeldFood() && currentHeldObject != null)
        {
            Debug.Log($"Dropped: {heldFood}");
            Destroy(currentHeldObject); // 들고 있는 객체 삭제
            heldFood = null;
            currentHeldObject = null;
        }
        else
        {
            Debug.LogWarning("No food to drop or no object held.");
        }
    }

    /// <summary>
    /// 음식 아이템을 들기
    /// </summary>
    public bool PickUpFood(Food? food)
    {
        if (heldFood != null)
        {
            Debug.LogWarning("You are already holding Food");
            return false;
        }
        if (food == null)
        {
            Debug.LogWarning("Cannot pick up null!");
            return false;
        }

        heldFood = food; // 플레이어가 들고 있는 음식 업데이트
        Debug.Log($"Player picked up: {food}");
        InstantiateFoodPrefab(); // 프리팹 생성 및 부모 설정
        return true;
    }

    /// <summary>
    /// 들고 있는 음식 반환
    /// </summary>
    public Food? GetHeldFood()
    {
        return heldFood;
    }

    /// <summary>
    /// 플레이어 이동 가능 여부 설정
    /// </summary>
    public void SetMovementEnabled(bool isEnabled)
    {
        isMovementEnabled = isEnabled;
    }

    // Update is called once per frame
    void Update()
    {   
        if (!isMovementEnabled) return;
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.5f;
        float playerHeight = 2.0f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            // Cannot move towards moveDir

            // Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                // Can move only on the X
                moveDir = moveDirX;
            }
            else
            {
                // Cannot move only on the X

                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    // Can move only on the Z
                    moveDir = moveDirZ;
                }
                else
                {
                    // Cannot move in any direction
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        float rotateSpeed = 10.0f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    /// <summary>
    /// heldFood에 해당하는 프리팹을 생성하고 플레이어의 자식으로 설정합니다.
    /// </summary>
    private void InstantiateFoodPrefab()
    {
        if (!HasHeldFood())
        {
            Debug.LogWarning("No held food to instantiate.");
            return;
        }

        FoodData foodData = FindFoodDataByType((Food)heldFood);
        if (foodData == null || foodData.prefab == null)
        {
            Debug.LogWarning($"No prefab found for held food: {heldFood}");
            return;
        }

        // 프리팹 생성
        currentHeldObject = Instantiate(foodData.prefab, holdPoint.position, Quaternion.identity, holdPoint);

        Debug.Log($"Spawned prefab for {heldFood} at {holdPoint.position}");
    }

    /// <summary>
    /// 특정 Food 타입에 해당하는 FoodData를 검색합니다.
    /// </summary>
    private FoodData FindFoodDataByType(Food foodType)
    {
        foreach (FoodData foodData in foodDatabase.foodData)
        {
            if (foodData.food == foodType)
            {
                return foodData;
            }
        }
        Debug.LogWarning($"Food type {foodType} not found in database.");
        return null;
    }
}
