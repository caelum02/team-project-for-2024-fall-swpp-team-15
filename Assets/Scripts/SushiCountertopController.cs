using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yogaewonsil.Common;

public class SushiCountertopController : CookingStationBase
{
    [Header("Additional UI References")]
    private Transform gaugeBarPanel;
    private GaugeBar gaugeBar;
    private Transform stopButtonPanel;

    private int currentGameIndex = 0; // 현재 실행 중인 미니게임 인덱스
    private int totalGames = 3; // 총 미니게임 수
    private bool allGamesSuccessful = true; // 모든 미니게임 성공 여부
    private bool isMiniGameActive = false;

    protected override void Start()
    {
        base.Start();

        // SushiCountertop 고유 설정
        stationName = "SushiCountertop";
        cookingMethod = CookMethod.초밥제작;

        // GaugeBarPanel 설정
        gaugeBarPanel = visualMenu.transform.Find("GaugeBarPanel");
        if (gaugeBarPanel == null)
        {
            Debug.LogError($"GaugeBarPanel is not found in {visualMenu.name}");
            return;
        }

        gaugeBar = gaugeBarPanel.GetComponentInChildren<GaugeBar>();
        if (gaugeBar == null)
        {
            Debug.LogError($"GaugeBar component not found in GaugeBarPanel!");
            return;
        }

        // StopButtonPanel 설정
        stopButtonPanel = visualMenu.transform.Find("StopButtonPanel");
        if (stopButtonPanel == null)
        {
            Debug.LogError($"StopButtonPanel is not found in {visualMenu.name}");
            return;
        }

        // 초기 상태
        gaugeBarPanel.gameObject.SetActive(false);
        stopButtonPanel.gameObject.SetActive(false);
    }

    protected override void Cook()
    {
        // UI 활성화
        gaugeBarPanel.gameObject.SetActive(true);
        iconPanel.gameObject.SetActive(false);

        // 미니게임 초기화
        currentGameIndex = 0;
        allGamesSuccessful = true;

        StartNextMiniGame();
    }

    private void StartNextMiniGame()
    {
        if (currentGameIndex >= totalGames)
        {
            // 모든 미니게임 종료 처리
            EndMiniGames(allGamesSuccessful);
            return;
        }

        isMiniGameActive = true;
        Debug.Log($"Starting mini-game {currentGameIndex + 1} of {totalGames}");

        gaugeBar.StartGame(GaugeBar.GameMode.MarkerMatching, 5f); // MarkerMatching 게임 시작
        gaugeBar.OnGameComplete += OnMiniGameComplete;
    }

    private void OnMiniGameComplete(bool isSuccess)
    {
        if (!isMiniGameActive) return;

        isMiniGameActive = false;
        gaugeBar.OnGameComplete -= OnMiniGameComplete;

        if (!isSuccess)
        {
            allGamesSuccessful = false; // 하나라도 실패하면 전체 실패로 설정
        }

        currentGameIndex++;
        StartNextMiniGame(); // 다음 미니게임 시작
    }

    private void EndMiniGames(bool isSuccess)
    {
        Debug.Log(isSuccess ? "All mini-games completed successfully!" : "Mini-games failed!");

        gaugeBarPanel.gameObject.SetActive(false);
        iconPanel.gameObject.SetActive(true);

        CompleteCook(isSuccess); // 성공 또는 실패 처리
    }
}
