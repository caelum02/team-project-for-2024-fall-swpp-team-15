using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refridgerator : MonoBehaviour
{
    
    [SerializeField] private FoodObjectSO foodObjectSO;

    private FoodObject foodObject;

    public void Interact(PlayerController player){
        if(!player.HasFoodObject()){
            Transform foodObjectTransform = Instantiate(foodObjectSO.prefab);
            foodObjectTransform.GetComponent<FoodObject>().SetFoodObjectParent(player);
        }
        else {
            Debug.Log("Cannot give food object. Player already holding object");
        }
    }
}
