using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yogaewonsil.Common;

public class JapanesePotController : CookingStationBase
{
    [Header("Additional UI References")]
    private Transform gaugeBarPanel;
    private GaugeBar gaugeBar;
    private Transform stopButtonPanel;
    private Button stopButton;

    private bool isMiniGameActive = false;

    [Header ("Quantity Tracking")]
    private int FoodQuantity = 0;

    protected override void Start()
    {
        base.Start();

        // Fryer 고유 설정
        stationName = "Japanese Pot";
        cookingMethod = CookMethod.밥짓기;

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

        stopButton = stopButtonPanel.GetComponentInChildren<Button>();
        if (stopButton == null)
        {
            Debug.LogError($"StopButton not found in StopButtonPanel!");
            return;
        }

        stopButton.onClick.AddListener(OnStopButtonPressed);

        // 초기 상태
        gaugeBarPanel.gameObject.SetActive(false);
        stopButtonPanel.gameObject.SetActive(false);
    }

    protected override void Cook()
    {
        isMiniGameActive = true;

        // UI 활성화
        gaugeBarPanel.gameObject.SetActive(true);
        iconPanel.gameObject.SetActive(false);

        // 게이지바 시작 (20초 카운트다운 모드)
        gaugeBar.StartGame(GaugeBar.GameMode.CountdownGauge, 20f);
        gaugeBar.OnGameComplete += OnGaugeComplete;

        // 10초 후에 StopButton 활성화
        StartCoroutine(EnableStopButtonAfterDelay(10f));
    }

    private IEnumerator EnableStopButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isMiniGameActive)
        {
            stopButtonPanel.gameObject.SetActive(true);
        }
    }

    private void OnGaugeComplete(bool isSuccess)
    {
        if (!isMiniGameActive) return;

        isMiniGameActive = false;
        gaugeBar.OnGameComplete -= OnGaugeComplete;

        stopButtonPanel.gameObject.SetActive(false);

        if (!isSuccess)
        {
            Debug.Log("Cooking failed: Time expired!");
            gaugeBarPanel.gameObject.SetActive(false);
            iconPanel.gameObject.SetActive(true);
            CompleteCook(false); // 실패 처리
        }
    }

    private void OnStopButtonPressed()
    {
        if (!isMiniGameActive)
        {
            Debug.LogWarning("Not cooking currently.");
            return;
        }

        isMiniGameActive = false;
        stopButtonPanel.gameObject.SetActive(false);

        Debug.Log("Cooking successful: Stopped in time!");
        gaugeBarPanel.gameObject.SetActive(false);
        iconPanel.gameObject.SetActive(true);
        CompleteCook(true); // 성공 처리
    }
}
