using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;
using System.Collections; // 코루틴에서 필요한 네임스페이스
using Yogaewonsil.Common;
using TMPro; // TextMeshPro 사용을 위한 네임스페이스

/// <summary>
/// 재료 추가 및 제거 기능이 있는 주방 기구 기본 클래스입니다.
/// </summary>
public abstract class KitchenInteriorWithAddandRemove : KitchenInteriorBase
{   
    protected Button addButton; // 재료를 추가하는 버튼
    protected Button removeButton; // 재료를 제거하는 버튼
    protected Transform selectionPanel; // 재료 선택 패널
    protected Button backButton; // 선택 패널을 닫는 버튼

    protected Transform visualMenu; // 시각적 메뉴
    protected Transform iconPanel; // 재료 아이콘 패널

    public List<Food> ingredients = new List<Food>(); // 현재 재료 목록

    [Header("Prefabs")]
    [SerializeField] private GameObject buttonPrefab; // 버튼 프리팹
    [SerializeField] private GameObject framePrefab; // 프레임 프리팹

    /// <summary>
    /// 초기화 메서드. 버튼과 패널을 설정합니다.
    /// </summary>
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

        // VisualMenu 찾기
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
        removeButton.onClick.AddListener(ShowSelectionPanel); // RemoveButton의 onClick 이벤트에 ShowSelectionPanel 함수 연결

        // BackButton 찾기
        backButton = selectionPanel.Find("BackButton")?.GetComponent<Button>();
        if (backButton == null)
        {
            Debug.LogError($"BackButton not found in InteractionPanel of {gameObject.name}");
            return;
        }
        backButton.onClick.AddListener(HideSelectionPanel); // BackButton의 onClick 이벤트에 HideSelectionPanel 함수 연결

         // 초기 상태 설정
        selectionPanel.gameObject.SetActive(false);  // selectionPanel은 어떤 재료를 빼낼지 선택할 때 활성화

        visualMenu.gameObject.SetActive(true); // visualMenu를 처음부터 활성화 해둬야 조리기구 위 아이콘이 보임
        iconPanel.gameObject.SetActive(true); // IconPanel도 초기에 활성화
    }

    /// <summary>
    /// 버튼 활성화 상태를 업데이트합니다.
    /// </summary>
    protected override void UpdateAllButtons()  // private일지 protected일지 고려 -> 조리대에서 버튼 하나 추가되면 바뀔 수 있을 듯
    {
        Food? heldFood = PlayerController.Instance.GetHeldFood();

        // AddButton은 플레이어가 재료를 들고 있고 재료가 4개 미만일 때 활성화
        addButton.interactable = heldFood != null && ingredients.Count < 4;
        
        // RemoveButton은 재료가 있고 플레이어가 아무것도 들고 있지 않을 때 활성화
        removeButton.interactable = ingredients.Count > 0 && heldFood == null;
    }

    /// <summary>
    /// 메뉴를 숨길 때 선택 패널을 닫습니다.
    /// </summary>
    protected override void HideMenu()
    {
        if (selectionPanel == null) return;

        base.HideMenu();

        selectionPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 플레이어가 들고 있는 재료를 추가합니다.
    /// </summary>
    public void AddIngredient()
    { 
        Debug.Log("AddIngredient!");
        if (PlayerController.Instance != null && PlayerController.Instance.heldFood != null)
        {
            ingredients.Add((Food)PlayerController.Instance.heldFood); // 재료 추가
            PlayerController.Instance.DropFood(); // 플레이어가 들고 있는 재료 제거
            Debug.Log("Ingredients in the cooking station:");
            foreach (var ingredient in ingredients)
            {
                Debug.Log($"- {ingredient}");
            }
        }

        UpdateAllButtons();
        UpdateIngredientIcons();
    }

    /// <summary>
    /// 지정된 재료를 제거합니다.
    /// </summary>
    /// <param name="ingredient">제거할 재료</param>
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

    /// <summary>
    /// 재료 선택 패널을 표시합니다. (어떤걸 뺄지 선택)
    /// </summary>
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

    /// <summary>
    /// 재료 선택 패널을 숨깁니다.
    /// </summary>
    private void HideSelectionPanel()
    {
        // SelectionPanel 비활성화, InteractionPanel 활성화
        interactionPanel.gameObject.SetActive(true);
        selectionPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 재료를 선택할 수 있는 버튼을 생성합니다.
    /// </summary>
    /// <param name="ingredient">버튼에 표시할 재료</param>
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

    /// <summary>
    /// 재료 아이콘을 업데이트합니다.
    /// </summary>
    protected void UpdateIngredientIcons()
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

    /// <summary>
    /// 재료 아이콘을 생성합니다.
    /// </summary>
    /// <param name="ingredient">아이콘을 생성할 재료</param>
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

    /// <summary>
    /// Texture를 Sprite로 변환합니다.
    /// </summary>
    /// <param name="texture">변환할 Texture</param>
    /// <returns>변환된 Sprite</returns>
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
