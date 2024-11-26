using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yogaewonsil.Common;

public class FrypanController : CookingStationBase
{
    [Header("Additional UI References")]
    private Transform gaugeBarPanel;
    private GaugeBar gaugeBar;
    private Transform stopButtonPanel;
    private Button stopButton;

    private bool isStopped = false;

    protected override void Start()
    {
        base.Start();

        // Fryer 고유 설정
        stationName = "Frypan";
        cookingMethod = CookMethod.굽기;

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
        isStopped = false;

        // UI 활성화
        gaugeBarPanel.gameObject.SetActive(true);
        iconPanel.gameObject.SetActive(false);

        // 게이지바 시작
        gaugeBar.StartCountdown(20f); // 10초 카운트다운
        gaugeBar.OnGaugeComplete += OnGaugeComplete;

        // 5초 후에 StopButton 활성화
        StartCoroutine(EnableStopButtonAfterDelay(10f));
    }

    private void OnGaugeComplete()
    {
        gaugeBar.OnGaugeComplete -= OnGaugeComplete;

        if (!isStopped)
        {
            Debug.Log("Cooking failed: Time expired!");
            CompleteCook(false); // 실패 처리
        }
    }

    private IEnumerator EnableStopButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isCooking)
        {
            stopButtonPanel.gameObject.SetActive(true);
        }
    }

    private void OnStopButtonPressed()
    {
        if (!isCooking)
        {
            Debug.LogWarning("Not cooking currently.");
            return;
        }
        stopButtonPanel.gameObject.SetActive(false);

        isStopped = true;
        Debug.Log("Cooking successful: Stopped in time!");

        gaugeBarPanel.gameObject.SetActive(false);
        iconPanel.gameObject.SetActive(true);
        CompleteCook(true); // 성공 처리
    }
}
