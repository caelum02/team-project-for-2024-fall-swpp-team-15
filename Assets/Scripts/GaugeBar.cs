using UnityEngine;
using UnityEngine.UI;

public class GaugeBar : MonoBehaviour
{
    public Image gaugeBar; // 게이지를 채울 이미지
    public float fillSpeed = 0.1f; // 자동 감소 속도
    public float maxGauge = 1f; // 게이지 최대 값
    private float currentGauge = 0f; // 현재 게이지 값
    private bool isFilling = false; // 게이지가 채워지는 중인지 여부
    private bool isCountdownActive = false; // 카운트다운 동작 여부
    private float countdownDuration = 5f; // 카운트다운 지속 시간
    private float countdownTimeRemaining = 0f; // 남은 카운트다운 시간

    public Color normalColor = Color.green; // 정상 상태 색상
    public Color warningColor = Color.red; // 경고 상태 색상
    private Image gaugeImage; // GaugeImage 컴포넌트 참조

    public delegate void GaugeCompleteHandler();
    public event GaugeCompleteHandler OnGaugeComplete; // 게이지가 비었을 때 이벤트

    void Awake()
    {
        // GaugeImage 가져오기
        if (gaugeBar != null)
        {
            gaugeImage = gaugeBar.GetComponent<Image>();
            if (gaugeImage == null)
            {
                Debug.LogError("GaugeImage component not found!");
            }
        }
    }

    void Update()
    {
        if (isCountdownActive)
        {
            HandleCountdown();
        }
        else if (isFilling)
        {
            HandleFilling();
        }
    }

    private void HandleFilling()
    {
        // 1. 마우스 클릭 시 게이지 증가
        if (Input.GetMouseButtonDown(0)) // 왼쪽 마우스 클릭
        {
            currentGauge += 0.2f; // 클릭 시 게이지 증가량
            currentGauge = Mathf.Clamp(currentGauge, 0, maxGauge); // 최대값 제한
        }

        // 2. 게이지 자동 감소
        currentGauge -= fillSpeed * Time.deltaTime; // 시간에 따라 감소
        currentGauge = Mathf.Clamp(currentGauge, 0, maxGauge); // 최소값 제한

        // 3. 게이지 UI 업데이트
        gaugeBar.fillAmount = currentGauge / maxGauge;

        // 4. 게이지 색상 업데이트
        UpdateGaugeColor();

        // 5. 게이지가 가득 찼는지 확인
        if (currentGauge >= maxGauge - 0.05f)
        {
            isFilling = false; // 게이지 채우기 중단
            OnGaugeComplete?.Invoke(); // 이벤트 호출
        }
    }

    private void HandleCountdown()
    {
        // 1. 카운트다운 동안 게이지 줄이기
        countdownTimeRemaining -= Time.deltaTime; // 남은 시간 감소
        currentGauge -= (maxGauge / countdownDuration) * Time.deltaTime; // 게이지 줄어들기
        currentGauge = Mathf.Clamp(currentGauge, 0, maxGauge); // 최소값 제한

        // 2. 게이지 UI 업데이트
        gaugeBar.fillAmount = currentGauge / maxGauge;

        // 3. 게이지 색상 업데이트
        UpdateGaugeColor();

        // 4. 카운트다운 종료 확인
        if (countdownTimeRemaining <= 0 || currentGauge <= 0)
        {
            isCountdownActive = false; // 카운트다운 종료
            OnGaugeComplete?.Invoke(); // 이벤트 호출
        }
    }

    private void UpdateGaugeColor()
    {
        // 남은 게이지가 30% 이하일 경우 경고 색상으로 변경
        if (currentGauge / maxGauge <= 0.3f)
        {
            gaugeImage.color = warningColor;
        }
        else
        {
            gaugeImage.color = normalColor;
        }
    }

    public void StartFilling()
    {
        currentGauge = 0f; // 게이지 초기화
        isFilling = true; // 채우기 시작
    }

    public void StopFilling()
    {
        isFilling = false; // 채우기 중단
    }

    public void StartCountdown(float duration)
    {
        countdownDuration = duration; // 카운트다운 지속 시간 설정
        countdownTimeRemaining = countdownDuration; // 초기화
        currentGauge = maxGauge; // 게이지를 최대값으로 초기화
        isCountdownActive = true; // 카운트다운 활성화
        gaugeImage.color = normalColor; // 시작 시 정상 상태 색상으로 초기화
    }

    public void StopCountdown()
    {
        isCountdownActive = false; // 카운트다운 중단
    }
}
