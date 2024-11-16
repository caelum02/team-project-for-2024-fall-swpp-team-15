using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour, IFoodObjectParent
{
    [SerializeField] private FoodObjectSO foodObjectSO;
    [SerializeField] private Transform counterTopPoint;

    private FoodObject foodObject;


    public void Interact(PlayerController player){
        if(foodObject == null){
            Transform foodObjectTransform = Instantiate(foodObjectSO.prefab, counterTopPoint); //위에 음식이 없으면 음식을 생성하고 더 생성 못하게 만든다
            foodObjectTransform.GetComponent<FoodObject>().SetFoodObjectParent(this);
        } else {
            foodObject.SetFoodObjectParent(player);
        }
    }

    public Transform GetFoodObjectFollowTransform() {
        return counterTopPoint;
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
