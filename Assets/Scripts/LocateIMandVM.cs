using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LocateIMandVM은 LocateIMOnly를 상속하여 InteractionMenu뿐만 아니라 VisualMenu도 동적으로 배치하는 기능을 추가한 클래스입니다.
/// </summary>
public class LocateIMandVM : LocateIMOnly
{
    private RectTransform visualMenu; // VisualMenu의 RectTransform
    public Vector2 visualMenuOffset = new Vector2(0.0f, 120f); // VisualMenu 위치를 조정하는 기본 오프셋

    /// <summary>
    /// 초기화 메서드로, VisualMenu를 찾아 설정하고 부모 클래스의 초기화를 호출합니다.
    /// </summary>
    protected override void Start()
    {   
        base.Start();

        // visualMenu 찾기
        visualMenu = transform.Find("VisualMenu")?.GetComponent<RectTransform>();
        if (visualMenu == null)
        {
            Debug.LogError("VisualMenu not found!!");
        }
    }

    /// <summary>
    /// InteractionMenu와 VisualMenu의 위치를 각각 업데이트합니다.
    /// </summary>
    /// <param name="screenPoint">화면 좌표</param>
    /// <param name="localPoint">Canvas 로컬 좌표</param>
    protected override void UpdatePanelsPosition(Vector2 screenPoint, Vector2 localPoint)
    {      
        // 부모 클래스에서 InteractionMenu의 위치를 업데이트
        base.UpdatePanelsPosition(screenPoint, localPoint);

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

        // 화면 상단에 가까운 경우
        if (screenPoint.y > Screen.height * 0.9f)
        {
            adjustedVMOffset.y = - 2* visualMenuOffset.y / 3; // 아래쪽으로 배치
        }
        // 화면 오른쪽에 가까운 경우 (현재 비활성화된 코드)
        // 굳이 조정하지 않아도 잘 보여서 비활성화
        if (screenPoint.x > Screen.width * 0.9f)
        {
            //adjustedVisualMenuOffset.x = -70f; // 왼쪽으로 배치
        }
        // 화면 왼쪽에 가까운 경우 (현재 비활성화된 코드)
        // 굳이 조정하지 않아도 잘 보여서 비활성화
        if (screenPoint.x < Screen.width * 0.05f)
        {
            //adjustedVisualMenuOffset.x = 70f; // 오른쪽으로 배치
        }

        return adjustedVMOffset;
    }
}
