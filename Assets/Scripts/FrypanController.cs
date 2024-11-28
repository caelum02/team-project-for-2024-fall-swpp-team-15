using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yogaewonsil.Common;

/// <summary>
/// FrypanController는 프라이팬을 사용한 굽기 요리를 처리하는 클래스입니다.
/// HeatBasedStationBase를 상속받아 열 기반 조리기구의 동작을 구현합니다.
/// </summary>
public class FrypanController : HeatBasedStationBase
{
    /// <summary>
    /// 초기화 메서드로, 프라이팬(Frypan)의 고유 설정을 정의합니다.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // Frypan 고유 설정
        stationName = "Frypan";
        cookingMethod = CookMethod.굽기;
    }
}
