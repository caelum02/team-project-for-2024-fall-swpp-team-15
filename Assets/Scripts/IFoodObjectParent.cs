using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFoodObjectParent
{
    public Transform GetFoodObjectFollowTransform();

    public void SetFoodObject(FoodObject foodObject);

    public FoodObject GetFoodObject();

    public void ClearFoodObject();

    public bool HasFoodObject();
}
