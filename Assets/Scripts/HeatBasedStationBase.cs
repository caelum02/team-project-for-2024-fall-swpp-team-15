using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yogaewonsil.Common;

public abstract class HeatBasedStationBase : CookingStationBase
{
    [Header("Additional UI References")]
    private Transform stopButtonPanel;
    private Button stopButton;

    protected override void Start()
    {
        base.Start();

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

        stopButtonPanel.gameObject.SetActive(false);
    }

    protected override void Cook()
    {
        isMiniGameActive = true;

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
        CompleteCook(true); // 성공 처리
    }
}
