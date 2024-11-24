using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientUI : MonoBehaviour, IBuyable
{
    public Fridge fridge;
    public GameObject fridgeScroll;
    public Image buyOrNotScreen;
    public Image boughtScreen;
    public string ingrident;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 재료 가져오기 버튼 클릭 시 (준희님 냉장고와 합칠 예정) 
    public void OnClickOpenFridge()
    {
        fridgeScroll.gameObject.SetActive(true);
    }

    // 냉장고 닫기 버튼 클릭 시 
    public void OnClickCloseFridge()
    {
        fridgeScroll.gameObject.SetActive(false);
    }

    // 가격 버튼 클릭 시 
    public void OnClickPrice()
    {
        buyOrNotScreen.gameObject.SetActive(true);
    }

    // 닫기 버튼 클릭 시 
    public void OnClickClose()
    {
        boughtScreen.gameObject.SetActive(false);
        fridgeScroll.gameObject.SetActive(false);
    }

    // 구매하시겠습니까? 창에서 네 버튼 클릭 시 
    public void OnClickYes()
    {
        buyOrNotScreen.gameObject.SetActive(false);
        boughtScreen.gameObject.SetActive(true);
        ingrident = fridge.ingredientName;
        fridge.BuyIngredient(ingrident);
        UpdateUI(ingrident);
    }

    // 구매하시겠습니까? 창에서 아니오 버튼 클릭 시 
    public void OnClickNo()
    {
        buyOrNotScreen.gameObject.SetActive(false);
    }

    // 재료 개수 업데이트 
    public void UpdateUI(string ingrident)
    {
        GameObject ingridentObject = GameObject.FindWithTag(ingrident);
        TextMeshProUGUI text = ingridentObject.GetComponent<TextMeshProUGUI>();
        text.text = "x" + fridge.GetIngridentStock(ingrident);
    }

}
