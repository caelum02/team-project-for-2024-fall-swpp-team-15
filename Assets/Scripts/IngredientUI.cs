using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientUI : MonoBehaviour
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

    public void OnClickOpenFridge()
    {
        fridgeScroll.gameObject.SetActive(true);
    }
    public void OnClickCloseFridge()
    {
        fridgeScroll.gameObject.SetActive(false);
    }

    public void OnClickPrice()
    {
        buyOrNotScreen.gameObject.SetActive(true);
    }

    public void OnClickClose()
    {
        boughtScreen.gameObject.SetActive(false);
        fridgeScroll.gameObject.SetActive(false);
    }

    public void OnClickYes()
    {
        buyOrNotScreen.gameObject.SetActive(false);
        boughtScreen.gameObject.SetActive(true);
        ingrident = fridge.ingredientName;
        fridge.BuyIngredient(ingrident);
        UpdateUI(ingrident);
    }

    public void OnClickNo()
    {
        buyOrNotScreen.gameObject.SetActive(false);
    }

    public void UpdateUI(string ingrident)
    {
        GameObject ingridentObject = GameObject.FindWithTag(ingrident);
        TextMeshProUGUI text = ingridentObject.GetComponent<TextMeshProUGUI>();
        text.text = "x" + fridge.GetIngridentStock(ingrident);
    }

}
