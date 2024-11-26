using UnityEngine;
using UnityEngine.UI;

public class GaugeBar : MonoBehaviour
{
    public Image gaugeBar; // 게이지를 채울 이미지
    public float fillSpeed = 0.1f; // 자동 감소 속도
    public float maxGauge = 1f; // 게이지 최대 값
    private float currentGauge = 0f; // 현재 게이지 값
    private bool isFilling = false; // 게이지가 채워지는 중인지 여부

    public delegate void GaugeCompleteHandler();
    public event GaugeCompleteHandler OnGaugeComplete; // 게이지가 가득 찼을 때 이벤트

    void Update()
    {
        if (!isFilling) return;

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

        // 4. 게이지가 가득 찼는지 확인
        if (currentGauge >= maxGauge - 0.05f)
        {
            isFilling = false; // 게이지 채우기 중단
            OnGaugeComplete?.Invoke(); // 이벤트 호출
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
}
