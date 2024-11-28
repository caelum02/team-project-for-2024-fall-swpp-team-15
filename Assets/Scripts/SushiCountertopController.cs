using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yogaewonsil.Common;

public class SushiCountertopController : CookingStationBase
{
    protected override void Start()
    {
        base.Start();

        // SushiCountertop 고유 설정
        stationName = "SushiCountertop";
        cookingMethod = CookMethod.초밥제작;
    }

    protected override void StartCook()
    {
        base.StartCook();

        // Player 비활성화
        PlayerController.Instance.SetMovementEnabled(false);
    }
    
    protected override void Cook()
    {
        // 미니게임 초기화
        isMiniGameActive = true;

        gaugeBar.StartGame(GaugeBar.GameMode.MarkerMatching, 5f);
        gaugeBar.OnGameComplete += OnGameComplete;
    }
    
    private void OnGameComplete(bool isSuccess)
    {
        if (!isMiniGameActive) return;

        Debug.Log(isSuccess
            ? "Mini-game succeeded! Cooking successful."
            : "Mini-game failed! Cooking failed.");

        EndMiniGame(isSuccess);
    }

    private void EndMiniGame(bool isSuccess)
    {
        isMiniGameActive = false;

        // 이벤트 해제
        gaugeBar.OnGameComplete -= OnGameComplete;

        // CompleteCook() 호출하여 요리 결과 처리
        CompleteCook(isSuccess);
    }
}
