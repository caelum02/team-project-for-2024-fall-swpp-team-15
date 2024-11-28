using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// 레시피 UI 클래스.
/// 음식 데이터 관리 및 레시피 상점 UI 처리 
/// </summary>
public class RecipeUI : MonoBehaviour, IBuyable
{
    /// <summary>
    /// 음식 데이터베이스 ScriptableObject. 
    /// 모든 음식 데이터 저장 
    /// </summary>
    [SerializeField] private FoodDatabaseSO foodDatabase;

    /// <summary>
    /// 게임의 전반적인 상태와 데이터를 관리하는 GameManager
    /// </summary>
    [SerializeField] private GameManager gameManager;

    [Header("Market Scrolls")]
    /// <summary>
    /// 레시피 상점 초밥 화면 
    /// </summary>
    [SerializeField] private ScrollRect sushiMarketScroll;
    /// <summary>
    /// 레시피 상점 라멘 화면 
    /// </summary>
    [SerializeField] private ScrollRect ramenMarketScroll;
    /// <summary>
    /// 레시피 상점 튀김 화면 
    /// </summary>
    [SerializeField] private ScrollRect tempuraMarketScroll;
    /// <summary>
    /// 레시피 상점 구이 화면 
    /// </summary>
    [SerializeField] private ScrollRect steakMarketScroll;
    /// <summary>
    /// 레시피 상점 밥 화면 
    /// </summary>
    [SerializeField] private ScrollRect riceMarketScroll;
    /// <summary>
    /// 레시피 상점 기타 화면 
    /// </summary>
    [SerializeField] private ScrollRect otherMarketScroll;

    [Header("Market Content")]
    /// <summary>
    /// 초밥 세부 요리 항목 담고 있는 Transform.
    /// RecipeMarket - Canvas - SushiMarketScroll - Viewport - SushiContent. 
    /// 세부 요리 항목 업데이트 시 필요
    /// </summary>
    [SerializeField] private Transform sushiContent;
    /// <summary>
    /// 라멘 세부 요리 항목 담고 있는 Transform
    /// </summary>
    [SerializeField] private Transform ramenContent;
    /// <summary>
    /// 튀김 세부 요리 항목 담고 있는 Transform
    /// </summary>
    [SerializeField] private Transform tempuraContent;
    /// <summary>
    /// 구이 세부 요리 항목 담고 있는 Transform
    /// </summary>
    [SerializeField] private Transform steakContent;
    /// <summary>
    /// 밥 세부 요리 항목 담고 있는 Transform
    /// </summary>
    [SerializeField] private Transform riceContent;
    /// <summary>
    /// 기타 세부 요리 항목 담고 있는 Transform
    /// </summary>
    [SerializeField] private Transform otherContent;
    
    /// <summary>
    /// 레시피 상점이 닫혀 있는지 여부. 
    /// true면 닫혀 있고, false면 열려 있음. 
    /// </summary>
    private bool isRecipeMarketClosed;

    /// <summary>
    /// 전체 레시피 상점 UI의 GameObject
    /// </summary>
    [SerializeField] private GameObject recipeMarket;

    /// <summary>
    /// 구매 확인 창 UI
    /// </summary>
    [SerializeField] private Image buyOrNotScreen;

    /// <summary>
    /// 구매 완료 창 UI
    /// </summary>
    [SerializeField] private Image boughtScreen;
    
    /// <summary>
    /// 현재 선택된 요리의 Transform.
    /// 요리 구매 시 필요 
    /// </summary>
    Transform selectedFoodItem;

    void Start()
    {
        InitializeRecipeUI();
        UpdateAllPriceAndLevel();
    }

    /// <summary>
    /// 레시피 UI 초기화 및 기본 상태로 설정
    /// </summary>
    private void InitializeRecipeUI()
    {
        isRecipeMarketClosed = true;
        buyOrNotScreen.gameObject.SetActive(false);
        boughtScreen.gameObject.SetActive(false);
        CloseRecipeMarket();
    }

    /// <summary>
    /// 모든 레시피 상점의 가격 및 잠금 상태 업데이트
    /// </summary>
    public void UpdateAllPriceAndLevel()
    {
        UpdatePriceAndLevel(sushiContent);
        UpdatePriceAndLevel(ramenContent);
        UpdatePriceAndLevel(tempuraContent);
        UpdatePriceAndLevel(steakContent);
        UpdatePriceAndLevel(riceContent);
        UpdatePriceAndLevel(otherContent);
    }

    /// <summary>
    /// 특정 상점 탭의 세부 요리 항목 (가격, 해금 레벨) 업데이트
    /// </summary>
    /// <param name="marketContent">업데이트할 상점 세부 항목의 Transform</param>
    public void UpdatePriceAndLevel(Transform marketContent)
    {
        foreach (Transform item in marketContent)
        {
            FoodData foodData = GetFoodData(item.name);
            if (foodData != null)
            {
                UpdatePriceAndLevelText(item, foodData);
                foodData.UpdateLockingStatus(false);
                UpdateLockStatus(item, foodData);
            }
            else
            {
                Debug.LogWarning($"No FoodData found for {item.name} in the FoodDatabase.");
            }
        }
    }

    /// <summary>
    /// 항목 이름으로 FoodData를 가져오기
    /// </summary>
    /// <param name="itemName">검색할 항목 이름</param>
    /// <returns> 음식 데이터베이스에서 해당 FoodData 객체를 반환</returns>
    private FoodData GetFoodData(string itemName)
    {
        return foodDatabase.foodData.Find(food => food.name == itemName);
    }

    /// <summary>
    /// 항목의 가격 및 레벨 '텍스트' 업데이트
    /// </summary>
    /// <param name="item">업데이트할 항목</param>
    /// <param name="foodData">음식 데이터베이스에서 가져온 해당 FoodData 객체</param>
    private void UpdatePriceAndLevelText(Transform item, FoodData foodData)
    {
        Transform priceObject = item.Find("Price");
        Transform levelObject = item.Find("Level");
        
        // 가격 텍스트 업데이트 
        if (priceObject != null) 
        {
            TextMeshProUGUI priceText = priceObject.GetComponentInChildren<TextMeshProUGUI>();
            if (priceText != null)
            {
                priceText.text = "   " + foodData.price.ToString();
            }
        }

        // 레벨 텍스트 업데이트 
        if (levelObject != null)
        {
            TextMeshProUGUI levelText = levelObject.GetComponent<TextMeshProUGUI>();
            if (levelText != null)
            {
                levelText.text = "Level " + foodData.level.ToString();
            }
        }
    }

    /// <summary>
    /// 항목의 잠금 상태 업데이트
    /// </summary>
    /// <param name="item">업데이트할 항목</param>
    /// <param name="foodData">음식 데이터베이스에서 가져온 해당 FoodData 객체</param>
    private void UpdateLockStatus(Transform item, FoodData foodData)
    {
        // Find the lock, key, and level objects
        Transform lockObject = item.Find("Lock");
        Transform keyObject = item.Find("Key");
        Transform levelObject = item.Find("Level");

        bool isUnlocked = gameManager.reputation >= foodData.level || foodData.isUnlocked;

        // Update the FoodData locking status if it has changed
        if (foodData.isUnlocked != isUnlocked)
        {
            foodData.UpdateLockingStatus(isUnlocked);
        }
        
        if (lockObject != null) lockObject.gameObject.SetActive(!isUnlocked);
        if (keyObject != null) keyObject.gameObject.SetActive(!isUnlocked);
        if (levelObject != null) levelObject.gameObject.SetActive(!isUnlocked);
    }

    /// <summary>
    /// 가격 버튼 클릭 시 호출되어 구매 확인 창 표시
    /// </summary>
    public void OnClickPrice()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        selectedFoodItem = clickedButton.transform.parent;
        buyOrNotScreen.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 구매 확인 창에서 "네" 클릭 시 호출
    /// </summary>
    public void OnClickYes()
    {
        BuyDish(selectedFoodItem);
        buyOrNotScreen.gameObject.SetActive(false);
        boughtScreen.gameObject.SetActive(true);
    }

    /// <summary>
    /// 지정된 항목 구매
    /// </summary>
    /// <param name="foodItem">구매할 항목의 Transform</param>
    public void BuyDish(Transform foodItem)
    {
        FoodData foodData = GetFoodData(foodItem.name);
        if (foodData != null)
        {
            foodData.UpdateBuyingStatus(true);
            foodItem.Find("Coin").gameObject.SetActive(false);
            foodItem.Find("Price").gameObject.SetActive(false);
            Debug.Log($"Dish '{foodData.name}' bought successfully.");
        }
    }

    /// <summary>
    /// 구매 확인 창에서 "아니오"를 클릭 시 호출
    /// </summary>
    public void OnClickNo()
    {
        buyOrNotScreen.gameObject.SetActive(false);
    }

    /// <summary>
    /// 구매 완료 및 확인 창 닫기
    /// </summary>
    public void OnClickClose()
    {
        boughtScreen.gameObject.SetActive(false);
        recipeMarket.gameObject.SetActive(false);
    }

    //// <summary>
    /// 레시피 버튼 클릭 시 호출되어 상점의 열림/닫힘 상태 변경 
    /// </summary>
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

    /// <summary>
    /// 레시피 상점 열기
    /// </summary>
    public void OpenRecipeMarket()
    {
        recipeMarket.SetActive(true);
        OnClickSushi();
    }

    /// <summary>
    /// 레시피 상점 닫기 
    /// </summary>
    public void CloseRecipeMarket()
    {
        recipeMarket.SetActive(false);
    }

    /// <summary>
    /// 주어진 탭 활성화, 나머지 탭 비활성화.
    /// </summary>
    /// <param name="selectedTab">활성화할 탭의 GameObject</param>
    private void ActivateTab(GameObject selectedTab)
    {
        SetActiveTab(sushiMarketScroll, selectedTab);
        SetActiveTab(ramenMarketScroll, selectedTab);
        SetActiveTab(tempuraMarketScroll, selectedTab);
        SetActiveTab(steakMarketScroll, selectedTab);
        SetActiveTab(riceMarketScroll, selectedTab);
        SetActiveTab(otherMarketScroll, selectedTab);
    }

    /// <summary>
    /// 특정 ScrollRect가 선택된 탭인지 확인하여 활성화 또는 비활성화함
    /// </summary>
    /// <param name="scrollRect">활성화 여부를 결정할 ScrollRect</param>
    /// <param name="selectedTab">활성화되는 탭의 GameObject</param>
    private void SetActiveTab(ScrollRect scrollRect, GameObject selectedTab)
    {
        scrollRect.gameObject.SetActive(scrollRect.gameObject == selectedTab);
    }

    /// <summary>
    /// 초밥 탭 클릭 시
    /// </summary>
    public void OnClickSushi() => ActivateTab(sushiMarketScroll.gameObject);

    /// <summary>
    /// 라멘 탭 클릭 시
    /// </summary>
    public void OnClickRamen() => ActivateTab(ramenMarketScroll.gameObject);

    /// <summary>
    /// 튀김 탭 클릭 시
    /// </summary>
    public void OnClickTempura() => ActivateTab(tempuraMarketScroll.gameObject);

    /// <summary>
    /// 구이 탭 클릭 시
    /// </summary>
    public void OnClickSteak() => ActivateTab(steakMarketScroll.gameObject);

    /// <summary>
    /// 밥 탭 클릭 시
    /// </summary>
    public void OnClickRice() => ActivateTab(riceMarketScroll.gameObject);

    /// <summary>
    /// 기타 탭 클릭 시
    /// </summary>
    public void OnClickOther() => ActivateTab(otherMarketScroll.gameObject);

}