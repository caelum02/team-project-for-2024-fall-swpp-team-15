using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
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
    protected Transform uiMenu; // Canvas들의 부모 객체
    protected Canvas interactionCanvas; // 상호작용 Canvas (자식 객체로 설정)
    // protected Canvas cookingStationCanvas; // 상호작용 Canvas (자식 객체로 설정)
    protected Transform interactionMenu; // 상호작용 메뉴
    protected Transform interactionPanel; // 상호작용 패널

    [Header("activeStation")]
    public static KitchenInteriorBase activeStation; // 현재 활성화된 기구

    [Header("Database")]
    [SerializeField] protected FoodDatabaseSO foodDatabase; // 음식 데이터베이스

    private static List<KitchenInteriorBase> allStations = new List<KitchenInteriorBase>(); // 모든 조리기구를 저장하는 리스트
    private GameManager gameManager; // GameManager 참조

    /// <summary>
    /// 기구 초기화. UI 요소를 설정하고 초기 상태를 설정합니다.
    /// </summary>
    protected virtual void Start()
    {   
        // GameManager 찾기
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
            return;
        }

        // UIMenu를 찾습니다.
        uiMenu = transform.Find("UIMenu");
        if (uiMenu== null)
        {
            Debug.LogError($"UIMenu not found in {gameObject.name}");
            return;
        }

        // InteractionCanvas를 찾습니다.
        interactionCanvas = uiMenu.Find("InteractionCanvas").GetComponent<Canvas>();
        if (interactionCanvas  == null)
        {
            Debug.LogError($"interactionCanvas not found in {gameObject.name}");
            return;
        }

        // InteractionMenu를 찾습니다.
        interactionMenu = interactionCanvas.transform.Find("InteractionMenu");
        if (interactionMenu == null)
        {
            Debug.LogError($"InteractionMenu not found in {gameObject.name}");
            return;
        }

        // InteractionPanel을 찾습니다.
        interactionPanel = interactionMenu.transform.Find("InteractionPanel");
        if (interactionPanel == null)
        {
            Debug.LogError($"InteractionPanel not found in {gameObject.name}");
            return;
        }

        // Canvas는 기본적으로 활성화 상태입니다.
        interactionCanvas.gameObject.SetActive(true);

        // InteractionMenu 초기화: 비활성화 (가까이 왔을 때 활성화되도록)
        interactionMenu.gameObject.SetActive(false);
        interactionPanel.gameObject.SetActive(true);

        // 조리기구를 리스트에 추가
        allStations.Add(this);
    }

    private void OnDestroy()
    {
        // 오브젝트 파괴 시 리스트에서 제거
        allStations.Remove(this);
    }

    /// <summary>
    /// 매 프레임마다 상호작용 메뉴를 처리하고 버튼 상태를 업데이트합니다.
    /// </summary>
    protected virtual void Update()
    {   
        UpdateUIMenu();
        HandleInteractionMenu();
        UpdateAllButtons();
    }

    /// <summary>
    /// UIMenu 활성화/비활성화 로직
    /// </summary>
    private void UpdateUIMenu()
    {
        uiMenu.gameObject.SetActive(gameManager.openOrCloseText.text != "정비 시간");
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
        if (PlayerController.Instance == null || uiMenu == null || interactionCanvas == null || interactionMenu == null) return;

        KitchenInteriorBase closestStation = GetClosestStation();

        if (closestStation == null && activeStation != null) 
        {   
            activeStation.HideMenu();
            activeStation = null;
            return;
        }
        if (activeStation != closestStation)
        {
            // 활성화된 기구가 변경되었으면 이전 메뉴를 숨기고 새로운 메뉴를 표시
            if (activeStation != null)
            {
                activeStation.HideMenu();
            }

            activeStation = closestStation;
            activeStation.ShowMenu();
        }
    }

    // protected virtual void HandleInteractionMenu()
    // {
    //     // PlayerController 또는 필수 요소가 없으면 처리하지 않음
    //     if (PlayerController.Instance == null|| cookingStationCanvas == null || interactionMenu == null) return;

    //     // 플레이어와 기구 간의 거리 계산
    //     float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);

    //     if (distanceToPlayer <= 3f) // 플레이어가 상호작용 범위 내에 있을 경우
    //     {
    //         if (activeStation == null || activeStation == this)
    //         {
    //             ShowMenu(); // 상호작용 메뉴 표시
    //             activeStation = this;
    //         }
    //     }
    //     else // 플레이어가 범위에서 벗어난 경우
    //     {
    //         if (activeStation == this)
    //         {
    //             HideMenu(); // 상호작용 메뉴 숨기기
    //             activeStation = null;
    //         }
    //     }
    // }

    /// <summary>
    /// 가장 가까운 조리기구를 반환합니다.
    /// </summary>
    private KitchenInteriorBase GetClosestStation()
    {   
        KitchenInteriorBase closestStation = null;
        float closestDistance = float.MaxValue;

        if (PlayerController.Instance == null) return closestStation;

        Vector3 playerPosition = PlayerController.Instance.transform.position;

        foreach (var station in allStations)
        {
            float distance = Vector3.Distance(playerPosition, station.transform.position);

            if (distance < 3f && distance < closestDistance) // 3f 이내에서 가장 가까운 조리기구 찾기
            {
                closestStation = station;
                closestDistance = distance;
            }
        }

        return closestStation;
    }

    /// <summary>
    /// 상호작용 메뉴를 활성화합니다.
    /// </summary>
    private void ShowMenu()
    {
        if (uiMenu != null && interactionCanvas != null && interactionMenu != null)
        {
            interactionMenu.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 상호작용 메뉴를 숨기고 초기 상태로 복원합니다.
    /// </summary>
    protected virtual void HideMenu()
    {
        if (uiMenu != null && interactionCanvas != null && interactionMenu != null && interactionPanel != null)
        {
            interactionMenu.gameObject.SetActive(false); // 메뉴 숨기기
            interactionPanel.gameObject.SetActive(true);
        }
    }
}
