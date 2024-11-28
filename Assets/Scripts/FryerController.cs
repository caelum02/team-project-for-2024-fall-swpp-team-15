using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yogaewonsil.Common;

/// <summary>
/// FryerController는 튀김 요리를 처리하는 클래스입니다.
/// HeatBasedStationBase를 상속받아 열 기반 조리기구의 동작을 구현합니다.
/// </summary>
public class FryerController : HeatBasedStationBase
{
    /// <summary>
    /// 초기화 메서드로, 튀김기(Fryer)의 고유 설정을 정의합니다.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // Fryer 고유 설정
        stationName = "Fryer";
        cookingMethod = CookMethod.튀기기;
    }
}
