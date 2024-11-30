using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fridge : MonoBehaviour
{
    /*
    public IngredientUI ingredientUI;
    public Dictionary<string, Ingredient> ingredients = new Dictionary<string, Ingredient>();
    public string ingredientName;
    int money = 2000;
    // Start is called before the first frame update

    
    void Start()
    {
        ingredients.Add("rice", new Ingredient(0, 0));
        ingredients.Add("cookedRice", new Ingredient(0, 0));
        ingredients.Add("egg", new Ingredient(0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClickBuyRice()
    {
        ingredientName = "rice";
    }

    public void OnClickBuyCookedRice()
    {
        ingredientName = "cookedRice";
    }

    public void OnClickBuyEgg()
    {
        ingredientName = "egg";
    }

    public void BuyIngredient(string ingredientName)
    {
        if (ingredients.ContainsKey(ingredientName))
        {
            int price = ingredients[ingredientName].price;
            if (money >= price)
            {
                money -= price;
                ingredients[ingredientName].stock += 1;
            }
            else
            {
                Debug.Log("Not enough money to buy " + ingredientName);
            }
        }
        else
        {
            Debug.LogError("Ingredient not found: " + ingredientName);
        }
    }

    public int GetIngridentStock(string ingredientName)
    {
        if (ingredients.ContainsKey(ingredientName))
        {
            return ingredients[ingredientName].stock;
        }
        else
        {
            Debug.LogError("Ingredient not found: " + ingredientName);
            return 0;
        }
    }
    */

}
