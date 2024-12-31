using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yogaewonsil.Common;

/// <summary>
/// HeatBasedStationBase는 열을 이용한 조리기구의 기본 동작을 정의하는 추상 클래스입니다.
/// *열을 이용하는 조리기구는 모두 일정 시간이 지난 후에 너무 늦기 전에 "끄기"버튼을 눌러야 하는 미니게임을 진행합니다.
/// </summary>
public abstract class HeatBasedStationBase : CookingStationBase
{
    [Header("Additional UI References")]
    private Transform stopButtonPanel; // 끄기 버튼 UI가 포함된 패널
    private Button stopButton; // 끄기 버튼
    private Button cancelButton; // 취소 버튼

    [Header("Additional Audio")]
    [SerializeField] protected AudioSource alertAudioSource; // 경고 사운드를 재생할 AudioSource
    [SerializeField] private AudioClip alertSound; // 요리모드 변환 시 재생할 사운드
    private Coroutine stopButtonCoroutine;

    // [Header("Effects")]
    // [SerializeField] private GameObject smokeParticlePrefab; // SmokeParticle 프리팹
    // [SerializeField] private GameObject failParticlePrefab; // failParticle 프리팹
    // private GameObject smokeInstance = null; // 파티클 오브젝트

    /// <summary>
    /// 초기화 메서드로, Stop 버튼 및 UI를 설정합니다.
    /// </summary>
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

        // StopButton 설정
        stopButton = stopButtonPanel.GetComponentInChildren<Button>();
        if (stopButton == null)
        {
            Debug.LogError($"StopButton not found in StopButtonPanel!");
            return;
        }

        stopButton.onClick.AddListener(OnStopButtonPressed); // Stop 버튼 클릭 이벤트 연결

        // CancelButton 설정
        cancelButton = stopButtonPanel.Find("CancelButton")?.GetComponent<Button>();
        if (cancelButton == null)
        {
            Debug.LogError($"CancelButton not found in StopButtonPanel!");
            return;
        }

        cancelButton.onClick.AddListener(OnCancelButtonPressed); // Cancel 버튼 클릭 이벤트 연결

        stopButtonPanel.gameObject.SetActive(false); // 초기에는 비활성화
    }

    /// <summary>
    /// 요리 시작 시 호출되는 메서드로, 카운트다운 모드의 미니게임을 시작합니다.
    /// </summary>
    protected override void Cook()
    {
        isMiniGameActive = true;

        // 게이지바 시작 (20초 카운트다운 모드)
        gaugeBar.StartGame(GaugeBar.GameMode.CountdownGauge, 20f);

        // Animator가 Normal 상태라면 Click 트리거를 활성화
        if (PlayerController.Instance.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal"))
        {
            // 상태가 일치하면 트리거 활성화
            PlayerController.Instance.playerAnimator.SetTrigger("clickTrig");
        }


        gaugeBar.OnGameComplete += OnGaugeComplete;

        // StopButtonPanel 활성화 및 취소 버튼 표시
        stopButtonPanel.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);

        // 10초 후에 StopButton 활성화
        stopButtonCoroutine = StartCoroutine(EnableStopButtonAfterDelay(10f));
    }

    /// <summary>
    /// 지정된 딜레이 후 Stop 버튼을 활성화합니다.
    /// </summary>
    /// <param name="delay">딜레이 시간 (초)</param>
    private IEnumerator EnableStopButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isMiniGameActive)
        {   
            cancelButton.gameObject.SetActive(false); // 취소 버튼 비활성화
            stopButton.gameObject.SetActive(true); // Stop 버튼 활성화
            // 틀리는 사운드 재생
            if (alertAudioSource != null && alertSound != null)
            {
                alertAudioSource.clip = alertSound;
                alertAudioSource.loop = false; // 필요 시 루프 설정
                alertAudioSource.Play(); // 사운드 재생
            }
        }
    }

    /// <summary>
    /// 취소 버튼 클릭 시 호출되는 메서드입니다.
    /// 요리 과정을 중지하고 초기 상태로 되돌립니다.
    /// </summary>
    private void OnCancelButtonPressed()
    {
        if (!isMiniGameActive)
        {
            Debug.LogWarning("Not cooking currently."); // 현재 요리가 진행 중이 아님
            return;
        }

        // Animator가 Normal 상태라면 Click 트리거를 활성화
        if (PlayerController.Instance.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal"))
        {
            // 상태가 일치하면 트리거 활성화
            PlayerController.Instance.playerAnimator.SetTrigger("clickTrig");
        }

        isMiniGameActive = false; // 미니게임 비활성화

        // 모든 코루틴 정지
        StopAllCoroutines();

        // 게이지 바 초기화
        gaugeBar.OnGameComplete -= OnGaugeComplete; // 이벤트 해제
        gaugeBar.ResetGauge();

        // StopButtonPanel 및 버튼 비활성화
        stopButtonPanel.gameObject.SetActive(false);

        // 오디오 중지
        if (audioSource != null)
        {   
            audioSource.Stop();
        }

        // 파티클 제거
        if (particleInstance != null)
        {
            Destroy(particleInstance);
            particleInstance = null;
        }

        // UI 상태 복원
        gaugeBarPanel.gameObject.SetActive(false);
        iconPanel.gameObject.SetActive(true);

        // 애니메이션 복원
        animator.SetBool("isCooking", false);

        // 상태 복원
        isCooking = false;

        Debug.Log("Cooking canceled. Returned to initial state.");

        // 버튼 상태 업데이트
        UpdateAllButtons();
        UpdateIngredientIcons();
    }

    /// <summary>
    /// 게이지바의 카운트다운 완료 시 호출되는 메서드입니다.
    /// </summary>
    /// <param name="isSuccess">미니게임 성공 여부</param>
    private void OnGaugeComplete(bool isSuccess)
    {
        if (!isMiniGameActive) return;

        isMiniGameActive = false; // 미니게임 비활성화
        gaugeBar.OnGameComplete -= OnGaugeComplete; // 이벤트 해제

        // Stop 버튼 패널 비활성화 
        stopButtonPanel.gameObject.SetActive(false);
        // Destroy(smokeInstance);

        if (!isSuccess)
        {
            Debug.Log("Cooking failed: Time expired!"); // 실패 메시지
            CompleteCook(false); // 실패 처리
        }
    }


    /// <summary>
    /// Stop 버튼 클릭 시 호출되는 메서드입니다.
    /// </summary>
    private void OnStopButtonPressed()
    {
        if (!isMiniGameActive)
        {
            Debug.LogWarning("Not cooking currently."); // 현재 요리가 진행 중이 아님
            return;
        }
        
        // Animator가 Normal 상태라면 Click 트리거를 활성화
        if (PlayerController.Instance.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal"))
        {
            // 상태가 일치하면 트리거 활성화
            PlayerController.Instance.playerAnimator.SetTrigger("clickTrig");
        }

        isMiniGameActive = false; // 미니게임 비활성화
        stopButtonPanel.gameObject.SetActive(false); // Stop 버튼 패널 비활성화
        // Destroy(smokeInstance);

        Debug.Log("Cooking successful: Stopped in time!"); 
        CompleteCook(true); // 성공 처리
    }

    public override void ResetCookingState()
    {   
        if (stopButtonCoroutine != null) 
        {
            StopCoroutine(stopButtonCoroutine);
            stopButtonCoroutine = null;
        }

        base.ResetCookingState();

        alertAudioSource.Stop();
        stopButtonPanel.gameObject.SetActive(false); 

    }
}
