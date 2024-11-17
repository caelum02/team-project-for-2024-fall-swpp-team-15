using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteriorUI : MonoBehaviour
{
    public Button interiorButton;
    public Button tileButton;
    public Button utenStorageButton;
    public Button utenPositionButton;
    public Button utenMarketButton;
    public Button npcButton;
    public Button recipeButton;
    public ScrollRect utenMarketScroll;
    public ScrollRect utenStorageScroll;
    private bool isInteriorClosed;
    private bool isUtenMarketClosed;
    private bool isUtenStorageClosed;
    public Image buyOrNotScreen;
    public Image boughtScreen;

    // Start is called before the first frame update
    void Start()
    {
        isInteriorClosed = true;
        isUtenMarketClosed = true;
        isUtenStorageClosed = true;
        CloseInteriorMenu();
        utenMarketScroll.gameObject.SetActive(false); 
        utenStorageScroll.gameObject.SetActive(false);
        buyOrNotScreen.gameObject.SetActive(false);
        boughtScreen.gameObject.SetActive(false);
        MakeInteriorButtonInvisible();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //정비 시간에 인테리어 버튼 활성화
    public void MakeInteriorButtonVisible()
    {
        interiorButton.gameObject.SetActive(true);
    }

    //영업 시간에 인테리어 버튼 비활성화 
    public void MakeInteriorButtonInvisible()
    {
        interiorButton.gameObject.SetActive(false);
        CloseUtenMarket();
        CloseUtenStorage();
        CloseInteriorMenu();
    }

    //인테리어 버튼 클릭 시
    public void OnClickInteriorMenu()
    {
        if (isInteriorClosed)
        {
            ExpandInteriorMenu();
            isInteriorClosed = false;
        }
        else
        {
            CloseInteriorMenu();
            isInteriorClosed = true;
        }
    }

    //조리도구 상점 버튼 클릭 시
    public void OnClickUtenMarket()
    {
       if (isUtenMarketClosed)
        {
            CloseUtenStorage();
            ExpandUtenMarket();
            isUtenMarketClosed = false;
        }
        else
        {
            CloseUtenMarket();
            isUtenMarketClosed = true;
        } 
    }

    //조리도구 보관함 버튼 클릭 시 
    public void OnClickUtenStorage()
    {
       if (isUtenStorageClosed)
        {
            CloseUtenMarket();
            ExpandUtenStorage();
            isUtenStorageClosed = false;
        }
        else
        {
            CloseUtenStorage();
            isUtenStorageClosed = true;
        }  
    }

    //인테리어 전체 메뉴 열기  
    private void ExpandInteriorMenu()
    {
        tileButton.gameObject.SetActive(true);
        utenStorageButton.gameObject.SetActive(true);
        utenPositionButton.gameObject.SetActive(true);
        utenMarketButton.gameObject.SetActive(true);
    }

    //인테리어 전체 메뉴 닫기
    private void CloseInteriorMenu()
    {
        tileButton.gameObject.SetActive(false);
        utenStorageButton.gameObject.SetActive(false);
        utenPositionButton.gameObject.SetActive(false);
        utenMarketButton.gameObject.SetActive(false);
        CloseUtenMarket();
        CloseUtenStorage();
    }

    //조리도구 상점 열기 
    private void ExpandUtenMarket()
    {
        utenMarketScroll.gameObject.SetActive(true);
    }

    //조리도구 상점 닫기 
    private void CloseUtenMarket()
    {
        utenMarketScroll.gameObject.SetActive(false);
    }

    //조리도구 보관함 열기 
    private void ExpandUtenStorage()
    {
        utenStorageScroll.gameObject.SetActive(true);
    }

    //조리도구 보관함 닫기 
    private void CloseUtenStorage()
    {
        utenStorageScroll.gameObject.SetActive(false);
    }

    //닫기 버튼 클릭 시 
    public void OnClickClose()
    {
        boughtScreen.gameObject.SetActive(false);
        utenMarketScroll.gameObject.SetActive(false);
    }

    //가격 버튼 클릭 시 
    public void OnClickPrice()
    {
        buyOrNotScreen.gameObject.SetActive(true);
    }

    //구매하시겠습니까? 창에서 네 버튼 클릭 시 
    public void OnClickYes()
    {
        buyOrNotScreen.gameObject.SetActive(false);
        boughtScreen.gameObject.SetActive(true);
    }

    //구매하시겠습니까? 창에서 아니오 버튼 클릭 시 
    public void OnClickNo()
    {
        buyOrNotScreen.gameObject.SetActive(false);
    }
}
