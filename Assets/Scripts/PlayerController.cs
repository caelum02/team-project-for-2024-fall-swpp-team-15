using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IFoodObjectParent
{
    public static PlayerController Instance { get; set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public ClearCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 15.0f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask utensilsLayerMask;
    private Transform objectHoldPoint;
    [SerializeField] Vector3 holdPointVector = new Vector3(0.31f, 0.19f, 0.67f);

    private FoodObject foodObject;

    private Vector3 lastInteractDir;
    private ClearCounter selectedCounter;

    private void Awake() {
        if (Instance != null){
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;

        if (objectHoldPoint == null)
        {
            GameObject holdPointObject = new GameObject("ObjectHoldPoint");
            holdPointObject.transform.SetParent(this.transform); // Set as a child of the player
            holdPointObject.transform.localPosition = holdPointVector;
            holdPointObject.transform.localRotation = Quaternion.identity;
            objectHoldPoint = holdPointObject.transform;
        }

        GameObject gameInputObject = GameObject.Find("GameInput");
        gameInput = gameInputObject.GetComponent<GameInput>();
    }

    void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        
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
}
