using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocateIMOnly : MonoBehaviour
{
    protected Transform cookingStation; // 조리도구 Transform
    protected RectTransform interactionMenu;
    public Vector2 interactionMenuOffset = new Vector2(120f, 120f); // InteractionPanel과 SelectionPanel에 적용되는 오프셋
    protected Camera mainCamera; // 연결된 카메라
    protected RectTransform canvasRectTransform; // Canvas의 RectTransform

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
    }

    // Update is called once per frame
    void Update()
    {
        CalculateLocalCoordinate();
    }

    private void CalculateLocalCoordinate()
    {
        if (cookingStation == null || interactionMenu == null || mainCamera == null || canvasRectTransform == null) return;

        // 조리도구의 월드 좌표를 화면 좌표로 변환
        Vector3 worldPosition = cookingStation.position;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPosition);

        // 화면 좌표를 Canvas 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvasRectTransform, screenPoint, mainCamera, out Vector2 localPoint);

        UpdatePanelsPosition(screenPoint, localPoint);
    }

    protected virtual void UpdatePanelsPosition(Vector2 screenPoint, Vector2 localPoint)
    {
        Vector2 adjustedIMOffset = CalculateIMOffset(screenPoint);

        // InteractionPanel과 SelectionPanel의 로컬 좌표 설정
        interactionMenu.localPosition = localPoint + adjustedIMOffset;
    }

    private Vector2 CalculateIMOffset(Vector2 screenPoint)
    {   
        // 기본 배치
        Vector2 adjustedIMOffset = new Vector2(interactionMenuOffset.x, interactionMenuOffset.y);

        if (screenPoint.y > Screen.height * 0.9f)
        {
            // 화면 위쪽에 가까운 경우 아래쪽으로 배치
            adjustedIMOffset.y = 0.0f;
        }
        else if (screenPoint.y < Screen.height * 0.1f)
        {
            // 화면 아래쪽에 가까운 경우 위쪽으로 배치
            adjustedIMOffset.y = 2.0f * interactionMenuOffset.y;
        } 

        if (screenPoint.x > Screen.width * 0.9f)
        {
            // 화면 오른쪽에 가까운 경우 왼쪽으로 배치
            adjustedIMOffset.x = -interactionMenuOffset.x;

        }
        if (screenPoint.x <  Screen.width * 0.7f)
        {
            adjustedIMOffset.x = 3 * interactionMenuOffset.x / 4;
        }
        if (screenPoint.x < Screen.width * 0.5f)
        {
            // 화면 왼쪽에 가까운 경우 오른쪽으로 배치
            adjustedIMOffset.x = interactionMenuOffset.x / 2;
        }

        return adjustedIMOffset;
    }
}
