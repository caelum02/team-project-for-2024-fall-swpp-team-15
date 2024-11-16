using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 15.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = new Vector2(0,0);
        if(Input.GetKey(KeyCode.W)){
            inputVector.y += 1;
        }
        if(Input.GetKey(KeyCode.S)){
            inputVector.y -= 1;
        }
        if(Input.GetKey(KeyCode.A)){
            inputVector.x -= 1;
        }
        if(Input.GetKey(KeyCode.D)){
            inputVector.x += 1;
        }

        inputVector = inputVector.normalized;

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float interactDistance = 2f;
        if(Physics.Raycast(transform.position, moveDir, out RaycastHit raycastHit, interactDistance)){
            Debug.Log(raycastHit.transform);
        }
    }

    private void HandleMovement(){
        Vector2 inputVector = new Vector2(0,0);
        if(Input.GetKey(KeyCode.W)){
            inputVector.y += 1;
        }
        if(Input.GetKey(KeyCode.S)){
            inputVector.y -= 1;
        }
        if(Input.GetKey(KeyCode.A)){
            inputVector.x -= 1;
        }
        if(Input.GetKey(KeyCode.D)){
            inputVector.x += 1;
        }

        inputVector = inputVector.normalized;

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}
