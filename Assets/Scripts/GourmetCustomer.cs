using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yogaewonsil.Common;

/// <summary>
/// 음식평론가 NPC의 동작을 다룸
/// </summary>
public class GourmetCustomer : NormalCustomer
{
    protected override void SetPatience()
    {
        patience = 70f; // 음식평론가는 일반손님보다 인내심이 더 적음.
    }
    
    protected override void PayMoneyAndReputation()
    {
        if (isSuccessTreatment)
        {   
            customerManager.UpdateGameStats(2 * requiredDish.price, requiredDish.level * 50); // 일반손님 2배의 보상
        }
    }
}