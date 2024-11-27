using UnityEngine;
using UnityEngine.UI;

public class GaugeBar : MonoBehaviour
{
    [Header("Gauge Settings")]
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
    private bool isGameActive = false; // 미니게임 활성화 여부

    public delegate void GaugeCompleteHandler();
    public event GaugeCompleteHandler OnGaugeComplete; // 게이지가 비었을 때 이벤트

    [Header("Marker Settings")]
    public RectTransform marker; // 동그라미 마커
    public RectTransform centralBand; // 중앙 띠
    public float markerSpeed = 50f; // 동그라미 이동 속도
    private bool markerMovingRight = true; // 마커가 오른쪽으로 움직이는 중인지 여부

    public delegate void MissionResultHandler(bool isSuccess);
    public event MissionResultHandler OnBarMissionResult; // 미션 성공/실패 이벤트

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

    public void CheckBarMissionSuccess()
    {
        if (!isGameActive) return;

        // 마커와 중앙 띠가 겹치는지 확인
        bool isSuccess = isRectOverlaps(marker, centralBand);

        // 미션 결과 전달
        OnBarMissionResult?.Invoke(isSuccess);

        // 게임 종료
        StopCountdown();
    }

    private void HandleMarkerMovement()
    {
        if (marker == null || centralBand == null) return;

        // 마커 이동
        float moveDirection = markerMovingRight ? 1f : -1f;
        marker.anchoredPosition += new Vector2(moveDirection * markerSpeed * Time.deltaTime, 0);

        // 마커가 게이지 바 끝을 넘어갈 경우 방향 반전
        if (marker.anchoredPosition.x >= gaugeBar.rectTransform.rect.width / 2)
        {
            markerMovingRight = false; // 왼쪽으로 이동
        }
        else if (marker.anchoredPosition.x <= -gaugeBar.rectTransform.rect.width / 2)
        {
            markerMovingRight = true; // 오른쪽으로 이동
        }
    }

    private bool isRectOverlaps(RectTransform rect1, RectTransform rect2)
    {
        Rect r1 = GetWorldRect(rect1);
        Rect r2 = GetWorldRect(rect2);

        return r1.Overlaps(r2);
    }

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
    }
}
