using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 손님 주문 UI
/// </summary>
public class OrderUI : MonoBehaviour
{
    /// <summary>
    /// 주문 데이터를 관리하는 OrderManager
    /// </summary>
    public OrderManager orderManager;

    /// <summary>
    /// 모든 주문 항목을 담고 있는 UI 컨테이너 (Contnet)
    /// </summary>
    public RectTransform orderListContainer;

    /// <summary>
    /// 주문 내역 UI에 뜨는 각 주문 항목의 프리팹
    /// </summary>
    public GameObject orderEntryPrefab;

    /// <summary>
    /// 주문 내역 화면 UI
    /// </summary>
    [SerializeField] private GameObject orderListScreen;
    
    void Start()
    {
        
    }

    void Update()
    {
        DisplayOrders();
    }

    /// <summary>
    /// 현재 저장된 주문을 UI에 표시
    /// - 이전에 표시된 주문 항목을 모두 제거
    /// - 새로 받은 주문을 UI에 추가
    /// </summary>
    public void DisplayOrders()
    {
        // 기존 UI 항목 제거
        foreach (Transform child in orderListContainer)
        {
            Destroy(child.gameObject);
        }

        // 현재 주문 목록 가져오기
        List<Order> orders = orderManager.GetAllOrders();

        // 각 주문을 UI에 표시
        foreach (Order order in orders)
        {
            GameObject orderEntry = Instantiate(orderEntryPrefab, orderListContainer);

            // 주문 아이콘 설정
            RawImage iconImage = orderEntry.transform.Find("RawImage").GetComponent<RawImage>();
            if (iconImage != null && order.dish.icon != null)
            {
                iconImage.texture = order.dish.icon;
            }

            // 메뉴 이름 설정
            TextMeshProUGUI menuText = orderEntry.transform.Find("MenuText").GetComponent<TextMeshProUGUI>();
            if (menuText != null)
            {
                menuText.text = $"{order.dish.food}";
            }
        }
    }

    /// <summary>
    /// 주문 목록 화면 닫기
    /// </summary>
    public void CloseOrderList()
    {
        orderListScreen.SetActive(false);
    }

    /// <summary>
    /// 주문 목록 화면 열기
    /// </summary>
    public void OpenOrderList()
    {
        orderListScreen.SetActive(true);
    }
}
