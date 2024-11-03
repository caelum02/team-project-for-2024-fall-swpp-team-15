using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        isInteriorClosed = true;
        isUtenMarketClosed = true;
        isUtenStorageClosed = true;
        CloseInteriorMenu();
        utenMarketScroll.gameObject.SetActive(false); 
        utenStorageScroll.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    public void OnClickUtenMarket()
    {
       if (isUtenMarketClosed)
        {
            ExpandUtenMarket();
            isUtenMarketClosed = false;
        }
        else
        {
            CloseUtenMarket();
            isUtenMarketClosed = true;
        } 
    }

    public void OnClickUtenStorage()
    {
       if (isUtenStorageClosed)
        {
            ExpandUtenStorage();
            isUtenStorageClosed = false;
        }
        else
        {
            CloseUtenStorage();
            isUtenStorageClosed = true;
        }  
    }

    private void ExpandInteriorMenu()
    {
        npcButton.gameObject.SetActive(false);
        recipeButton.gameObject.SetActive(false);
        tileButton.gameObject.SetActive(true);
        utenStorageButton.gameObject.SetActive(true);
        utenPositionButton.gameObject.SetActive(true);
        utenMarketButton.gameObject.SetActive(true);
    }

    private void CloseInteriorMenu()
    {
        npcButton.gameObject.SetActive(true);
        recipeButton.gameObject.SetActive(true);
        tileButton.gameObject.SetActive(false);
        utenStorageButton.gameObject.SetActive(false);
        utenPositionButton.gameObject.SetActive(false);
        utenMarketButton.gameObject.SetActive(false);
    }

    private void ExpandUtenMarket()
    {
        utenMarketScroll.gameObject.SetActive(true);
    }

    private void CloseUtenMarket()
    {
        utenMarketScroll.gameObject.SetActive(false);
    }

    private void ExpandUtenStorage()
    {
        utenStorageScroll.gameObject.SetActive(true);
    }

    private void CloseUtenStorage()
    {
        utenStorageScroll.gameObject.SetActive(false);
    }
}
