using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Yogaewonsil.Common;
using System.Linq;

public class RecipeHelpUIController : MonoBehaviour
{   
    [Header("RecipeTree")]
    private Transform RecipeTree;
    private Transform Format;
    private Transform Ingredients;
    private Transform CookingMethod;
    private Transform TargetFood;
    private TMP_Text cookMethodText;
    private RawImage cookingStationImage;
    private TMP_Text targetFoodText;
    private RawImage targetFoodImage;

    [Header("RecipeList")]
    private Transform RecipeList;
    private Transform RecipeListContent;

    [Header("Prefabs")]
    [SerializeField] private GameObject IngredientButtonPrefab; // 버튼 프리팹

    [Header("Database")]
    [SerializeField] protected FoodDatabaseSO foodDatabase; // 음식 데이터베이스

    [Header("Icons")]
    [SerializeField] private Texture CountertopIcon;       // Countertop 아이콘
    [SerializeField] private Texture SushiCountertopIcon;  // SushiCountertop 아이콘
    [SerializeField] private Texture GasRangePotIcon;      // GasRangePot 아이콘
    [SerializeField] private Texture GasRangeFryPanIcon;   // GasRangeFryPan 아이콘
    [SerializeField] private Texture FryerIcon;           // Fryer 아이콘
    [SerializeField] private Texture PotIcon;             // Pot 아이콘
    [SerializeField] private Texture IngredientIcon;             // Pot 아이콘
    // 사전 매핑
    private Dictionary<CookMethod, Texture> cookingStationIcons;
    
    // 뒤로 가기용 스택
    private Food? currentFood = null;
    private Stack<Food> foodStack;

    // Start is called before the first frame update
    void Awake()
    {   
        // 아이콘 사전 초기화
        cookingStationIcons = new Dictionary<CookMethod, Texture>
        {
            { CookMethod.비가열조리, CountertopIcon },
            { CookMethod.손질, CountertopIcon },
            { CookMethod.초밥제작, SushiCountertopIcon },
            { CookMethod.끓이기, GasRangePotIcon },
            { CookMethod.굽기, GasRangeFryPanIcon },
            { CookMethod.튀기기, FryerIcon },
            { CookMethod.밥짓기, PotIcon }
        };

        RecipeTree = transform.Find("RecipeTree");
        if (RecipeTree == null)
        {
            Debug.LogError($"RecipeTree not found in {gameObject.name}");
            return;
        }

        Format= RecipeTree.Find("Format");
        if (Format == null)
        {
            Debug.LogError($"Format not found in {gameObject.name}");
            return;
        }

        Ingredients = Format.Find("Ingredients");
        if (Ingredients == null)
        {
            Debug.LogError($"Ingredients not found in {gameObject.name}");
            return;
        }

        CookingMethod = Format.Find("CookingMethod");
        if (CookingMethod == null)
        {
            Debug.LogError($"CookingMethod not found in {gameObject.name}");
            return;
        }

        cookMethodText = CookingMethod.GetComponentInChildren<TMP_Text>();
        if (cookMethodText == null)
        {
            Debug.LogError($"cookMethodText not found in {gameObject.name}");
            return;
        }

        cookingStationImage = CookingMethod.Find("CookingStationImage").GetComponent<RawImage>();
        if (cookingStationImage == null)
        {
            Debug.LogError($"cookingStationImage not found in {gameObject.name}");
            return;
        }

        TargetFood = Format.Find("TargetFood");
        if (TargetFood == null)
        {
            Debug.LogError($"TargetFood not found in {gameObject.name}");
            return;
        }

        targetFoodText = TargetFood.GetComponentInChildren<TMP_Text>();
        if (targetFoodText == null)
        {
            Debug.LogError($"targetFoodText not found in {gameObject.name}");
            return;
        }

        targetFoodImage = TargetFood.Find("TargetFoodImage").GetComponent<RawImage>();
        if (targetFoodImage == null)
        {
            Debug.LogError($"targetFoodImage not found in {gameObject.name}");
            return;
        }

        RecipeList = transform.Find("RecipeList");
        if (RecipeList == null)
        {
            Debug.LogError($"RecipeList not found in {gameObject.name}");
            return;
        }

        RecipeListContent = RecipeList.Find("Viewport/Content");
        if (RecipeListContent == null)
        {
            Debug.LogError($"RecipeListContent not found in {gameObject.name}");
            return;
        }

        foodStack = new Stack<Food>();
        
        // To Do
        // 초기화면 설정
        RecipeTree.gameObject.SetActive(false);
        PopulateRecipeList();
    }

    private void ShowRecipe(Food food)
    {   
        Debug.Log("ShowRecipe");
        // RecipeTree 활성화
        RecipeTree.gameObject.SetActive(true);
        RecipeList.gameObject.SetActive(false);

        // 이전 정보 삭제
        RemoveAllChildren(Ingredients);

        // 최종 결과물 표시
        FoodData foodData = foodDatabase.foodData.Find(data => data.food == food);

        targetFoodText.text = food.ToString();
        targetFoodImage.texture = foodData.icon;

        // 레시피 정보 얻어오기
        var recipes = Recipe.GetRecipesForOutput(food);
        if (recipes == null || !recipes.Any())
        {
            Debug.Log("찾으시는 레시피는 존재하지 않습니다.");
            
            cookMethodText.text = "재료";
            cookingStationImage.texture = IngredientIcon;

            return;
        }

        // 첫 번째 레시피를 가져오기
        var firstRecipe = recipes.FirstOrDefault();

        // 튜플의 개별 요소 추출
        CookMethod cookMethod = firstRecipe.mthd;    // 조리 방법
        Food[] inFoods = firstRecipe.inFoods;       // 필요한 재료들

        // 재료 표시
        foreach(Food ingredient in inFoods)
        {
            CreateIngredientButton(ingredient, Ingredients);
        }

        // 조리방법 표시
        cookMethodText.text = cookMethod.ToString();
        cookingStationImage.texture = cookingStationIcons[cookMethod];
    }

    private void CreateIngredientButton(Food ingredient, Transform parent)
    {   
        // 해당 재료의 FoodData 가져오기
        FoodData foodData = foodDatabase.foodData.Find(data => data.food == ingredient);

        // Button Prefab 복제
        GameObject buttonObject = Instantiate(IngredientButtonPrefab, parent);

        buttonObject.transform.SetSiblingIndex(0);

        // TextMeshProUGUI 컴포넌트 설정
        TMP_Text buttonText = buttonObject.GetComponentInChildren<TMP_Text>(); // TextMeshProUGUI를 찾음
        if (buttonText != null)
        {
            buttonText.text = ingredient.ToString(); // 재료 이름 설정
        }
        else
        {
            Debug.LogWarning($"TextMeshProUGUI component not found in Button Prefab for {ingredient}.");
        }
        
        RawImage foodImage = buttonObject.transform.Find("FoodImage").GetComponent<RawImage>();
        if (foodImage != null)
        {
            foodImage.texture = foodData.icon;
        }

        // 버튼 클릭 이벤트 추가
        Button button = buttonObject.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => RememberAndShowRecipe(ingredient));
        }
    }

    void Update()
    {
        
    }

    public void RemoveAllChildren(Transform parentTransform)
    {
        // 자식들을 하나씩 순회하며 삭제
        for (int i = parentTransform.childCount - 1; i >= 0; i--)
        {
            Transform child = parentTransform.GetChild(i); // 자식 Transform 가져오기
            Destroy(child.gameObject); // 자식 GameObject 삭제
        }
    }

    public void RememberAndShowRecipe(Food food)
    {   
        Debug.Log("RememberAndShowRecipe");
        // stack 업데이트
        if (currentFood != null)
        {
            foodStack.Push((Food)currentFood);
            Debug.Log(currentFood.ToString());
        }
        currentFood = food;
        ShowRecipe(food);
    }

    public void ReturnToPreviousRecipe()
    {   
        // 스택 업데이트 하지 않음
        if (foodStack.Count == 0)
        {
            OpenRecipeList();
            return;
        }
        Food lastFood = foodStack.Pop();
        Debug.Log(lastFood.ToString());
        currentFood = lastFood;
        ShowRecipe(lastFood);
    }

    public void PopulateRecipeList()
    {
        // RecipeList 아래 모든 자식을 삭제
        RemoveAllChildren(RecipeListContent);

        // foodDatabase를 순회
        foreach (var foodData in foodDatabase.foodData)
        {
            // tag가 FoodTag.Ingredient가 아닌 경우
            if (foodData.tag != FoodTag.Ingredient && foodData.food != Food.실패요리)
            {
                // RecipeList 아래에 버튼 생성
                CreateIngredientButton(foodData.food, RecipeListContent);
            }
        }
    }

    public void OpenRecipeList()
    {
        foodStack.Clear();
        currentFood = null;
        RecipeList.gameObject.SetActive(true);
        RecipeTree.gameObject.SetActive(false);
    }

    public void OnClickClose()
    {
        foodStack.Clear();
        currentFood = null;
        // RecipeList.gameObject.SetActive(true);
        // RecipeTree.gameObject.SetActive(false);
        gameObject.SetActive(false);

        ResumeGame();
    }

    public void OnClickOpen()
    {
        // 매개변수 없이 호출된 경우의 동작
        gameObject.SetActive(true);
        RecipeList.gameObject.SetActive(true);
        RecipeTree.gameObject.SetActive(false);

        PauseGame(); // 게임 일시정지
    }

    public void OnClickOpen(Food food)
    {
        // Food 매개변수를 받았을 때의 동작
        gameObject.SetActive(true);
        RememberAndShowRecipe(food);// Food를 기반으로 레시피 UI 업데이트

        PauseGame(); // 게임 일시정지
    }

    public void PauseGame()
    {
        Time.timeScale = 0; // 게임 일시정지
        Debug.Log("게임이 일시정지되었습니다.");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1; // 게임 재개
        Debug.Log("게임이 재개되었습니다.");
    }
}
