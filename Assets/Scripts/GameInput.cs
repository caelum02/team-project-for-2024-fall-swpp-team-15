using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public event Action OnInteractAction;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            OnInteractAction?.Invoke();
    }

    public Vector2 GetMovementVectorNormalized(){
        Vector2 inputVector = new Vector2(0,0);
        if(Input.GetKey(KeyCode.W)){
            inputVector.x += 1;
        }
        if(Input.GetKey(KeyCode.S)){
            inputVector.x -= 1;
        }
        if(Input.GetKey(KeyCode.A)){
            inputVector.y += 1;
        }
        if(Input.GetKey(KeyCode.D)){
            inputVector.y -= 1;
        }

        inputVector = inputVector.normalized;

        return inputVector;
    }
}
