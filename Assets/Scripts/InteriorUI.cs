using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class InteriorUI : MonoBehaviour, IBuyable
{
    /// <summary>
    /// 음식 데이터베이스 ScriptableObject. 
    /// 모든 음식 데이터 저장 
    /// </summary>
    [SerializeField] private InteriorDatabaseSO interiorDatabase;

    [Header("Buttons")]
    public Button interiorButton;
    public Button tileButton;
    public Button utenStorageButton;
    public Button utenPositionButton;
    public Button utenMarketButton;
    public Button npcButton;
    public Button recipeButton;

    [Header("Scroll Rects")]
    public ScrollRect utenMarketScroll;
    public ScrollRect utenStorageScroll;

    [Header("Screens")]
    public Image buyOrNotScreen;
    public Image boughtScreen;
    public Image notEnoughMoneyScreen;

    private bool isInteriorClosed = true;
    private bool isUtenMarketClosed = true;
    private bool isUtenStorageClosed = true;
    public GameManager gameManager;
    Transform selectedInteriorItem;
    private bool isBoughtSuccessful;

    // Start is called before the first frame update
    void Start()
    {
        MakeInteriorButtonInvisible();
        Initialize();
    }

    // 정비 시간에 인테리어 버튼 활성화
    public void MakeInteriorButtonVisible()
    {
        interiorButton.gameObject.SetActive(true);
    }

    // 영업 시간에 인테리어 버튼 비활성화 
    public void MakeInteriorButtonInvisible()
    {
        interiorButton.gameObject.SetActive(false);
        CloseUtenMarket();
        CloseUtenStorage();
        CloseInteriorMenu();
    }

    private void ProcessStorageContent(Action<Transform, InteriorData> processItem)
    {
        Transform storageContent = utenStorageScroll.transform.Find("Viewport/Content");
        foreach (Transform item in storageContent)
        {
            InteriorData interiorData = GetInteriorData(item.name);
            if (interiorData != null)
            {
                processItem(item, interiorData);
            }
            else
            {
                Debug.LogWarning($"No InteriorData found for {item.name} in the InteriorDatabase.");
            }
        }
    }

    private void Initialize()
    {
        ProcessStorageContent((item, interiorData) => interiorData.InitializeStock());
    }

    private void UpdateStock()
    {
        ProcessStorageContent((item, interiorData) => UpdateStockText(item, interiorData));
    }

        private InteriorData GetInteriorData(string itemName)
        {
            return interiorDatabase.interiorData.Find(interior => interior.name == itemName);
        }

     private void UpdateStockText(Transform item, InteriorData interiorData)
    {
        Transform stockObject = item.Find("Stock");
        
        // 재고 텍스트 업데이트 
        if (stockObject != null) 
        {
            TextMeshProUGUI stockText = stockObject.GetComponent<TextMeshProUGUI>();
            if (stockText != null)
            {
                stockText.text = "x" + interiorData.stock.ToString();
            }
        }
    }

    public void BuyInterior(Transform interiorItem)
    {
        InteriorData interiorData = GetInteriorData(interiorItem.name);
        if (interiorData != null)
        {
            int interiorPrice = interiorData.price;
            if (gameManager.money >= interiorPrice)
            {
                interiorData.UpdateBuyingStatus();
                gameManager.UpdateMoney(interiorPrice, false);
                Debug.Log($"Dish '{interiorData.name}' bought successfully.");
                isBoughtSuccessful = true;
            }
            else
            {
                notEnoughMoneyScreen.gameObject.SetActive(true);
                Debug.Log($"Not enough money");
                isBoughtSuccessful = false;
            }
            
        }
    }

    private void UpdatePrice()
    {
        Transform marketContent = utenMarketScroll.transform.Find("Viewport/Content");
        foreach (Transform item in marketContent)
        {
            InteriorData interiorData = GetInteriorData(item.name);
            if (interiorData != null)
            {
                UpdatePriceText(item, interiorData);
            }
            else
            {
                Debug.LogWarning($"No FoodData found for {item.name} in the FoodDatabase.");
            }
        }
    }

    private void UpdatePriceText(Transform item, InteriorData interiorData)
    {
        Transform priceObject = item.Find("Price");
        
        // 가격 텍스트 업데이트 
        if (priceObject != null) 
        {
            TextMeshProUGUI priceText = priceObject.GetComponentInChildren<TextMeshProUGUI>();
            if (priceText != null)
            {
                priceText.text = "     " + interiorData.price.ToString();
            }
        }
    }

    // 인테리어 버튼 클릭 시
    public void OnClickInteriorMenu()
    {
        if (isInteriorClosed)
        {
            ExpandInteriorMenu();
        }
        else
        {
            CloseInteriorMenu();
        }
        isInteriorClosed = !isInteriorClosed;
    }

    // 조리도구 상점 버튼 클릭 시
    public void OnClickUtenMarket()
    {
        if (isUtenMarketClosed)
        {
            CloseUtenStorage();
            ExpandUtenMarket();
        }
        else
        {
            CloseUtenMarket();
        }
    }

    // 조리도구 보관함 버튼 클릭 시 
    public void OnClickUtenStorage()
    {
        if (isUtenStorageClosed)
        {
            CloseUtenMarket();
            ExpandUtenStorage();
        }
        else
        {
            CloseUtenStorage();
        }
    }

    // 인테리어 전체 메뉴 열기  
    private void ExpandInteriorMenu()
    {
        tileButton.gameObject.SetActive(true);
        utenStorageButton.gameObject.SetActive(true);
        utenPositionButton.gameObject.SetActive(true);
        utenMarketButton.gameObject.SetActive(true);
    }

    // 인테리어 전체 메뉴 닫기
    private void CloseInteriorMenu()
    {
        tileButton.gameObject.SetActive(false);
        utenStorageButton.gameObject.SetActive(false);
        utenPositionButton.gameObject.SetActive(false);
        utenMarketButton.gameObject.SetActive(false);
        CloseUtenMarket();
        CloseUtenStorage();
    }

    // 조리도구 상점 열기 
    private void ExpandUtenMarket()
    {
        utenMarketScroll.gameObject.SetActive(true);
        isUtenMarketClosed = false;
        UpdatePrice();
    }

    // 조리도구 상점 닫기 
    private void CloseUtenMarket()
    {
        utenMarketScroll.gameObject.SetActive(false);
        isUtenMarketClosed = true;
    }

    // 조리도구 보관함 열기 
    private void ExpandUtenStorage()
    {
        utenStorageScroll.gameObject.SetActive(true);
        isUtenStorageClosed = false;
        UpdateStock();
    }

    // 조리도구 보관함 닫기 
    private void CloseUtenStorage()
    {
        utenStorageScroll.gameObject.SetActive(false);
        isUtenStorageClosed = true;
    }

    // 닫기 버튼 클릭 시 
    public void OnClickClose()
    {
        boughtScreen.gameObject.SetActive(false);
        notEnoughMoneyScreen.gameObject.SetActive(false);
        CloseUtenMarket();
        CloseUtenStorage();
    }

    // 가격 버튼 클릭 시 
    public void OnClickPrice()
    {
        buyOrNotScreen.gameObject.SetActive(true);
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        selectedInteriorItem = clickedButton.transform.parent;
    }

    // 구매하시겠습니까? 창에서 네 버튼 클릭 시 
    public void OnClickYes()
    {
        buyOrNotScreen.gameObject.SetActive(false);
        BuyInterior(selectedInteriorItem);
        if (isBoughtSuccessful)
        {
            boughtScreen.gameObject.SetActive(true);
        }
    }

    // 구매하시겠습니까? 창에서 아니오 버튼 클릭 시 
    public void OnClickNo()
    {
        buyOrNotScreen.gameObject.SetActive(false);
    }
}