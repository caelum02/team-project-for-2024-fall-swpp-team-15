using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteriorUI : MonoBehaviour, IBuyable
{
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

    private bool isInteriorClosed = true;
    private bool isUtenMarketClosed = true;
    private bool isUtenStorageClosed = true;

    // Start is called before the first frame update
    void Start()
    {
        MakeInteriorButtonInvisible();
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
        CloseUtenMarket();
        CloseUtenStorage();
    }

    // 가격 버튼 클릭 시 
    public void OnClickPrice()
    {
        buyOrNotScreen.gameObject.SetActive(true);
    }

    // 구매하시겠습니까? 창에서 네 버튼 클릭 시 
    public void OnClickYes()
    {
        buyOrNotScreen.gameObject.SetActive(false);
        boughtScreen.gameObject.SetActive(true);
    }

    // 구매하시겠습니까? 창에서 아니오 버튼 클릭 시 
    public void OnClickNo()
    {
        buyOrNotScreen.gameObject.SetActive(false);
    }
}