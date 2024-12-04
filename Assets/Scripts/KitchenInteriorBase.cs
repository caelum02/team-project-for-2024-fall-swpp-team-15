using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;
using System.Collections;
using Yogaewonsil.Common;
using TMPro;

/// <summary>
/// 주방 내부의 기본 기구 클래스입니다.
/// 기구의 기본적인 상호작용 및 UI 설정을 관리합니다.
/// </summary>
public abstract class KitchenInteriorBase : MonoBehaviour
{

    [Header("Station Setting")]
    public string stationName; // 기구 이름

    [Header("UI References")]
    protected Canvas cookingStationCanvas; // 상호작용 Canvas (자식 객체로 설정)
    protected Transform interactionMenu; // 상호작용 메뉴
    protected Transform interactionPanel; // 상호작용 패널

    [Header("activeStation")]
    public static KitchenInteriorBase activeStation; // 현재 활성화된 기구

    [Header("Database")]
    [SerializeField] protected FoodDatabaseSO foodDatabase; // 음식 데이터베이스

    /// <summary>
    /// 기구 초기화. UI 요소를 설정하고 초기 상태를 설정합니다.
    /// </summary>
    protected virtual void Start()
    { 
        // CookingStationCanvas를 찾습니다.
        cookingStationCanvas = transform.Find("CookingStationCanvas").GetComponent<Canvas>();
        if (cookingStationCanvas == null)
        {
            Debug.LogError($"cookingStationCanvas not found in {gameObject.name}");
            return;
        }

        // InteractionMenu를 찾습니다.
        interactionMenu = cookingStationCanvas.transform.Find("InteractionMenu");
        if (interactionMenu == null)
        {
            Debug.LogError($"InteractionMenu not found in {cookingStationCanvas.name}");
            return;
        }

        // InteractionPanel을 찾습니다.
        interactionPanel = interactionMenu.transform.Find("InteractionPanel");
        if (interactionPanel == null)
        {
            Debug.LogError($"InteractionPanel not found in {cookingStationCanvas.name}");
            return;
        }

        // Canvas는 기본적으로 활성화 상태입니다.
        cookingStationCanvas.gameObject.SetActive(true);

        // InteractionMenu 초기화: 비활성화 (가까이 왔을 때 활성화되도록)
        interactionMenu.gameObject.SetActive(false);
        interactionPanel.gameObject.SetActive(true);

    }

    /// <summary>
    /// 매 프레임마다 상호작용 메뉴를 처리하고 버튼 상태를 업데이트합니다.
    /// </summary>
    protected virtual void Update()
    {
        HandleInteractionMenu();
        UpdateAllButtons();
    }

    /// <summary>
    /// 버튼 상태를 업데이트합니다.
    /// 이 메서드는 상속받은 클래스에서 구현됩니다.
    /// </summary>
    protected virtual void UpdateAllButtons()
    {

    }

    /// <summary>
    /// 플레이어와의 거리 기반으로 상호작용 메뉴를 처리합니다.
    /// </summary>
    protected virtual void HandleInteractionMenu()
    {
        // PlayerController 또는 필수 요소가 없으면 처리하지 않음
        if (PlayerController.Instance == null|| cookingStationCanvas == null || interactionMenu == null) return;

        // 플레이어와 기구 간의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);

        if (distanceToPlayer <= 3f) // 플레이어가 상호작용 범위 내에 있을 경우
        {
            if (activeStation == null || activeStation == this)
            {
                ShowMenu(); // 상호작용 메뉴 표시
                activeStation = this;
            }
        }
        else // 플레이어가 범위에서 벗어난 경우
        {
            if (activeStation == this)
            {
                HideMenu(); // 상호작용 메뉴 숨기기
                activeStation = null;
            }
        }
    }

    /// <summary>
    /// 상호작용 메뉴를 활성화합니다.
    /// </summary>
    private void ShowMenu()
    {
        if (cookingStationCanvas != null && interactionMenu != null)
        {
            interactionMenu.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 상호작용 메뉴를 숨기고 초기 상태로 복원합니다.
    /// </summary>
    protected virtual void HideMenu()
    {
        if (cookingStationCanvas != null && interactionMenu != null && interactionPanel != null)
        {
            interactionMenu.gameObject.SetActive(false); // 메뉴 숨기기
            interactionPanel.gameObject.SetActive(true);
        }
    }
}
