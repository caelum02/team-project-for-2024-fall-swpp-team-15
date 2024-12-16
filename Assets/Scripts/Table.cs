using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using Yogaewonsil.Common;

public class Table : TableBase
{   
    public bool isOccupied = false;

    protected override void UpdateAllButtons()
    {   
        if(PlayerController.Instance == null) return;
        Food? heldFood = PlayerController.Instance.GetHeldFood();
        putButton.interactable = heldFood != null && plateFood == null && isOccupied; // 손님이 있고 플레이어가 손에 음식이 있고 테이블에 음식이 없어야 버튼 활성화
    }


    public void Occupy()
    {
        isOccupied = true;
    }

    public void Vacate()
    {
        isOccupied = false;
    }    
}
