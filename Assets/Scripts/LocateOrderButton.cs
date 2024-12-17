using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocateOrderButton : MonoBehaviour
{   
    [SerializeField] protected Transform referencePoint; // 기준점 Transform
    private RectTransform canvasRectTransform; // Canvas의 RectTransform
    protected RectTransform orderButton; // OrderButton 의 RectTransform
    private Camera mainCamera; // 메인 카메라 참조
    // Start is called before the first frame update
    void Start()
    {
        // 메인 카메라 가져오기
        mainCamera = Camera.main;

        // Canvas 찾기
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null )
        {
            Debug.LogError("Canvas not found!");
            return;
        }

        // Canvas의 Render Mode를 Screen Space - Camera 로 설정
        if (canvas.renderMode != RenderMode.ScreenSpaceCamera)
        {
            Debug.LogWarning("Canvas render mode is not Screen Space - Camera. Adjusting...");
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = mainCamera; // Render 카메라 설정
        }

        canvas.planeDistance = 4f;

        canvasRectTransform = canvas.GetComponent<RectTransform>();

        // OrderButton 찾기
        orderButton= transform.Find("OrderButton")?.GetComponent<RectTransform>();
        if (orderButton == null)
        {
            Debug.Log("OrderButton not found!");
        }
    }
    
    /// <summary>
    /// 매 프레임마다 호출되어 OrderButton의 위치를 업데이트합니다.
    /// </summary>
    void Update()
    {
        CalculateLocalCoordinate();
    }

    /// <summary>
    /// Customer의 화면 좌표를 Canvas 로컬 좌표로 변환하여 OrderButton 위치를 계산합니다.
    /// </summary>
    private void CalculateLocalCoordinate()
    {
        if (referencePoint == null || orderButton == null || mainCamera == null || canvasRectTransform == null) return;

        // referencePoint의 월드 좌표를 화면 좌표로 변환
        Vector3 worldPosition = referencePoint.position;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPosition);

        // 화면 좌표를 Canvas 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvasRectTransform, screenPoint, mainCamera, out Vector2 localPoint);

        // OrderButton의 로컬 좌표 설정
        orderButton.localPosition = localPoint;
    }
}
