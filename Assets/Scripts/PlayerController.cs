using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 15.0f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask utensilsLayerMask;
    [SerializeField] private Transform objectHoldPoint;

    private GameObject heldObject;

    private Vector3 lastInteractDir;

    
    // Start is called before the first frame update
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

        float interactDistance = 2f;
        Debug.DrawRay(transform.position, lastInteractDir * interactDistance, Color.red, 0.1f);

        if(Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, utensilsLayerMask)){
            if( raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)) {
                // Has ClearCounter
                clearCounter.Interact();
            }
            if( raycastHit.transform.TryGetComponent(out Refridgerator refridgerator)) {
                // Has Refridgerator
                refridgerator.Interact();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    private void HandleInteractions()
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
                //clearCounter.Interact();
            }
        }
    }

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

    public Transform GetObjectFollowTransform(){
        return objectHoldPoint;
    }
}
