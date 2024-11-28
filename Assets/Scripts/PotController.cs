using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yogaewonsil.Common;

public class PotController : HeatBasedStationBase
{
    protected override void Start()
    {
        base.Start();

        // Fryer 고유 설정
        stationName = "Pot";
        cookingMethod = CookMethod.끓이기;
    }
}
