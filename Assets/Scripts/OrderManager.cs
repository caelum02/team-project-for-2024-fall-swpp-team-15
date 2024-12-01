using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public List<Order> orders = new List<Order>();
    public FoodDatabaseSO foodDatabase;
    List<FoodData> eligibleFood;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 주문 저장하기 
    public void SaveOrder(Order order)
    {
        orders.Add(order);
        Debug.Log($"Order Saved: {order.customerName} ordered {order.dish.name}");
    }

    // 모든 주문 보여주기 
    public List<Order> GetAllOrders()
    {
        return orders;
    }

    public FoodData GetRandomEligibleFood()
    {
        eligibleFood = foodDatabase.foodData.FindAll(food => food.tag == FoodTag.Dish && food.isUnlocked);
        if (eligibleFood.Count > 0)
        {
            // Return a random food item
            int randomIndex = Random.Range(0, eligibleFood.Count);
            return eligibleFood[randomIndex];
        }
        return null;
    }

    public void ClearOrder()
    {
        orders.Clear();
    }

}
