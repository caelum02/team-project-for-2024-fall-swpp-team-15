using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yogaewonsil.Common;

public class FryerController : HeatBasedStationBase
{
    protected override void Start()
    {
        base.Start();

        // Fryer 고유 설정
        stationName = "Fryer";
        cookingMethod = CookMethod.튀기기;
    }
}
