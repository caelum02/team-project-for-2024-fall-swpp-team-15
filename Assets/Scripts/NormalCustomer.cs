using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yogaewonsil.Common;

/// <summary>
/// 일반손님 NPC의 동작을 다룸
/// </summary>
public class NormalCustomer : CustomerBase
{
    protected override FoodData MakeOrder()
    {
        FoodData orderedDish = customerManager.GetRandomDish();
        menuText.text = orderedDish.food.ToString();
        DisplayIcon(orderedDish.icon);

        customerManager.HandleOrder(this, orderedDish); // customerManager에게 order 보고
        return orderedDish;
    }

    protected override void DeleteOrder()
    {
        customerManager.orderManager.RemoveOrder(this, requiredDish);  // 주문 삭제
    }

    protected override void PayMoneyAndReputation()
    {
        if (isSuccessTreatment)
        {   
            customerManager.UpdateGameStats(requiredDish.price, requiredDish.level * 25);
        }
    }
}