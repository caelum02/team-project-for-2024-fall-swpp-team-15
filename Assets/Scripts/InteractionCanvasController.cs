using UnityEngine;

public class InteractionCanvasController : MonoBehaviour
{
    private Transform cookingStation; // 조리도구 Transform
    private RectTransform interactionPanel; // InteractionPanel (UI 위치를 업데이트할 요소 1)
    private RectTransform selectionPanel; // SelectionPanel (UI 위치를 업데이트할 요소 2)
    public Vector2 screenOffset = new Vector2(70f, 70f); // 화면 기준 오프셋
    private Camera mainCamera; // 연결된 카메라
    private RectTransform canvasRectTransform; // Canvas의 RectTransform

    private void Start()
    {
        // 메인 카메라 가져오기
        mainCamera = Camera.main;

        // CookingStation과 InteractionPanel 자동으로 찾기
        cookingStation = transform.parent; // 부모 객체가 CookingStation
        if (cookingStation == null)
        {
            Debug.LogError("CookingStation not found as parent!");
            return;
        }

        // Canvas와 InteractionPanel 찾기
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
        interactionPanel = transform.Find("InteractionPanel")?.GetComponent<RectTransform>();
        if (interactionPanel == null)
        {
            Debug.LogError("InteractionPanel not found!");
        }
        selectionPanel = transform.Find("SelectionPanel")?.GetComponent<RectTransform>();
        if (selectionPanel == null)
        {
            Debug.LogError("SelectionPanel not found!");
        }
    }

    private void Update()
    {
        UpdateUIPosition();
    }

    private void UpdateUIPosition()
    {
        if (cookingStation == null || interactionPanel == null || mainCamera == null || canvasRectTransform == null) return;

        // 조리도구의 월드 좌표를 화면 좌표로 변환
        Vector3 worldPosition = cookingStation.position;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPosition);

        // 화면 좌표를 Canvas 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform, screenPoint, mainCamera, out Vector2 localPoint);

        // 화면 경계를 기준으로 오프셋 계산
        Vector2 adjustedOffset = CalculateOffset(screenPoint);

        // InteractionPanel의 로컬 좌표 설정
        interactionPanel.localPosition = localPoint + adjustedOffset;
        selectionPanel.localPosition = interactionPanel.localPosition;
    }

    private Vector2 CalculateOffset(Vector2 screenPoint)
    {   
        // 기본 배치
        Vector2 adjustedOffset = new Vector2(screenOffset.x, screenOffset.y);

        if (screenPoint.y > Screen.height * 0.8f)
        {
            // 화면 위쪽에 가까운 경우 아래쪽으로 배치
            adjustedOffset.y = 0.0f;
        }
        else if (screenPoint.y < Screen.height * 0.2f)
        {
            // 화면 아래쪽에 가까운 경우 위쪽으로 배치
            adjustedOffset.y = 2.0f * screenOffset.y;
        } 

        if (screenPoint.x > Screen.width * 0.8f)
        {
            // 화면 오른쪽에 가까운 경우 왼쪽으로 배치
            adjustedOffset.x = -screenOffset.x;

        }
        if (screenPoint.x < Screen.width * 0.2f)
        {
            // 화면 왼쪽에 가까운 경우 오른쪽으로 배치
            adjustedOffset.x = 1.5f * screenOffset.x;
        }

        return adjustedOffset;
    }
}
