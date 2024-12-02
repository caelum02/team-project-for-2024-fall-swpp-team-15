using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 손님의 주문 데이터 관리
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 새 주문 저장
    /// </summary>
    /// <param name="order">저장할 <see cref="Order"/> 객체</param>
    public void SaveOrder(Order order)
    {
        orders.Add(order);
        Debug.Log($"Order Saved: {order.customerName} ordered {order.dish.name}");
    }

    /// <summary>
    /// 저장된 모든 주문 반환
    /// </summary>
    /// <returns>현재 저장된 <see cref="Order"/> 목록</returns>
    public List<Order> GetAllOrders()
    {
        return orders;
    }

    /// <summary>
    /// 주문 가능한 음식 랜덤으로 반환
    /// </summary>
    /// <returns>랜덤으로 선택된 <see cref="FoodData"/> 객체 또는 null</returns>
    public FoodData GetRandomEligibleFood()
    {
        eligibleFood = foodDatabase.foodData.FindAll(food => food.tag == FoodTag.Dish && food.isUnlocked); //우선 테스트 위해 isUnlocked로 설정. 이후 isBought로 바꿀 예정
        if (eligibleFood.Count > 0)
        {
            int randomIndex = Random.Range(0, eligibleFood.Count);
            return eligibleFood[randomIndex];
        }
        return null;
    }

    /// <summary>
    /// 주문 목록 초기화
    /// </summary>
    public void ClearOrder()
    {
        orders.Clear();
    }

}
