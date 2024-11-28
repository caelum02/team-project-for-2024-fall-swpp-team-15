using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yogaewonsil.Common;

public class FrypanController : HeatBasedStationBase
{
    protected override void Start()
    {
        base.Start();

        // Fryer 고유 설정
        stationName = "Frypan";
        cookingMethod = CookMethod.굽기;
    }
}
