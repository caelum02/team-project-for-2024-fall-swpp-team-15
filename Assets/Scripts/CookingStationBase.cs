using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;
using System.Collections; // 코루틴에서 필요한 네임스페이스
using Yogaewonsil.Common;
using TMPro; // TextMeshPro 사용을 위한 네임스페이스

public abstract class CookingStationBase : MonoBehaviour
{

    [Header("Station Setting")]
    public string stationName; // 조리기구 이름
    public CookMethod cookingMethod; // 조리 방법

    [Header("UI References")]
    protected Canvas interactionCanvas; // 상호작용 Canvas (자식 객체로 설정)
    protected Transform interactionPanel;
    protected Button addButton; // AddButton (InteractionCanvas의 자식)
    protected Button cookButton; // RemoveButton (InteractionCanvas의 자식)
    protected Button removeButton; // RemoveButton (InteractionCanvas의 자식)
    protected Transform selectionPanel;
    protected Button backButton;

    protected Canvas iconCanvas; // 아이콘 표시용 Canvas (자식 객체로 설정)
    protected Transform iconPanel;

    [Header("Station State")]
    public List<Food> ingredients = new List<Food>();
    protected bool isCooking = false;

    [Header("Animator")]
    protected Animator animator;

    [Header("Prefabs")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject framePrefab; // Frame Prefab을 에디터에서 연결

    [Header("activeStation")] // 결과물 생성 위치
    public static CookingStationBase activeStation; // 현재 활성화된 조리기구

    [Header("Else")]
    public Transform dishSpawnPoint;

    protected virtual void Start()
    { 
        // Animator 자동 연결
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on this GameObject or its children!");
        }

        
        interactionCanvas = transform.Find("InteractionCanvas").GetComponent<Canvas>();
        if (interactionCanvas == null)
        {
            Debug.LogError($"InteractionCanvas not found in {gameObject.name}");
            return;
        }

        // InteractionPanel 찾기
        interactionPanel = interactionCanvas.transform.Find("InteractionPanel");
        if (interactionPanel == null)
        {
            Debug.LogError($"InteractionPanel not found in {interactionCanvas.name}");
            return;
        }

        // SelectionPanel 찾기
        selectionPanel = interactionCanvas.transform.Find("SelectionPanel");
        if (selectionPanel == null)
        {
            Debug.LogError($"SelectionPanel not found in {interactionCanvas.name}");
            return;
        }

        // 자식 객체에서 IconCanvas를 찾음
        iconCanvas = transform.Find("IconCanvas").GetComponent<Canvas>();
        if (iconCanvas == null)
        {
            Debug.LogError($"IconCanvas not found in {gameObject.name}");
            return;
        }

        // IconPanel 찾기
        iconPanel = iconCanvas.transform.Find("IconPanel");
        if (iconPanel == null)
        {
            Debug.LogError($"SelectionPanel not found in {interactionCanvas.name}");
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

        // CookButton 찾기
        cookButton = interactionPanel.Find("CookButton")?.GetComponent<Button>();
        if (cookButton == null)
        {
            Debug.LogError($"CookButton not found in InteractionPanel of {gameObject.name}");
            return;
        }
        cookButton.onClick.AddListener(StartCook); // CookButton의 onClick 이벤트에 Cook 함수 연결

        // RemoveButton 찾기
        removeButton = interactionPanel.Find("RemoveButton")?.GetComponent<Button>();
        if (removeButton == null)
        {
            Debug.LogError($"RemoveButton not found in InteractionPanel of {gameObject.name}");
            return;
        }
        removeButton.onClick.AddListener(ShowSelectionPanel); // RemoveButton의 onClick 이벤트에 RemoveIngredient 함수 연결

        // BackButton 찾기
        backButton = selectionPanel.Find("BackButton")?.GetComponent<Button>();
        if (backButton == null)
        {
            Debug.LogError($"BackButton not found in InteractionPanel of {gameObject.name}");
            return;
        }
        backButton.onClick.AddListener(HideSelectionPanel); // BackButton의 onClick 이벤트에 HideSelectionPanel함수 연결

        // 초기에 selectionPanel 비활성화
        interactionCanvas.gameObject.SetActive(false);
        interactionPanel.gameObject.SetActive(true);
        selectionPanel.gameObject.SetActive(false);

        // IconPanel은 언제나 활성화
        iconCanvas.gameObject.SetActive(true);
        iconPanel.gameObject.SetActive(true);
    }


    private void Update()
    { 
        UpdateAllButtons();
        HandleInteractionCanvas();
    }

    protected virtual void UpdateAllButtons()  // private일지 protected일지 고려 -> 조리대에서 버튼 하나 추가되면 바뀔 수 있을 듯
    {
        if (isCooking)
        {
            // 조리 중에는 모든 버튼 비활성화
            addButton.interactable = false;
            cookButton.interactable = false;
            removeButton.interactable = false;
            return;
        }

        if (PlayerController.Instance == null)
        {
            addButton.interactable = false;
            cookButton.interactable = false;
            removeButton.interactable = false;
            return;
        }

        Food heldFood = PlayerController.Instance.GetHeldFood();

        // 버튼 활성화/비활성화 상태 업데이트
        addButton.interactable = heldFood != Food.None && ingredients.Count < 4;        // AddButton: 음식이 있으면 활성화
        cookButton.interactable = ingredients.Count > 0;       // CookButton: 재료가 있으면 활성화
        removeButton.interactable = ingredients.Count > 0 && heldFood == Food.None;     // RemoveButton: 재료가 있으면 활성화
    }

    protected virtual void HandleInteractionCanvas()
    {

        if (PlayerController.Instance == null|| interactionCanvas == null) return;

        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);

        if (distanceToPlayer <= 3f) // 상호작용 범위 내
        {
            if (activeStation == null || activeStation == this)
            {
                ShowCanvas();
                activeStation = this;
            }
        }
        else
        {
            if (activeStation == this)
            {
                HideCanvas();
                activeStation = null;
            }
        }
    }

    private void ShowCanvas()
    {
        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(true);
        }
    }

    private void HideCanvas()
    {
        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(false);
        }
    }

    public virtual void AddIngredient()
    { 
        Debug.Log("AddIngredient!");
        if (PlayerController.Instance != null && PlayerController.Instance.heldFood != Food.None)
        {
            ingredients.Add(PlayerController.Instance.heldFood); // 재료 추가
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


    private void StartCook()
    {
        if (ingredients.Count == 0)
        {
            Debug.LogWarning("No ingredients to cook!");
            return;
        }

        isCooking = true;
        animator.SetBool("isCooking", true);

        // Player 비활성화
        PlayerController.Instance.SetMovementEnabled(false);

        // 버튼 상태 비활성화
        UpdateAllButtons();

        // 요리 진행
        Cook();
    }

    protected virtual void Cook()
    {
        // 조리기구에서 MiniGame을 진행하고 결과에 따라 CompleteCook를 호출하는 식으로 구현
    }

    protected void CompleteCook(bool isMiniGameSuccess)
    {
        StartCoroutine(CompleteCookWithDelay(isMiniGameSuccess));
    }

    private IEnumerator CompleteCookWithDelay(bool isMiniGameSuccess)
    {
        // 3초 대기
        yield return new WaitForSeconds(3f); // 3초 정도 대기

        // Recipe.Execute를 활용하여 결과 요리 확인
        Food resultFood = Food.실패요리;
        if (isMiniGameSuccess)
        {
            resultFood = Recipe.Execute(cookingMethod, ingredients);
        }

        if (resultFood != Food.실패요리)
        {
            Debug.Log($"Cooking successful: {resultFood}");
        }
        else
        {
            Debug.LogWarning("Cooking failed! No matching recipe.");
        }

        ingredients.Clear();
        ingredients.Add(resultFood);

        animator.SetBool("isCooking", false);
        isCooking = false;

        // Player 활성화
        PlayerController.Instance.SetMovementEnabled(true);

        // 조리 후 버튼 상태 업데이트
        UpdateAllButtons();

        // 조리 후 아이콘 상태 업데이트
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
            //CreateIngredientIcon(ingredient) // Database에 icon 채워지면 적용
            CreateIngredientId(ingredient);
        }
    }

    // Database에 icon 채워지면 적용
    // private void CreateIngredientIcon(Food ingredient)
    // {
    //     // Frame Prefab 복제
    //     GameObject frameObject = Instantiate(framePrefab, iconPanel);

    //     // Frame 내부에 Icon 생성
    //     Transform iconTransform = frameObject.transform.Find("Icon");
    //     if (iconTransform != null)
    //     {
    //         Image iconImage = iconTransform.GetComponent<Image>();
    //         if (iconImage != null)
    //         {
    //             // FoodDatabase에서 해당 재료의 아이콘 정보 가져오기
    //             FoodData foodData = foodDatabase.foodData.Find(data => data.Name == ingredient.ToString());
    //             if (foodData != null && foodData.Icon != null)
    //             {
    //                 // 아이콘 이미지 설정
    //                 iconImage.sprite = foodData.Icon.GetComponent<SpriteRenderer>().sprite;
    //             }
    //             else
    //             {
    //                 Debug.LogWarning($"Icon not found for ingredient {ingredient}");
    //             }
    //         }
    //         else
    //         {
    //             Debug.LogWarning("Icon Image component not found in Frame Prefab.");
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Icon Transform not found in Frame Prefab.");
    //     }

    //     // Frame 이름 설정 (디버그용)
    //     frameObject.name = $"Frame_{ingredient}";
    // }
    private void CreateIngredientId(Food ingredient)
    {
        // Frame Prefab 인스턴스 생성
        GameObject frameObject = Instantiate(framePrefab, iconPanel);

        // Frame 내부의 TextMeshPro 설정
        TMP_Text idText = frameObject.GetComponentInChildren<TMP_Text>();
        if (idText != null)
        {
            idText.text = ((int)ingredient).ToString(); // Food Enum의 ID 값 설정
        }
        else
        {
            Debug.LogWarning("TextMeshPro component not found in Frame Prefab!");
        }
    }
}
