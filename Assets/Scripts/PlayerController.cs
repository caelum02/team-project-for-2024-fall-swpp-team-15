using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Yogaewonsil.Common;

/// <summary>
/// 플레이어 캐릭터를 제어하는 클래스입니다. 
/// 이동, 상호작용, 아이템 조작 등을 담당합니다.
/// </summary>
public class PlayerController : MonoBehaviour, IFoodObjectParent
{   
    public static PlayerController Instance { get; private set; } // Singleton Instance
    [SerializeField] private float moveSpeed = 15.0f; // 플레이어 이동 속도
    [SerializeField] private GameInput gameInput; // 사용자 입력 처리 클래스
    [SerializeField] private LayerMask utensilsLayerMask; // 상호작용 가능한 오브젝트 레이어 마스크
    private Transform objectHoldPoint; // Legacy (추후에 사용 예정)
    [SerializeField] Vector3 holdPointVector = new Vector3(0.31f, 0.19f, 0.67f); // Legacy (추후에 사용 예정)

    private FoodObject foodObject; // Legacy (사용하지 않을 예정)
    private Vector3 lastInteractDir; // Legacy (사용하지 않을 예정)
    public Food? heldFood = Food.쌀; // 플레이어가 들고 있는 음식 (Nullable)
    private bool isMovementEnabled = true; // 플레이어 이동 가능 여부

    /// <summary>
    /// Singleton Instance를 설정합니다.
    /// </summary>
    private void Awake()
    {
        // Singleton Instance 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 기존 인스턴스가 있으면 현재 객체 삭제
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 파괴되지 않도록 설정
    }

    /// 이 밑에 함수는 준희님이 이전에 구현했던 함수들
    void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void OnDestroy()
    {
        gameInput.OnInteractAction -= GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if(moveDir != Vector3.zero){
            lastInteractDir = moveDir;
        }
        Debug.Log("Player Interact");

        float interactDistance = 2f;
        Debug.DrawRay(transform.position, lastInteractDir * interactDistance, Color.red, 0.1f);

        if(Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, utensilsLayerMask)){
            if( raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)) {
                // Has ClearCounter
                clearCounter.Interact(this);
            }
            if( raycastHit.transform.TryGetComponent(out Refridgerator refridgerator)) {
                // Has Refridgerator
                refridgerator.Interact(this);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {   
        if (!isMovementEnabled) return;
        HandleMovement();
        //HandleInteractions();
        //Debug.Log(foodObject);
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
        if (HasHeldFood())
        {
            Debug.Log($"Dropped: {heldFood}");
            heldFood = null;
        }
        else
        {
            Debug.LogWarning("No food to drop.");
        }
    }

    /// <summary>
    /// 음식 아이템을 들기
    /// </summary>
    public bool PickUpFood(Food? food)
    {   
        if (heldFood != null) {
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

    // 이번에 새로 구현한 기능과 상충되어서 주석 처리
    /*private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if(moveDir != Vector3.zero){
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        Debug.DrawRay(transform.position, lastInteractDir * interactDistance, Color.red, 0.1f);

        if(Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, utensilsLayerMask)){
            if( raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)) {
                // Has ClearCounter
                if(clearCounter != selectedCounter){
                    selectedCounter = clearCounter;

                    OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
                        selectedCounter = selectedCounter
                    });
                }
                else {
                    selectedCounter = null;
                }
            }
            else {
                selectedCounter = null;
            }
        }
        Debug.Log(selectedCounter);
    }*/

    private void HandleMovement(){
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.5f;
        float playerHeight = 2.0f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if(!canMove){
            // Cannot move towards moveDir

            // Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x,0,0);
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if(canMove){
                // Can move only on the X
                moveDir = moveDirX;
            }
            else{
                // Cannot move only on the X

                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0,0,moveDir.z);
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if(canMove){
                    // Can move only on the Z
                    moveDir = moveDirZ;
                }
                else {
                    // Cannot move in any direction
                }
            }
        }
        
        if(canMove) {
            transform.position += moveDir * moveDistance;
        }


        float rotateSpeed = 10.0f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime*rotateSpeed);
    }

    // private void SetSelectedCounter(ClearCounter selectedCounter) {
    //     this.selectedCounter = selectedCounter;
    // }

    public Transform GetFoodObjectFollowTransform() {
        return objectHoldPoint;
    }

    public void SetFoodObject(FoodObject foodObject){
        this.foodObject = foodObject;
    }

    public FoodObject GetFoodObject() {
        return foodObject;
    }

    public void ClearFoodObject() {
        foodObject = null;
    }

    public bool HasFoodObject() {
        return foodObject != null;
    }
}
