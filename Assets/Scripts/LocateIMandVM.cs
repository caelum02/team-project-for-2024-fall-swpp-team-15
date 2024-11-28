using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocateIMandVM : LocateIMOnly
{
    private RectTransform visualMenu; // visualMenu
    public Vector2 visualMenuOffset = new Vector2(0.0f, 120f); // visualMenu에 적용되는 오프셋

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

    protected override void UpdatePanelsPosition(Vector2 screenPoint, Vector2 localPoint)
    {   
        base.UpdatePanelsPosition(screenPoint, localPoint);

        Vector2 adjustedVMOffset = CalculateVMOffset(screenPoint);

        visualMenu.localPosition = localPoint + adjustedVMOffset;
    }

    private Vector2 CalculateVMOffset(Vector2 screenPoint)
    {   
        // 기본 배치
        Vector2 adjustedVMOffset = new Vector2(visualMenuOffset.x, visualMenuOffset.y);

        if (screenPoint.y > Screen.height * 0.9f)
        {
            // 화면 위쪽에 가까운 경우 아래쪽으로 배치
            adjustedVMOffset.y = - 2* visualMenuOffset.y / 3;
        }

        if (screenPoint.x > Screen.width * 0.9f)
        {
            // 화면 오른쪽에 가까운 경우 왼쪽으로 배치
            //adjustedVisualMenuOffset.x = -70f;
        }
        if (screenPoint.x < Screen.width * 0.05f)
        {
            // 화면 왼쪽에 가까운 경우 오른쪽으로 배치
            //adjustedVisualMenuOffset.x = 70f;
        }

        return adjustedVMOffset;
    }
}
