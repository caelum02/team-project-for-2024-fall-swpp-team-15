using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LocateIMandVM은 LocateIMOnly를 상속하여 InteractionMenu뿐만 아니라 VisualMenu도 동적으로 배치하는 기능을 추가한 클래스입니다.
/// </summary>
public class VisualCanvasController : MonoBehaviour
{   
    [SerializeField] protected Transform referencePoint; // 기준점 Transform
    private RectTransform visualMenu; // InteractionMenu의 RectTransform
    public Vector2 visualMenuOffset = new Vector2(0.0f, 50f); // VisualMenu 위치를 조정하는 기본 오프셋
    protected Camera mainCamera; // 메인 카메라 참조
    protected RectTransform canvasRectTransform; // Canvas의 RectTransform


    /// <summary>
    /// 초기화 메서드로, VisualMenu를 찾아 설정하고 부모 클래스의 초기화를 호출합니다.
    /// </summary>
    private void Start()
    {   
        // 메인 카메라 가져오기
        mainCamera = Camera.main;

        // Canvas 찾기
        Canvas visualCanvas = transform.GetComponent<Canvas>();
        if (visualCanvas == null )
        {
            Debug.LogError("Canvas not found!");
            return;
        }

        // VisualCanvas의 Render Mode를 Screen Space - Camera 로 설정
        if (visualCanvas.renderMode != RenderMode.ScreenSpaceCamera)
        {
            Debug.LogWarning("visualCanvas render mode is not Screen Space - Camera. Adjusting...");
            visualCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            visualCanvas.worldCamera = mainCamera; // Render 카메라 설정

            visualCanvas.planeDistance = 3f;
        }

        canvasRectTransform = visualCanvas.GetComponent<RectTransform>();

        // VisualMenu 찾기
        visualMenu = transform.Find("VisualMenu")?.GetComponent<RectTransform>();
        if (visualMenu == null)
        {
            Debug.LogError("VisualMenu not found!");
        }
    }

    /// <summary>
    /// 매 프레임마다 호출되어 VisualMenu의 위치를 업데이트합니다.
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
        if (referencePoint == null || visualMenu == null || mainCamera == null || canvasRectTransform == null) return;

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
    /// VisualMenu의 위치를 업데이트합니다.
    /// </summary>
    /// <param name="screenPoint">화면 좌표</param>
    /// <param name="localPoint">Canvas 로컬 좌표</param>
    private void UpdatePanelsPosition(Vector2 screenPoint, Vector2 localPoint)
    {   
        // VisualMenu의 위치를 계산 및 업데이트
        Vector2 adjustedVMOffset = CalculateVMOffset(screenPoint);
        visualMenu.localPosition = localPoint + adjustedVMOffset;
    }

    /// <summary>
    /// VisualMenu의 위치를 화면의 가장자리와의 거리에 따라 조정합니다.
    /// </summary>
    /// <param name="screenPoint">화면 좌표</param>
    /// <returns>조정된 VisualMenu 오프셋</returns>
    private Vector2 CalculateVMOffset(Vector2 screenPoint)
    {   
        // 기본 배치
        Vector2 adjustedVMOffset = new Vector2(visualMenuOffset.x, visualMenuOffset.y);

        // 카메라 조정이 가능해지면서 필요없어진 기능
        // // 화면 상단에 가까운 경우
        // if (screenPoint.y > Screen.height * 0.9f)
        // {
        //     adjustedVMOffset.y = - 2* visualMenuOffset.y / 3; // 아래쪽으로 배치
        // }
        // // 화면 오른쪽에 가까운 경우 (현재 비활성화된 코드)
        // // 굳이 조정하지 않아도 잘 보여서 비활성화
        // if (screenPoint.x > Screen.width * 0.9f)
        // {
        //     //adjustedVisualMenuOffset.x = -70f; // 왼쪽으로 배치
        // }
        // // 화면 왼쪽에 가까운 경우 (현재 비활성화된 코드)
        // // 굳이 조정하지 않아도 잘 보여서 비활성화
        // if (screenPoint.x < Screen.width * 0.05f)
        // {
        //     //adjustedVisualMenuOffset.x = 70f; // 오른쪽으로 배치
        // }

        return adjustedVMOffset;
    }
}
