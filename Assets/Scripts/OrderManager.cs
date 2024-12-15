using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 손님의 주문 데이터를 관리하는 클래스
/// </summary>
public class OrderManager : MonoBehaviour
{
    /// <summary>
    /// 현재 저장된 주문 목록
    /// </summary>
    public List<Order> orders = new List<Order>();

    /// <summary>
    /// 음식 데이터베이스
    /// </summary>
    public FoodDatabaseSO foodDatabase;

    /// <summary>
    /// 주문 가능한 음식 데이터 리스트
    /// </summary>
    public List<FoodData> eligibleFood;

    void Start()
    {
        // 초기화 로직이 필요한 경우 작성
    }

    void Update()
    {
        // 필요 시 추가 로직 작성
    }

    /// <summary>
    /// 새 주문을 저장
    /// </summary>
    /// <param name="order">저장할 주문 데이터</param>
    public void SaveOrder(Order order)
    {
        orders.Add(order);
        Debug.Log($"Order Saved: {order.customerName} ordered {order.dish.name}");
    }

    /// <summary>
    /// 저장된 모든 주문 반환
    /// </summary>
    /// <returns>현재 저장된 주문 목록</returns>
    public List<Order> GetAllOrders()
    {
        return orders;
    }

    /// <summary>
    /// 주문 가능한 음식 데이터 중 하나를 랜덤으로 반환
    /// </summary>
    /// <returns>랜덤으로 선택된 음식 데이터</returns>
    public FoodData GetRandomEligibleFood()
    {
        eligibleFood = foodDatabase.foodData.FindAll(food => food.tag == FoodTag.Dish && food.isBought);
        if (eligibleFood.Count > 0)
        {
            int randomIndex = Random.Range(0, eligibleFood.Count);
            return eligibleFood[randomIndex];
        }
        return null;
    }

    /// <summary>
    /// 특정 주문을 삭제
    /// </summary>
    /// <param name="dish">삭제할 주문의 음식 데이터</param>
    public void RemoveOrder(CustomerNPC customer, FoodData dish)
    {   
        int removedOrders = orders.RemoveAll(order => order.dish == dish && order.customerName == customer.name);
        Debug.Log($"{removedOrders} order(s) for {dish.name} by {customer.name} removed.");
    }

    /// <summary>
    /// 주문 목록 초기화
    /// </summary>
    public void ClearOrder()
    {
        orders.Clear();
        Debug.Log("All orders cleared.");
    }
}
