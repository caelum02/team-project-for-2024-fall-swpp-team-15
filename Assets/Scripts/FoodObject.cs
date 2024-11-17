using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodObject : MonoBehaviour
{
    [SerializeField] private FoodObjectSO foodObjectSO;

    private IFoodObjectParent foodObjectParent;

    public FoodObjectSO GetFoodObjectSO(){
        return foodObjectSO;
    }

    public void SetFoodObjectParent(IFoodObjectParent foodObjectParent){
        if(this.foodObjectParent != null){ // 전 foodObjectParent에 음식이 있으면 Clear해준다
            this.foodObjectParent.ClearFoodObject();
        }
        this.foodObjectParent = foodObjectParent;

        if(foodObjectParent.HasFoodObject()){
            Debug.LogError("IFoodObjectParent already has a FoodObject!");
        }

        foodObjectParent.SetFoodObject(this); //새로운 clearCounter로 foodObject를 옮긴다

        transform.parent = foodObjectParent.GetFoodObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public IFoodObjectParent GetFoodObjectParent() {
        return foodObjectParent;
    }


}
