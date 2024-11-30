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

    /// <summary>
    /// 재료 가져오기 버튼 클릭 시
    /// </summary>
    public void OnClickOpenFridge()
    {
        fridgeScroll.gameObject.SetActive(true);
    }

    /// <summary>
    /// 냉장고 닫기 버튼 클릭 시 
    /// </summary>
    public void OnClickCloseFridge()
    {
        fridgeScroll.gameObject.SetActive(false);
    }

    /// <summary>
    /// 가격 버튼 클릭 시 
    /// </summary>
    public void OnClickPrice()
    {
        buyOrNotScreen.gameObject.SetActive(true);
    }

    /// <summary>
    /// 닫기 버튼 클릭 시 
    /// </summary>
    public void OnClickClose()
    {
        boughtScreen.gameObject.SetActive(false);
        fridgeScroll.gameObject.SetActive(false);
    }

    /// <summary>
    /// 구매하시겠습니까? 창에서 네 버튼 클릭 시 
    /// </summary>
    public void OnClickYes()
    {
        buyOrNotScreen.gameObject.SetActive(false);
        boughtScreen.gameObject.SetActive(true);
    }

    /// <summary>
    /// 구매하시겠습니까? 창에서 아니오 버튼 클릭 시
    /// </summary>
    public void OnClickNo()
    {
        buyOrNotScreen.gameObject.SetActive(false);
    }
}
