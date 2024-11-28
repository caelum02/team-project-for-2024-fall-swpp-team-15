using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;
using System.Collections; // 코루틴에서 필요한 네임스페이스
using Yogaewonsil.Common;
using TMPro; // TextMeshPro 사용을 위한 네임스페이스

public abstract class KitchenInteriorBase : MonoBehaviour
{

    [Header("Station Setting")]
    public string stationName; // 기구 이름

    [Header("UI References")]
    protected Canvas cookingStationCanvas; // 상호작용 Canvas (자식 객체로 설정)
    protected Transform interactionMenu;
    protected Transform interactionPanel;

    [Header("Animator")]
    protected Animator animator;

    [Header("activeStation")]
    public static KitchenInteriorBase activeStation; // 현재 활성화된 조리기구

    [Header("Database")]
    [SerializeField] private FoodDatabaseSO foodDatabase; 

    protected virtual void Start()
    { 
        // Animator 자동 연결
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on this GameObject or its children!");
        }

        // 캔버스 찾기
        cookingStationCanvas = transform.Find("CookingStationCanvas").GetComponent<Canvas>();
        if (cookingStationCanvas == null)
        {
            Debug.LogError($"cookingStationCanvas not found in {gameObject.name}");
            return;
        }

        // InteractionMenu 찾기
        interactionMenu = cookingStationCanvas.transform.Find("InteractionMenu");
        if (interactionMenu == null)
        {
            Debug.LogError($"InteractionMenu not found in {cookingStationCanvas.name}");
            return;
        }

        // InteractionPanel 찾기
        interactionPanel = interactionMenu.transform.Find("InteractionPanel");
        if (interactionPanel == null)
        {
            Debug.LogError($"InteractionPanel not found in {cookingStationCanvas.name}");
            return;
        }

        // Canvas는 언제나 활성화
        cookingStationCanvas.gameObject.SetActive(true);

        // 초기화
        interactionMenu.gameObject.SetActive(false);
        interactionPanel.gameObject.SetActive(true);

    }


    protected virtual void Update()
    {
        HandleInteractionMenu();
        UpdateAllButtons();
    }

    protected virtual void HandleInteractionMenu()
    {

        if (PlayerController.Instance == null|| cookingStationCanvas == null || interactionMenu == null) return;

        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);

        if (distanceToPlayer <= 3f) // 상호작용 범위 내
        {
            if (activeStation == null || activeStation == this)
            {
                ShowMenu();
                activeStation = this;
            }
        }
        else
        {
            if (activeStation == this)
            {
                HideMenu();
                activeStation = null;
            }
        }
    }

    protected virtual void UpdateAllButtons()
    {

    }

    private void ShowMenu()
    {
        if (cookingStationCanvas != null && interactionMenu != null)
        {
            interactionMenu.gameObject.SetActive(true);
        }
    }

    private void HideMenu()
    {
        if (cookingStationCanvas != null && interactionMenu != null)
        {
            interactionMenu.gameObject.SetActive(false);
            interactionPanel.gameObject.SetActive(true);
        }
    }
}
