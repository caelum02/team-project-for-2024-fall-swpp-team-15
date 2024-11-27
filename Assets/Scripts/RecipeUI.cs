using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour, IBuyable
{
    [SerializeField] private FoodDatabaseSO foodDatabase;
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

    // 닫기 버튼 클릭 시 
    public void OnClickClose()
    {
        boughtScreen.gameObject.SetActive(false);
        recipeMarket.gameObject.SetActive(false);
    }

    // 레시피 버튼 클릭 시 
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

    // 레시피 상점 열기 
    public void OpenRecipeMarket()
    {
        recipeMarket.SetActive(true);
        OnClickSushi();
    }

    // 레시피 상점 닫기 
    public void CloseRecipeMarket()
    {
        recipeMarket.SetActive(false);
    }

    // 초밥 탭 클릭 시 
    public void OnClickSushi()
    {
        sushiMarketScroll.gameObject.SetActive(true);
        ramenMarketScroll.gameObject.SetActive(false);
        tempuraMarketScroll.gameObject.SetActive(false);
        steakMarketScroll.gameObject.SetActive(false);
        riceMarketScroll.gameObject.SetActive(false);
        otherMarketScroll.gameObject.SetActive(false);
    }

    // 라멘 탭 클릭 시 
    public void OnClickRamen()
    {
        sushiMarketScroll.gameObject.SetActive(false);
        ramenMarketScroll.gameObject.SetActive(true);
        tempuraMarketScroll.gameObject.SetActive(false);
        steakMarketScroll.gameObject.SetActive(false);
        riceMarketScroll.gameObject.SetActive(false);
        otherMarketScroll.gameObject.SetActive(false);
    }

    // 튀김 탭 클릭 시 
    public void OnClickTempura()
    {
        sushiMarketScroll.gameObject.SetActive(false);
        ramenMarketScroll.gameObject.SetActive(false);
        tempuraMarketScroll.gameObject.SetActive(true);
        steakMarketScroll.gameObject.SetActive(false);
        riceMarketScroll.gameObject.SetActive(false);
        otherMarketScroll.gameObject.SetActive(false);
    }

    // 구이 탭 클릭 시 
    public void OnClickSteak()
    {
        sushiMarketScroll.gameObject.SetActive(false);
        ramenMarketScroll.gameObject.SetActive(false);
        tempuraMarketScroll.gameObject.SetActive(false);
        steakMarketScroll.gameObject.SetActive(true);
        riceMarketScroll.gameObject.SetActive(false);
        otherMarketScroll.gameObject.SetActive(false);
    }

    // 밥 탭 클릭 시 
    public void OnClickRice()
    {
        sushiMarketScroll.gameObject.SetActive(false);
        ramenMarketScroll.gameObject.SetActive(false);
        tempuraMarketScroll.gameObject.SetActive(false);
        steakMarketScroll.gameObject.SetActive(false);
        riceMarketScroll.gameObject.SetActive(true);
        otherMarketScroll.gameObject.SetActive(false);
    }

    // 기타 탭 클릭 시 
    public void OnClickOther()
    {
        sushiMarketScroll.gameObject.SetActive(false);
        ramenMarketScroll.gameObject.SetActive(false);
        tempuraMarketScroll.gameObject.SetActive(false);
        steakMarketScroll.gameObject.SetActive(false);
        riceMarketScroll.gameObject.SetActive(false);
        otherMarketScroll.gameObject.SetActive(true);
    }

    public void LockDish()
    {

    }
}