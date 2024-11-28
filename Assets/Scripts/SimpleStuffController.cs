using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;
using System.Collections; // 코루틴에서 필요한 네임스페이스
using Yogaewonsil.Common;
using TMPro; // TextMeshPro 사용을 위한 네임스페이스

public  class SimpleStuffController : KitchenInteriorBase
{
    protected Button addButton; // AddButton (cookingStationCanvas의 자식)
    protected Button removeButton; // RemoveButton (cookingStationCanvas의 자식)
    protected Transform selectionPanel;
    protected Button backButton;

    protected Transform visualMenu;
    protected Transform iconPanel;

    public List<Food> ingredients = new List<Food>();

    [Header("Prefabs")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject framePrefab;

    protected virtual void Start()
    {   
        base.Start();
        // SelectionPanel 찾기
        selectionPanel = interactionMenu.transform.Find("SelectionPanel");
        if (selectionPanel == null)
        {
            Debug.LogError($"SelectionPanel not found in {cookingStationCanvas.name}");
            return;
        }

        // 자식 객체에서 IconCanvas를 찾음
        visualMenu = cookingStationCanvas.transform.Find("VisualMenu");
        if (visualMenu == null)
        {
            Debug.LogError($"VisualMenu not found in {cookingStationCanvas.name}");
            return;
        }

        // IconPanel 찾기
        iconPanel = visualMenu.transform.Find("IconPanel");
        if (iconPanel == null)
        {
            Debug.LogError($"iconPanel not found in {visualMenu.name}");
            return;
        }
        // AddButton 찾기
        addButton = interactionPanel.Find("AddButton")?.GetComponent<Button>();
        if (addButton == null)
        {
            Debug.LogError($"AddButton not found in InteractionPanel of {gameObject.name}");
            return;
        }
        addButton.onClick.AddListener(AddIngredient); // AddButton의 onClick 이벤트에 AddIngredient 함수 연결


        // RemoveButton 찾기
        removeButton = interactionPanel.Find("RemoveButton")?.GetComponent<Button>();
        if (removeButton == null)
        {
            Debug.LogError($"RemoveButton not found in InteractionPanel of {gameObject.name}");
            return;
        }
        removeButton.onClick.AddListener(ShowSelectionPanel);

        // BackButton 찾기
        backButton = selectionPanel.Find("BackButton")?.GetComponent<Button>();
        if (backButton == null)
        {
            Debug.LogError($"BackButton not found in InteractionPanel of {gameObject.name}");
            return;
        }
        backButton.onClick.AddListener(HideSelectionPanel); // BackButton의 onClick 이벤트에 HideSelectionPanel함수 연결

        selectionPanel.gameObject.SetActive(false);

        // IconPanel은 언제나 활성화
        visualMenu.gameObject.SetActive(true);
        iconPanel.gameObject.SetActive(true);
    }

    protected override void UpdateAllButtons()  // private일지 protected일지 고려 -> 조리대에서 버튼 하나 추가되면 바뀔 수 있을 듯
    {
        Food? heldFood = PlayerController.Instance.GetHeldFood();

        // 버튼 활성화/비활성화 상태 업데이트
        addButton.interactable = heldFood != null && ingredients.Count < 4;        // AddButton: 음식이 있으면 활성화/ CookButton: 재료가 있으면 활성화
        removeButton.interactable = ingredients.Count > 0 && heldFood == null;     // RemoveButton: 재료가 있으면 활성화
    }

    public void AddIngredient()
    { 
        Debug.Log("AddIngredient!");
        if (PlayerController.Instance != null && PlayerController.Instance.heldFood != null)
        {
            ingredients.Add((Food)PlayerController.Instance.heldFood); // 재료 추가
            PlayerController.Instance.DropFood(); // 플레이어가 들고 있는 재료 내려놓기
            Debug.Log("Ingredients in the cooking station:");
            foreach (var ingredient in ingredients)
            {
                Debug.Log($"- {ingredient}");
            }
        }

        UpdateAllButtons();
        UpdateIngredientIcons();
    }
    public virtual void RemoveIngredient(Food ingredient)
    {
        // 조리도구에서 재료 제거
        if (ingredients.Contains(ingredient))
        {
            // PlayerController에 재료 추가
            if(PlayerController.Instance != null)
            {
                if (PlayerController.Instance.PickUpFood(ingredient)){
                    Debug.Log($"Player picked up: {ingredient}"); // 플레이어가 재료를 집음
                    ingredients.Remove(ingredient);
                    Debug.Log($"Removed ingredient: {ingredient}");
                } 
                else {
                    Debug.Log($"Player already hold: {PlayerController.Instance.GetHeldFood()}"); // 플레이어가 이미 집고 있는 재료가 있음
                }
            }
            else
            {
                Debug.LogWarning("PlayerController instance not found!");
            }
        }

        // SelectionPanel 숨김
        HideSelectionPanel();

        UpdateAllButtons();
        UpdateIngredientIcons();
    }

    private void ShowSelectionPanel()
    {
        // InteractionPanel 비활성화
        interactionPanel.gameObject.SetActive(false);

        // SelectionPanel 활성화
        selectionPanel.gameObject.SetActive(true);

        // 기존 버튼 삭제 (BackButton 제외)
        foreach (Transform child in selectionPanel)
        {
            if (child.name != "BackButton")
            {
                Destroy(child.gameObject);
            }
        }

        // 재료 버튼 생성
        foreach (var ingredient in ingredients)
        {
            CreateIngredientButton(ingredient);
        }
    }

    private void HideSelectionPanel()
    {
        // SelectionPanel 비활성화, InteractionPanel 활성화
        interactionPanel.gameObject.SetActive(true);
        selectionPanel.gameObject.SetActive(false);
    }

    private void CreateIngredientButton(Food ingredient)
    {
        // Button Prefab 복제
        GameObject buttonObject = Instantiate(buttonPrefab, selectionPanel);

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

        // 버튼 클릭 이벤트 추가
        Button button = buttonObject.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => RemoveIngredient(ingredient));
        }
    }

    private void UpdateIngredientIcons()
    {
        // 기존 아이콘 제거
        foreach (Transform child in iconPanel)
        {
            Destroy(child.gameObject);
        }

        // 재료별로 Frame과 Icon 생성
        foreach (Food ingredient in ingredients)
        {
            CreateIngredientIcon(ingredient); // Database에 icon 채워지면 적용
            //CreateIngredientId(ingredient);
        }
    }

    private void CreateIngredientIcon(Food ingredient)
    {
        // Frame Prefab 복제
        GameObject frameObject = Instantiate(framePrefab, iconPanel);

        // Frame 내부에 Icon 생성
        Transform iconTransform = frameObject.transform.Find("Icon");
        if (iconTransform != null)
        {
            Image iconImage = iconTransform.GetComponent<Image>();
            if (iconImage != null)
            {
                // FoodDatabase에서 해당 재료의 아이콘 정보 가져오기
                FoodData foodData = foodDatabase.foodData.Find(data => data.food == ingredient);
                if (foodData != null && foodData.icon != null)
                {
                    // Texture를 Sprite로 변환
                    Sprite iconSprite = ConvertTextureToSprite(foodData.icon);
                    if (iconSprite != null)
                    {
                        iconImage.sprite = iconSprite;
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to convert texture to sprite for ingredient {ingredient}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Icon not found for ingredient {ingredient}");
                }
            }
            else
            {
                Debug.LogWarning("Icon Image component not found in Frame Prefab.");
            }
        }
        else
        {
            Debug.LogWarning("Icon Transform not found in Frame Prefab.");
        }

        // Frame 이름 설정 (디버그용)
        frameObject.name = $"Frame_{ingredient}";
    }

    private Sprite ConvertTextureToSprite(Texture texture)
    {
        if (texture is Texture2D texture2D)
        {
            // Texture2D를 Sprite로 변환
            return Sprite.Create(
                texture2D,
                new Rect(0, 0, texture2D.width, texture2D.height),
                new Vector2(0.5f, 0.5f)
            );
        }
        return null;
    }
}
