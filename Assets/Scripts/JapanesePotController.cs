using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yogaewonsil.Common;

public class JapanesePotController : HeatBasedStationBase
{
    protected override void Start()
    {
        base.Start();

        // Fryer 고유 설정
        stationName = "Japanese Pot";
        cookingMethod = CookMethod.밥짓기;
    }
}
