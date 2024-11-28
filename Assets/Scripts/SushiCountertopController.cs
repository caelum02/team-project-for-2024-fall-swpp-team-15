using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yogaewonsil.Common;

/// <summary>
/// SushiCountertopController는 초밥 조리대의 동작을 관리하는 클래스입니다.
/// 초밥 제작을 위한 미니게임을 실행하고, 요리 완료 여부를 처리합니다.
/// </summary>
public class SushiCountertopController : CookingStationBase
{
    /// <summary>
    /// 초기화 메서드로, 조리대의 기본 설정을 초기화합니다.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // SushiCountertop 고유 설정
        stationName = "SushiCountertop"; // 조리대 이름
        cookingMethod = CookMethod.초밥제작; // 초밥 제작 요리 방법
    }

    /// <summary>
    /// 요리를 시작할 때 호출되는 메서드로, 플레이어를 비활성화하고 기본 상태를 설정합니다.
    /// </summary>
    protected override void StartCook()
    {
        base.StartCook();

        // Player 비활성화
        PlayerController.Instance.SetMovementEnabled(false);
    }

    /// <summary>
    /// 요리 로직을 처리하며, 미니게임을 실행합니다.
    /// </summary>
    protected override void Cook()
    {
        // 미니게임 초기화
        isMiniGameActive = true;

        // 초밥제작 미니게임: 마커 맞추기 미니게임
        gaugeBar.StartGame(GaugeBar.GameMode.MarkerMatching, 5f);
        gaugeBar.OnGameComplete += OnGameComplete; // 미니게임 완료 이벤트 연결
    }
    
    /// <summary>
    /// 미니게임 완료 시 호출되는 메서드입니다.
    /// </summary>
    /// <param name="isSuccess">미니게임 성공 여부</param>
    private void OnGameComplete(bool isSuccess)
    {
        if (!isMiniGameActive) return;

        Debug.Log(isSuccess
            ? "Mini-game succeeded! Cooking successful."
            : "Mini-game failed! Cooking failed.");

        EndMiniGame(isSuccess);
    }

    /// <summary>
    /// 미니게임 종료를 처리하며, 요리 결과를 처리합니다.
    /// </summary>
    /// <param name="isSuccess">미니게임 성공 여부</param>
    private void EndMiniGame(bool isSuccess)
    {
        isMiniGameActive = false;

        // 미니게임 이벤트 해제
        gaugeBar.OnGameComplete -= OnGameComplete;

        // CompleteCook() 호출하여 요리 결과 처리
        CompleteCook(isSuccess);
    }
}
