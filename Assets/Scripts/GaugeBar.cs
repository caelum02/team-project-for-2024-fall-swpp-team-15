using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GaugeBar : MonoBehaviour
{
    public enum GameMode
    {
        FillGaugeByClicking,
        CountdownGauge,
        MarkerMatching
    }

    [Header("Gauge Settings")]
    private Image gaugeBar; // 게이지를 채울 이미지
    public float fillSpeed = 0.1f; // 자동 감소 속도
    public float maxGauge = 1f; // 게이지 최대 값
    private float currentGauge = 0f; // 현재 게이지 값
    private float countdownDuration = 5f; // 게임 지속 시간
    private float countdownTimeRemaining = 0f; // 남은 시간

    public Color normalColor = Color.green; // 정상 상태 색상
    public Color warningColor = Color.red; // 경고 상태 색상
    private Image gaugeImage; // GaugeImage 컴포넌트 참조 
    private Image backgroundImage;

    private GameMode currentMode; // 현재 게임 모드
    private bool isGameActive = false; // 게임 활성화 여부

    [Header("Marker Settings (for Marker Matching)")]
    private RectTransform marker; // 동그라미 마커
    private RectTransform centralBand; // 중앙 띠
    public float markerSpeed = 300f; // 동그라미 이동 속도
    private bool markerMovingRight = true; // 마커가 오른쪽으로 움직이는 중인지 여부

    private int totalGames = 3; // 총 3번 진행
    private int currentGameIndex = 0; // 현재 게임 인덱스
    private int successGames = 0; // 성공한 게임 횟수

    public delegate void MissionResultHandler(bool isSuccess);
    public event MissionResultHandler OnGameComplete; // 미션 성공/실패 이벤트

    void Awake()
    {
        // 계층 구조에서 자동으로 UI 요소 찾기
        Transform backgroundImageTransform = transform.Find("OutlineImage/BackgroundImage");
        if (backgroundImageTransform != null)
        {   

            backgroundImage = backgroundImageTransform.GetComponent<Image>();
            if (backgroundImage == null)
            {
                Debug.LogError("BackgroundImage does not have an Image component!");
            }

            gaugeBar = backgroundImageTransform.Find("GaugeImage")?.GetComponent<Image>();
            if (gaugeBar != null)
            {
                gaugeImage = gaugeBar.GetComponent<Image>();
            }
        }
        else
        {
            Debug.LogError("BackgroundImage not found! Please check the hierarchy.");
        }

        centralBand = transform.Find("CentralBand")?.GetComponent<RectTransform>();
        if (centralBand == null)
        {
            Debug.LogError("CentralBand not found! Please check the hierarchy.");
        }

        marker = transform.Find("Marker")?.GetComponent<RectTransform>();
        if (marker == null)
        {
            Debug.LogError("Marker not found! Please check the hierarchy.");
        }

        centralBand.gameObject.SetActive(false);
        marker.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isGameActive) return;

        // 카운트다운 시간 갱신
        countdownTimeRemaining -= Time.deltaTime;
        if (countdownTimeRemaining <= 0)
        {
            EndGame(false); // 시간 초과로 실패
            return;
        }

        switch (currentMode)
        {
            case GameMode.FillGaugeByClicking:
                HandleClickToFillGauge();
                break;
            case GameMode.CountdownGauge:
                HandleCountdown();
                break;
            case GameMode.MarkerMatching:
                HandleMarkerMovement();
                break;
        }
    }

    // --- 게임 모드 핸들러 ---
    private void HandleClickToFillGauge()
    {
        // 클릭으로 게이지 채우기
        if (Input.GetMouseButtonDown(0))
        {
            currentGauge += 0.2f;
            currentGauge = Mathf.Clamp(currentGauge, 0, maxGauge);
        }

        currentGauge -= fillSpeed * Time.deltaTime; // 자동 감소
        currentGauge = Mathf.Clamp(currentGauge, 0, maxGauge);

        UpdateGaugeUI();

        if (currentGauge >= maxGauge - 0.01f)
        {   
            currentGauge = 0.0f;
            UpdateGaugeUI();
            EndGame(true); // 성공
        }
    }

    private void HandleCountdown()
    {
        // 게이지 감소
        currentGauge -= (maxGauge / countdownDuration) * Time.deltaTime;
        currentGauge = Mathf.Clamp(currentGauge, 0, maxGauge);

        UpdateGaugeUI();

        if (currentGauge <= 0)
        {   
            EndGame(false); // 실패
        }
    }

    private void HandleMarkerMovement()
    {
        if (marker == null || centralBand == null) return;

        centralBand.gameObject.SetActive(true);
        marker.gameObject.SetActive(true);

        // 마커 이동
        float moveDirection = markerMovingRight ? 1f : -1f;
        marker.anchoredPosition += new Vector2(moveDirection * markerSpeed * Time.deltaTime, 0);

        // 마커가 게이지 바 끝을 넘어갈 경우 방향 반전
        if (marker.anchoredPosition.x >= gaugeBar.rectTransform.rect.width / 2)
        {
            markerMovingRight = false;
        }
        else if (marker.anchoredPosition.x <= -gaugeBar.rectTransform.rect.width / 2)
        {
            markerMovingRight = true;
        }

        // 클릭으로 중앙 띠와의 겹침 확인
        if (Input.GetMouseButtonDown(0))
        {
            bool isSuccess = isRectOverlaps(marker, centralBand);

            // 성공 여부에 따른 처리
            if (isSuccess)
            {
                successGames++;
                Debug.Log($"Game {currentGameIndex + 1}: Success!");
            }
            else
            {
                Debug.Log($"Game {currentGameIndex + 1}: Failed!");
            }

            // 성공/실패에 따른 색상 변경
            Color flashColor = isSuccess ? Color.green : Color.red; // 연두색 / 옅은 빨간색
            StartCoroutine(HandleFlashAndCheckEnd(flashColor));
        }
    }

    private IEnumerator HandleFlashAndCheckEnd(Color flashColor)
    {
        // Background 색상 플래시 처리
        yield return StartCoroutine(FlashBackgroundColor(flashColor));

        // 게임 진행 상태 업데이트
        currentGameIndex++;

        // 모든 게임이 끝난 경우
        if (currentGameIndex >= totalGames)
        {
            centralBand.gameObject.SetActive(false);
            marker.gameObject.SetActive(false);

            // 결과에 따라 EndGame 호출
            bool isSuccess = (successGames == totalGames);
            currentGameIndex = 0;
            successGames = 0;
            EndGame(isSuccess);
        }
    }


    // --- 공통 로직 ---
    private void UpdateGaugeUI()
    {
        gaugeBar.fillAmount = currentGauge / maxGauge;

        // 색상 업데이트
        gaugeImage.color = currentGauge / maxGauge <= 0.3f ? warningColor : normalColor;
    }

    private void EndGame(bool isSuccess)
    {
        isGameActive = false;
        OnGameComplete?.Invoke(isSuccess);
    }

    // --- 게임 시작/중단 ---
    public void StartGame(GameMode mode, float duration)
    {
        currentMode = mode;
        isGameActive = true;
        countdownDuration = duration;
        countdownTimeRemaining = duration;
        currentGauge = mode == GameMode.CountdownGauge ? maxGauge : 0f;
    }

    public void StopGame()
    {
        isGameActive = false;
    }

    // --- 유틸리티 ---
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
    private IEnumerator FlashBackgroundColor(Color flashColor)
    {
        // 색상 변경
        backgroundImage.color = flashColor;

        // 잠시 대기
        yield return new WaitForSeconds(0.2f);

        // 원래 색상으로 복원
        backgroundImage.color = Color.white;
    }
}
