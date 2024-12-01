using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour
{
    public OrderManager orderManager;
    public RectTransform orderListContainer;
    public GameObject orderEntryPrefab;
    [SerializeField] private GameObject orderListScreen;
    
    void Start()
    {
        
    }

    void Update()
    {
        DisplayOrders();
    }

    public void DisplayOrders()
    {
        foreach (Transform child in orderListContainer)
        {
            Destroy(child.gameObject);
        }

        List<Order> orders = orderManager.GetAllOrders();

        foreach (Order order in orders)
        {
            GameObject orderEntry = Instantiate(orderEntryPrefab, orderListContainer);

            RawImage iconImage = orderEntry.transform.Find("RawImage").GetComponent<RawImage>();
            TextMeshProUGUI menuText = orderEntry.transform.Find("MenuText").GetComponent<TextMeshProUGUI>();

            if (iconImage != null && order.dish.icon != null)
            {
                iconImage.texture = order.dish.icon;
            }

            if (menuText != null)
            {
                menuText.text = $"{order.dish.food}";
            }
        }
    }

    public void CloseOrderList()
    {
        orderListScreen.SetActive(false);
    }

    public void OpenOrderList()
    {
        orderListScreen.SetActive(true);
    }
}
