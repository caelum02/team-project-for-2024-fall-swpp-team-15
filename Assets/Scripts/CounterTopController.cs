using UnityEngine;
using System.Collections;
using Yogaewonsil.Common;

public class CountertopController : CookingStationBase
{
    [Header("Additional UI References")]
    private Canvas extraCanvas;
    private Transform gaugeBarPanel;
    private GaugeBar gaugeBar;

    private bool isMiniGameActive = false;

    protected override void Start()
    {
        base.Start();

        // Countertop 고유 설정
        stationName = "Countertop";
        cookingMethod = CookMethod.손질;

        gaugeBarPanel = visualMenu.transform.Find("GaugeBarPanel");
        if (gaugeBarPanel == null)
        {
            Debug.LogError($"GaugeBarPanel is not found in {visualMenu.name}");
            return;
        }

        gaugeBar = gaugeBarPanel.GetComponentInChildren<GaugeBar>();
        if (gaugeBar == null)
        {
            Debug.LogError("GaugeBar component not found in GaugeBarPanel!");
            return;
        }

        gaugeBarPanel.gameObject.SetActive(false);
    }

    protected override void StartCook()
    {
        base.StartCook();

        // Player 비활성화
        PlayerController.Instance.SetMovementEnabled(false);
    }

    protected override void Cook()
    {
        // UI 상태 업데이트
        gaugeBarPanel.gameObject.SetActive(true);
        iconPanel.gameObject.SetActive(false);

        // 미니게임 초기화
        isMiniGameActive = true;

        // 게이지 바 채우기 모드 시작
        gaugeBar.StartGame(GaugeBar.GameMode.FillGaugeByClicking, 5f);
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

        // UI 상태 복원
        gaugeBarPanel.gameObject.SetActive(false);
        iconPanel.gameObject.SetActive(true);

        // 이벤트 해제
        gaugeBar.OnGameComplete -= OnGameComplete;

        // CompleteCook() 호출하여 요리 결과 처리
        CompleteCook(isSuccess);
    }
}
