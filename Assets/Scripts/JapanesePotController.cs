using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yogaewonsil.Common;

/// <summary>
/// JapanesePotController는 솥을 사용한 밥짓기 요리를 처리하는 클래스입니다.
/// HeatBasedStationBase를 상속받아 열 기반 조리기구의 동작을 구현합니다.
/// </summary>
public class JapanesePotController : HeatBasedStationBase
{
    /// <summary>
    /// 초기화 메서드로, 솥의 고유 설정을 정의합니다.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // JapanesePot 고유 설정
        stationName = "Japanese Pot";
        cookingMethod = CookMethod.밥짓기;
    }
}
