using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CookingStationCanvasBase : MonoBehaviour
{
    private Transform cookingStation; // 조리도구 Transform
    private RectTransform interactionMenu;
    private RectTransform visualMenu; // visualMenu
    public Vector2 interactionMenuOffset = new Vector2(120f, 120f); // InteractionPanel과 SelectionPanel에 적용되는 오프셋
    public Vector2 visualMenuOffset = new Vector2(0.0f, 120f); // visualMenu에 적용되는 오프셋
    private Camera mainCamera; // 연결된 카메라
    private RectTransform canvasRectTransform; // Canvas의 RectTransform

    protected virtual void Start()
    {
        // 메인 카메라 가져오기
        mainCamera = Camera.main;

        // CookingStation 자동으로 찾기
        cookingStation = transform.parent; // 부모 객체가 CookingStation
        if (cookingStation == null)
        {
            Debug.LogError("CookingStation not found as parent!");
            return;
        }

        // Canvas와 InteractionMenu 찾기
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

        canvasRectTransform = canvas.GetComponent<RectTransform>();

        // InteractionMenu 찾기
        interactionMenu = transform.Find("InteractionMenu")?.GetComponent<RectTransform>();
        if (interactionMenu == null)
        {
            Debug.LogError("InteractionMenu not found!");
        }

        // visualMenu 찾기
        visualMenu = transform.Find("VisualMenu")?.GetComponent<RectTransform>();
        if (visualMenu == null)
        {
            Debug.LogError("VisualMenu not found!!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUIPosition();
    }

    protected virtual void UpdateUIPosition()
    {
        if (cookingStation == null || interactionMenu == null || visualMenu == null || mainCamera == null || canvasRectTransform == null) return;

        // 조리도구의 월드 좌표를 화면 좌표로 변환
        Vector3 worldPosition = cookingStation.position;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPosition);

        // 화면 좌표를 Canvas 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform, screenPoint, mainCamera, out Vector2 localPoint);

        // 화면 경계를 기준으로 오프셋 계산
        Vector2 adjustedIMOffset = CalculateIMOffset(screenPoint);
        Vector2 adjustedVisualMenuOffset = CalculateVisualMenuOffset(screenPoint);

        // InteractionPanel과 SelectionPanel의 로컬 좌표 설정
        interactionMenu.localPosition = localPoint + adjustedIMOffset;

        // VisualMenu의 로컬 좌표 설정
        visualMenu.localPosition = localPoint + adjustedVisualMenuOffset;
    }

    private Vector2 CalculateIMOffset(Vector2 screenPoint)
    {   
        // 기본 배치
        Vector2 adjustedIMOffset = new Vector2(interactionMenuOffset.x, interactionMenuOffset.y);

        if (screenPoint.y > Screen.height * 0.8f)
        {
            // 화면 위쪽에 가까운 경우 아래쪽으로 배치
            adjustedIMOffset.y = 0.0f;
        }
        else if (screenPoint.y < Screen.height * 0.2f)
        {
            // 화면 아래쪽에 가까운 경우 위쪽으로 배치
            adjustedIMOffset.y = 2.0f * interactionMenuOffset.y;
        } 

        if (screenPoint.x > Screen.width * 0.8f)
        {
            // 화면 오른쪽에 가까운 경우 왼쪽으로 배치
            adjustedIMOffset.x = -interactionMenuOffset.x;

        }
        if (screenPoint.x < Screen.width * 0.2f)
        {
            // 화면 왼쪽에 가까운 경우 오른쪽으로 배치
            adjustedIMOffset.x = interactionMenuOffset.x;
        }

        return adjustedIMOffset;
    }

    private Vector2 CalculateVisualMenuOffset(Vector2 screenPoint)
    {   
        // 기본 배치
        Vector2 adjustedVisualMenuOffset = new Vector2(visualMenuOffset.x, visualMenuOffset.y);

        if (screenPoint.y > Screen.height * 0.8f)
        {
            // 화면 위쪽에 가까운 경우 아래쪽으로 배치
            adjustedVisualMenuOffset.y = - 2* visualMenuOffset.y / 3;
        }

        if (screenPoint.x > Screen.width * 0.8f)
        {
            // 화면 오른쪽에 가까운 경우 왼쪽으로 배치
            //adjustedVisualMenuOffset.x = -70f;
        }
        if (screenPoint.x < Screen.width * 0.2f)
        {
            // 화면 왼쪽에 가까운 경우 오른쪽으로 배치
            //adjustedVisualMenuOffset.x = 70f;
        }

        return adjustedVisualMenuOffset;
    }
}
