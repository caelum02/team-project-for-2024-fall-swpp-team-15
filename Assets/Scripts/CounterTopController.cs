using UnityEngine;
using System.Collections; 
using Yogaewonsil.Common;

public class CounterTopController : CookingStationBase
{
    [Header("Additional UI References")]
    private Transform gaugeBarPanel;
    private GaugeBar gaugeBar;
    private bool miniGameCompleted = false;
    private bool isMiniGameActive = false;

    protected override void Start()
    {
        base.Start();

        // Countertop 고유 설정
        stationName = "Countertop";
        cookingMethod = CookMethod.손질;

        gaugeBarPanel = iconCanvas.transform.Find("GaugeBarPanel");
        if (gaugeBarPanel == null)
        {
            Debug.LogError($"GaugeBarPanel is not found in {iconCanvas.name}");
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

    protected override void Cook()
    {
        // UI 상태 업데이트
        gaugeBarPanel.gameObject.SetActive(true);
        iconPanel.gameObject.SetActive(false);

        // 미니게임 초기화
        miniGameCompleted = false;
        isMiniGameActive = true;

        // 게이지 바 채우기 시작
        gaugeBar.StartFilling();
        gaugeBar.OnGaugeComplete += OnGaugeComplete;

        // 5초 타이머 시작
        StartCoroutine(MiniGameTimer());
    }

    private IEnumerator MiniGameTimer()
    {
        yield return new WaitForSeconds(5f);

        // 5초가 지나면 미니게임 종료 처리
        if (!miniGameCompleted)
        {
            Debug.Log("Mini-game failed! Time ran out.");
            EndMiniGame(false);
        }
    }

    private void OnGaugeComplete()
    {
        if (!isMiniGameActive) return; // 이미 종료된 경우 무시

        Debug.Log("Gauge full! Mini-game complete!");
        miniGameCompleted = true;

        EndMiniGame(true);
    }

    private void EndMiniGame(bool isSuccess)
    {
        isMiniGameActive = false;

        // 게이지 바 및 UI 상태 복원
        gaugeBarPanel.gameObject.SetActive(false);
        iconPanel.gameObject.SetActive(true);

        // 이벤트 해제
        gaugeBar.OnGaugeComplete -= OnGaugeComplete;

        // CompleteCook() 호출하여 요리 결과 처리
        CompleteCook(isSuccess);
    }
}
