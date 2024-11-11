using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
    public ScrollRect sushiMarketScroll;
    public ScrollRect ramenMarketScroll;
    public ScrollRect tempuraMarketScroll;
    public ScrollRect steakMarketScroll;
    public ScrollRect riceMarketScroll;
    public ScrollRect otherMarketScroll;
    private bool isRecipeMarketClosed;
    public GameObject recipeMarket;
    public Image buyOrNotScreen;
    public Image boughtScreen;

    // Start is called before the first frame update
    void Start()
    {
        isRecipeMarketClosed = true;
        buyOrNotScreen.gameObject.SetActive(false);
        boughtScreen.gameObject.SetActive(false);
        CloseRecipeMarket();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickPrice()
    {
        buyOrNotScreen.gameObject.SetActive(true);
    }

    public void OnClickYes()
    {
        buyOrNotScreen.gameObject.SetActive(false);
        boughtScreen.gameObject.SetActive(true);
    }

    public void OnClickNo()
    {
        buyOrNotScreen.gameObject.SetActive(false);
    }

    public void OnClickClose()
    {
        boughtScreen.gameObject.SetActive(false);
        recipeMarket.gameObject.SetActive(false);
    }

    public void OnClickRecipeMarket()
    {
       if (isRecipeMarketClosed)
        {
            OpenRecipeMarket();
            isRecipeMarketClosed = false;
        }
        else
        {
            CloseRecipeMarket();
            isRecipeMarketClosed = true;
        } 
    }

    public void OpenRecipeMarket()
    {
        recipeMarket.SetActive(true);
        OnClickSushi();
    }

    public void CloseRecipeMarket()
    {
        recipeMarket.SetActive(false);
    }
    public void OnClickSushi()
    {
        sushiMarketScroll.gameObject.SetActive(true);
        ramenMarketScroll.gameObject.SetActive(false);
        tempuraMarketScroll.gameObject.SetActive(false);
        steakMarketScroll.gameObject.SetActive(false);
        riceMarketScroll.gameObject.SetActive(false);
        otherMarketScroll.gameObject.SetActive(false);
    }

    public void OnClickRamen()
    {
        sushiMarketScroll.gameObject.SetActive(false);
        ramenMarketScroll.gameObject.SetActive(true);
        tempuraMarketScroll.gameObject.SetActive(false);
        steakMarketScroll.gameObject.SetActive(false);
        riceMarketScroll.gameObject.SetActive(false);
        otherMarketScroll.gameObject.SetActive(false);
    }

    public void OnClickTempura()
    {
        sushiMarketScroll.gameObject.SetActive(false);
        ramenMarketScroll.gameObject.SetActive(false);
        tempuraMarketScroll.gameObject.SetActive(true);
        steakMarketScroll.gameObject.SetActive(false);
        riceMarketScroll.gameObject.SetActive(false);
        otherMarketScroll.gameObject.SetActive(false);
    }

    public void OnClickSteak()
    {
        sushiMarketScroll.gameObject.SetActive(false);
        ramenMarketScroll.gameObject.SetActive(false);
        tempuraMarketScroll.gameObject.SetActive(false);
        steakMarketScroll.gameObject.SetActive(true);
        riceMarketScroll.gameObject.SetActive(false);
        otherMarketScroll.gameObject.SetActive(false);
    }

    public void OnClickRice()
    {
        sushiMarketScroll.gameObject.SetActive(false);
        ramenMarketScroll.gameObject.SetActive(false);
        tempuraMarketScroll.gameObject.SetActive(false);
        steakMarketScroll.gameObject.SetActive(false);
        riceMarketScroll.gameObject.SetActive(true);
        otherMarketScroll.gameObject.SetActive(false);
    }

    public void OnClickOther()
    {
        sushiMarketScroll.gameObject.SetActive(false);
        ramenMarketScroll.gameObject.SetActive(false);
        tempuraMarketScroll.gameObject.SetActive(false);
        steakMarketScroll.gameObject.SetActive(false);
        riceMarketScroll.gameObject.SetActive(false);
        otherMarketScroll.gameObject.SetActive(true);
    }
}