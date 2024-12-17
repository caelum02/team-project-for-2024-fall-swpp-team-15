using UnityEngine;
using Yogaewonsil.Common;

public static class FoodDataUtility
{
    /// <summary>
    /// 특정 Food 타입에 해당하는 FoodData를 반환합니다.
    /// </summary>
    /// <param name="foodDatabase">FoodDatabaseSO 인스턴스</param>
    /// <param name="foodType">찾을 음식</param>
    /// <returns>FoodData 객체 또는 null</returns>
    public static FoodData FindFoodDataByType(FoodDatabaseSO foodDatabase, Food foodType)
    {
        if (foodDatabase == null || foodDatabase.foodData == null)
        {
            Debug.LogWarning("FoodDatabase is null or empty.");
            return null;
        }

        foreach (FoodData foodData in foodDatabase.foodData)
        {
            if (foodData.food == foodType)
            {
                return foodData;
            }
        }
        Debug.LogWarning($"Food type {foodType} not found in database.");
        return null;
    }
}
