using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Yogaewonsil.Common;

public class PlayerController : MonoBehaviour, IFoodObjectParent
{   
    public static PlayerController Instance { get; private set; } // Singleton Instance
    [SerializeField] private float moveSpeed = 15.0f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask utensilsLayerMask;
    private Transform objectHoldPoint;
    [SerializeField] Vector3 holdPointVector = new Vector3(0.31f, 0.19f, 0.67f);

    private FoodObject foodObject;
    private Vector3 lastInteractDir;
    public Food heldFood = Food.쌀;

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

    
    // Start is called before the first frame update
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
        HandleMovement();
        //HandleInteractions();
        //Debug.Log(foodObject);
    }

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

    private void SetSelectedCounter(ClearCounter selectedCounter) {
        this.selectedCounter = selectedCounter;
    }

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

    public bool HasHeldFood()
    {
        return heldFood != Food.None;
    }
    public void DropFood()
    {
        if (HasHeldFood())
        {
            Debug.Log($"Dropped: {heldFood}");
            heldFood = Food.None;
        }
        else
        {
            Debug.LogWarning("No food to drop.");
        }
    }

    public bool PickUpFood(Food food)
    {   
        if (heldFood != Food.None) {
            Debug.LogWarning("You are already holding Food");
            return false;
        }
        if (food == Food.None)
        {
            Debug.LogWarning("Cannot pick up Food.None!");
            return false;
        }

        heldFood = food; // 플레이어가 들고 있는 음식 업데이트
        Debug.Log($"Player picked up: {food}");
        return true;
    }

    public Food GetHeldFood()
    {
        return heldFood;
    }
}
