using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public List<Order> orders = new List<Order>();
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
        Debug.Log($"Order Saved: {order.customerName} ordered {order.dishName}");
    }

    // 모든 주문 보여주기 
    public List<Order> GetAllOrders()
    {
        return orders;
    }
}
