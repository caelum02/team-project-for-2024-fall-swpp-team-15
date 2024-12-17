using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Legacy 파일: LocateIMOnly 파일로 대체
/// </summary>

public class InteractionCanvasController : MonoBehaviour
{

    [SerializeField] private Transform referencePoint; // 기준점 Transform
    private RectTransform interactionMenu; // InteractionMenu의 RectTransform
    public Vector2 interactionMenuOffset = new Vector2(120f, 120f); // InteractionMenu 위치 조정을 위한 기본 오프셋
    private Camera mainCamera; // 메인 카메라 참조
    private RectTransform canvasRectTransform; // Canvas의 RectTransform

    /// <summary>
    /// 초기화 메서드로, 필요한 구성 요소를 자동으로 찾고 초기 설정을 진행합니다.
    /// </summary>
    private void Start()
    {
        // 메인 카메라 가져오기
        mainCamera = Camera.main;

        // Canvas 찾기
        Canvas interactionCanvas = transform.GetComponent<Canvas>();
        if (interactionCanvas == null )
        {
            Debug.LogError("Canvas not found!");
            return;
        }

        // InteractionCanvas의 Render Mode를 Screen Space - Camera 로 설정
        if (interactionCanvas.renderMode != RenderMode.ScreenSpaceCamera)
        {
            interactionCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            interactionCanvas.worldCamera = mainCamera; // Render 카메라 설정

            interactionCanvas.planeDistance = 2f;
        }

        canvasRectTransform = interactionCanvas.GetComponent<RectTransform>();

        // InteractionMenu 찾기
        interactionMenu = transform.Find("InteractionMenu")?.GetComponent<RectTransform>();
        if (interactionMenu == null)
        {
            Debug.LogError("InteractionMenu not found!");
        }
    }

    /// <summary>
    /// 매 프레임마다 호출되어 InteractionMenu의 위치를 업데이트합니다.
    /// </summary>
    void FixedUpdate()
    {
        CalculateLocalCoordinate();
    }

    /// <summary>
    /// 조리도구의 화면 좌표를 Canvas 로컬 좌표로 변환하여 InteractionMenu 위치를 계산합니다.
    /// </summary>
    private void CalculateLocalCoordinate()
    {
        if (referencePoint == null || interactionMenu == null || mainCamera == null || canvasRectTransform == null) return;

        // 조리도구의 월드 좌표를 화면 좌표로 변환
        Vector3 worldPosition = referencePoint.position;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPosition);

        // 화면 좌표를 Canvas 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvasRectTransform, screenPoint, mainCamera, out Vector2 localPoint);

        // InteractionMenu 위치 업데이트
        UpdatePanelsPosition(screenPoint, localPoint);
    }

    /// <summary>
    /// InteractionMenu의 위치를 업데이트합니다.
    /// </summary>
    /// <param name="screenPoint">화면 좌표</param>
    /// <param name="localPoint">Canvas 로컬 좌표</param>
    private void UpdatePanelsPosition(Vector2 screenPoint, Vector2 localPoint)
    {
        Vector2 adjustedIMOffset = CalculateIMOffset(screenPoint);

        // InteractionPanel과 SelectionPanel의 로컬 좌표 설정
        interactionMenu.localPosition = localPoint + adjustedIMOffset;
    }

    /// <summary>
    /// InteractionMenu의 오프셋을 화면 가장자리와의 거리에 따라 조정합니다.
    /// </summary>
    /// <param name="screenPoint">화면 좌표</param>
    /// <returns>조정된 오프셋</returns>
    private Vector2 CalculateIMOffset(Vector2 screenPoint)
    {   
        // 기본 배치
        Vector2 adjustedIMOffset = new Vector2(interactionMenuOffset.x, interactionMenuOffset.y);

        // 카메라 조정이 가능해지면서 필요없어진 기능 
        // 화면 상단에 가까운 경우
        // if (screenPoint.y > Screen.height * 0.9f)
        // {
        //     adjustedIMOffset.y = 0.0f; // 아래쪽으로 배치
        // }
        // // 화면 하단에 가까운 경우
        // else if (screenPoint.y < Screen.height * 0.1f)
        // {
        //     adjustedIMOffset.y = 2.0f * interactionMenuOffset.y; // 위쪽으로 배치
        // } 
        // // 화면 오른쪽에 가까운 경우
        // if (screenPoint.x > Screen.width * 0.9f)
        // {
        //     adjustedIMOffset.x = -interactionMenuOffset.x; // 왼쪽으로 배치
        // }
        // // 화면 왼쪽에 가까워질수록 오른쪽 간격을 줄임
        // if (screenPoint.x <  Screen.width * 0.7f)
        // {
        //     adjustedIMOffset.x = 3 * interactionMenuOffset.x / 4;
        // }
        // if (screenPoint.x < Screen.width * 0.5f)
        // {
        //     adjustedIMOffset.x = interactionMenuOffset.x / 2;
        // }

        return adjustedIMOffset;
    }
}
