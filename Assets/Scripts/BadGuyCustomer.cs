using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yogaewonsil.Common;

/// <summary>
/// 진상손님 NPC의 동작을 다룸
/// </summary>
public class BadGuyCustomer : CustomerBase
{   
    protected override void SetPatience()
    {
        patience = 200f; // 진상손님은 내쫓지 않으면 좀처럼 나가지 않음
    }

    protected override FoodData MakeOrder()
    {
        // 아무 주문도 하지 않고 인상만 씀
        DisplayIcon(FailIcon);
        orderButton.interactable = false;

        return FindFoodDataByType(Food.실패요리);
    }

    protected override void PayMoneyAndReputation()
    {
        if (isSuccessTreatment)
        {   
            customerManager.UpdateGameStats(10000, 20);
        }
        else 
        {
            customerManager.UpdateGameStats(0, -10);
        }
    }
}